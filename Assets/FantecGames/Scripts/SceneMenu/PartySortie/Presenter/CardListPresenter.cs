using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Master;
using fantec.Menu.Card.View;
using fantec.Menu.Manager;
using fantec.Menu.PartyEdit;
using fantec.Menu.PartySortie;
using fantec.PlayFabClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

namespace fantec.Menu.Card.Presernter
{
    public class CardListPresenter : MonoBehaviour
    {
        [SerializeField]
        private CardListGridView m_GridView;
        [SerializeField]
        private PartySortieCellController m_PartySortie;
        [SerializeField]
        private CommonProgressView m_ProgressView;
        [SerializeField]
        private CardListView m_View;

        private List<CardData> m_PlayerCardList;// ソート等に対応できるようにコピーをとっておく
        private List<CardData> m_BeforeList;    // 現在の描画しているソートを弄る前の生のリスト

        private List<int> previousMemberList=new List<int>();// 変更前の編成リスト情報

        private CancellationTokenSource _cts = new CancellationTokenSource();

        // カードIDとポジションインデックスのセットを保持するリスト
        private List<Tuple<int, List<int>>> cardPositionList = new List<Tuple<int, List<int>>>();
        private List<Tuple<int, List<int>>> PrevPositionList = new List<Tuple<int, List<int>>>();

        private readonly List<int> dummyPosition = new List<int>() { 10, 10, 10, 10, 10 };

        private void OnEnable()
        {
            // 所持Magiリストの更新
            if (m_BeforeList != null)
            {
                ViewUpdate();
            }

            // 編成をいじるまえのデータをバックアップしておく
            // (編成を破棄しますか？→はいで元に戻すため)
            CopyDefaultPartyData();
        }

        void Start()
        {
            //  AddListener
            m_GridView.OnCellClicked(index => OnClickCharacterCell(index));
            m_GridView.OnCellLongTaped(index => OnLongTapCharacterCell(index));
            m_View.OnClickBackButtonObservable.Subscribe(OnClickBackButton).AddTo(this);
            m_View.OnClickYesButtonObservable.Subscribe(OnClickNoticeYesButton).AddTo(this);
            m_View.OnClickNoButtonObservable.Subscribe(OnClickNoticeNoButton).AddTo(this);
            m_View.OnClickCheckYesButtonObservable.Subscribe(OnClickCheckYesButton).AddTo(this);
            m_View.OnClickCheckNoButtonObservable.Subscribe(OnClickCheckNoButton).AddTo(this);

            // キャラクターセル初期化
            Initialized();

 
            m_PartySortie.changeCardData.Subscribe(updatedList =>
            {
                // マギの場所を変更した際に購読される
                foreach (var tuple in updatedList)
                {
                    SaveCardPosition(tuple.Item1,tuple.Item2);
                    MenuManager.Instance.ChangePartyData = CheckMemberListChanges();
                    Debug.Log($"パーティを更新: {(MenuManager.Instance.ChangePartyData ? "した" : "してない")}");
                
                }
            }).AddTo(this);

            m_View.OnNextSwipe.Subscribe(_ =>
            {
                // スワイプが行われた際に購読される
                Initialized();
                CopyDefaultPartyData();
                Debug.Log($"-----------------(ディレクトリ番号：{PlayerPrefsManager.SelectPartyIndex+1})------------------------");
            }).AddTo(this);
        }

        /// <summary>
        /// キャラクターセル初期化処理
        /// </summary>
        private void Initialized()
        {
            // プレイヤーが所持するマギをID昇順で取得
            m_PlayerCardList = CardManager.CardDatas;
            m_PlayerCardList.Sort((a, b) => a.cardId.CompareTo(b.cardId));



            // グリッドに表示
            m_GridView.UpdateContents(m_PlayerCardList);

            // ID昇順で並べられている生データを保持
            m_BeforeList = new List<CardData>(m_PlayerCardList);
        }

        private void ViewUpdate()
        {
            m_PlayerCardList= m_BeforeList;

            m_GridView.UpdateContents(m_PlayerCardList);
        }

        // 普通にタップした時
        private void OnClickCharacterCell(int index)
        {
            CardData data = m_PlayerCardList[index];
            MenuManager.Instance.SelectCardId = data.cardId;
            Debug.Log($"選択ID:{data.cardId}");

            if (MenuManager.Instance.SelectCardId == 0)
            {
                Debug.Log("キャラクターが選択されていません");
                return;
            }

            // 現在のディレクトリのメンバーIDリストを取得
            //List<int> MemberList = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex].MemberList;
            PartyData partyData = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex];

            if (partyData.MemberList.Contains(data.cardId))
            {
                //選択したカードがディレクトリに編成中なら
                int cardindex = partyData.MemberList.IndexOf(data.cardId);
                CardData TargetCardData = CardManager.GetCardData(data.cardId);
                partyData.MemberList[cardindex] = -1; // 空を配置する。
                SaveCardPosition(partyData.MemberList[cardindex], TargetCardData.positionIndex);
                // ディレクトリへの反映
                UpdateContents().Forget(); // ← 非同期で呼び出し
            }
            else
            if (partyData.MemberList.Contains(-1))
            {
                // パーティーに空きがあったら、そのインデックスを調べ、そこに選択したMagiを入れこむ
                int emptyIndex = partyData.MemberList.IndexOf(-1);
                partyData.MemberList[emptyIndex] = data.cardId;


                // タップしたカード情報と現在のディレクトリのインデックスを変数に保存
                CardData TargetCardData = CardManager.GetCardData(data.cardId);
                int selectedIndex = PlayerPrefsManager.SelectPartyIndex;

                // タップしたカード以外の編成中のポジションを集める
                HashSet<int> usedPositions = new HashSet<int>();
                for (int i = 0; i < partyData.MemberList.Count; i++)
                {
                    int otherCardId = partyData.MemberList[i];
                    if (otherCardId == data.cardId || otherCardId == -1 || otherCardId == 0) continue;

                    CardData otherCard = CardManager.GetCardData(otherCardId);
                    int pos = otherCard.positionIndex[selectedIndex];
                    usedPositions.Add(pos);
                }

                // 使用されていないPlaceMentPointを探し、前列から自動編成していく
                List<int> PlaceMentPoint = new List<int>() { 1, 5, 2, 4, 8, 6, 0, 7, 3 };

                foreach (int point in PlaceMentPoint)
                {
                    if (!usedPositions.Contains(point))
                    {
                        // カードの場所を一時的に保存
                        TargetCardData.positionIndex[selectedIndex] = point;
                        SaveCardPosition(data.cardId,TargetCardData.positionIndex);
                        MenuManager.Instance.ChangePartyData = true;
          //              CardManager.UpdateCardPositionIndex(data.cardId,TargetCardData.positionIndex).Forget();
                        break;
                    }
                }

                // プレイヤーが所持するマギをID昇順で取得
                m_PlayerCardList = CardManager.CardDatas;
                m_PlayerCardList.Sort((a, b) => a.cardId.CompareTo(b.cardId));

                // ディレクトリへの反映
                UpdateContents().Forget(); // ← 非同期で呼び出し
            }
            m_GridView.UpdateContents(m_PlayerCardList);
            // パーティ情報を更新したかどうかをみる
            MenuManager.Instance.ChangePartyData = CheckMemberListChanges();
            Debug.Log($"パーティを更新: {(MenuManager.Instance.ChangePartyData ? "した" : "してない")}");
        }

        // 長押ししたとき(1秒間)
        // 詳細画面を開くようにする
        private void OnLongTapCharacterCell(int index)
        {
            CardData data = m_BeforeList[index];
            MenuManager.Instance.SelectCardId = data.cardId;
            Debug.Log($"選択ID:{data.cardId}");

            if(MenuManager.Instance.SelectCardId==0)
            {
                Debug.Log("キャラクターが選択されていません");
                return;
            }

            LocalDataManager.Instance.LocalData.AddCheckCard(data.cardId);
            Debug.Log("詳細画面を表示させる");
            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.CardDetail);
        }

        /// <summary>
        /// ディレクトリの見た目を反映 
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateContents()
        {
            // パーティーの内容を更新
            await m_PartySortie.PartySetting(PlayerPrefsManager.SelectPartyIndex, _cts.Token);

            m_ProgressView.ChangeProgress(PlayerPrefsManager.SelectPartyIndex);
        }

        // SortieViewを閉じる
        private void OnClickBackButton(Unit unit)
        {
            List<int> MemberList = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex].MemberList;
            if (MemberList.All(id => id == -1 || id == 0))
            {
                // すべて -1 または 0 で構成されている場合の処理
                m_View.SetNoticeWindow(true);
                return;
            }

            if(MenuManager.Instance.ChangePartyData)
            {
                m_View.SetCheckWindow(true);
                return;
            }

            this.gameObject.SetActive(false);

            UserDataManager.UpdatePlayFab().Forget();
            
            // 現在のディレクトリをセーブする
            m_PartySortie.PartySlotSave(PlayerPrefsManager.SelectPartyIndex).Forget();
        }

        // 編成が弄られているかどうかを返す
        public bool CheckMemberListChanges()
        {
            var partyData = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex];


            bool isMemberListChanged = !previousMemberList.SequenceEqual(partyData.MemberList);


            bool isPositionListChanged = false;


            // サイズが違う場合は変更されたとみなす
            if (cardPositionList.Count != PrevPositionList.Count)
            {
                isPositionListChanged = true;
            }
            else
            {
                // 中身を比較
                foreach (var current in cardPositionList)
                {
                    var previous = PrevPositionList.FirstOrDefault(p => p.Item1 == current.Item1);

                    if (previous == null)
                    {
                        // 前のリストに存在しない場合も変更とみなす
                        isPositionListChanged = true;
                        break;
                    }

                    // 手動でリストの内容を比較する
                    if (previous.Item2.Count != current.Item2.Count ||
                        !previous.Item2.Zip(current.Item2, (a, b) => a == b).All(equal => equal))
                    {
                        isPositionListChanged = true;
                        break;
                    }
                }
            }


            if (isMemberListChanged)
            {
                Debug.Log($"[Check] MemberList の内容が変更されています。\nBefore: {string.Join(", ", previousMemberList)}\nAfter: {string.Join(", ", partyData.MemberList)}");
            }

            if (isPositionListChanged)
            {
                Debug.Log("[Check] PositionList の内容が変更されています。");
                Debug.Log($"場所を変更前: {string.Join(", ", PrevPositionList.Select(x => $"{x.Item1}: {string.Join(", ", x.Item2)}"))}");
                Debug.Log($"場所を変更後: {string.Join(", ", cardPositionList.Select(x => $"{x.Item1}: {string.Join(", ", x.Item2)}"))}");
            }

            // 場所を変えたか、編成の内容を弄った場合は true を返す
            if (isMemberListChanged || isPositionListChanged)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// カードIDとポジションインデックスのペアを保存する
        /// もしカードIDが存在する場合は上書きする
        /// </summary>
        /// <param name="cardId">カードID</param>
        /// <param name="positionIndex">ポジションインデックス</param>
        private void SaveCardPosition(int cardId, List<int> positionIndex)
        {
            if (cardId != -1)
            {
                // すでに存在するか確認
                var existingItem = cardPositionList.FirstOrDefault(tuple => tuple.Item1 == cardId);

                if (existingItem != null)
                {
                    // 存在する場合は削除してから追加（上書き）
                    cardPositionList.Remove(existingItem);
                    //Debug.Log($"カードID {cardId} のポジションを {positionIndex} に更新しました。");
                }
            }


            //  cardId が -1 の場合、同じポジションにある他のカードを削除する
            if (cardId == -1)
            {
                // PlayerPrefsManager.SelectPartyIndex 番目の位置を取得
                int targetPosition = positionIndex[PlayerPrefsManager.SelectPartyIndex];

                // 同じ位置にある他のカードを検索
                var overlappingItems = cardPositionList
                    .Where(tuple => tuple.Item1 != cardId &&
                                    tuple.Item2.Count > PlayerPrefsManager.SelectPartyIndex &&
                                    tuple.Item2[PlayerPrefsManager.SelectPartyIndex] == targetPosition)
                    .ToList();

                // 削除処理
                foreach (var item in overlappingItems)
                {
                    cardPositionList.Remove(item);
                    Debug.Log($"重複が発生したため、カードID {item.Item1} をリストから削除しました。");
                }
            }
            else
            {
                //  -1 の場合、リストの中で -1 のものを探して置き換える
                var negativeOneItem = cardPositionList.FirstOrDefault(tuple => tuple.Item1 == -1);

                if (negativeOneItem != null)
                {
                    // -1 のデータを削除して新しいカードIDで追加する
                    cardPositionList.Remove(negativeOneItem);
                    Debug.Log($"-1 のカードIDを削除し、新しいカードID {cardId} に置き換えます。");
                }
                Debug.Log($"カードID {cardId} のポジションを最終的に保存しました: {string.Join(", ", positionIndex)}");
            }
            // 新しいデータを追加
            cardPositionList.Add(new Tuple<int, List<int>>(cardId, positionIndex));
        }

        // 編成をいじるまえのデータをバックアップしておく
        private void CopyDefaultPartyData()
        {
            previousMemberList.Clear();
            PrevPositionList.Clear();
            cardPositionList.Clear();

            // 現在の MemberList の内容をコピーして保持
            var partyData = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex];
            previousMemberList = new List<int>(partyData.MemberList);

            for (int i = 0; i < partyData.MemberList.Count; i++)
            {
                int CardId = partyData.MemberList[i];

                if (CardId != -1)
                {
                    CardData carddata = CardManager.GetCardData(CardId);

                    // **深いコピー**: 新しいリストインスタンスに値をコピーする
                    List<int> posCopy = new List<int>(carddata.positionIndex);

                    // Deep Copyしたリストを使用してTupleを生成する
                    PrevPositionList.Add(new Tuple<int, List<int>>(CardId, posCopy));
                    cardPositionList.Add(new Tuple<int, List<int>>(CardId, posCopy));
                }
                else// 空のデータが入っていた場合はダミーのポジションを用いる
                {
                    // Deep Copyしたリストを使用してTupleを生成する
                    PrevPositionList.Add(new Tuple<int, List<int>>(-1, dummyPosition));
                    cardPositionList.Add(new Tuple<int, List<int>>(-1, dummyPosition));
                }
            }
        }

        // 編成を保存せず破棄しますか？→はいを押した場合(Magiが一体も編成されていませんver)
        private void OnClickNoticeYesButton(Unit unit)
        {
            RestorePreviousPartyData();

            m_View.SetNoticeWindow(false);
            
            this.gameObject.SetActive(false);
        }

        // 編成を保存せず破棄しますか？→いいえを押した場合(Magiが一体も編成されていませんver)
        private void OnClickNoticeNoButton(Unit unit)
        {
            m_View.SetNoticeWindow(false);
        }

        // 現在の編成を保存しますか？→はいを押した場合
        private void OnClickCheckYesButton(Unit unit)
        {
            // 編成を保存する
            m_View.SetCheckWindow(false);
            foreach (var data in cardPositionList)
            {
                CardManager.UpdateCardPositionIndex(data.Item1, data.Item2).Forget();
                Debug.Log($"カードID {data.Item1} のポジションを最終的に保存しました: {string.Join(", ", data.Item2)}");
            }

            // 現在のディレクトリをセーブする
            m_GridView.UpdateContents(m_PlayerCardList);
            m_PartySortie.PartySlotSave(PlayerPrefsManager.SelectPartyIndex).Forget();
            UserDataManager.UpdatePlayFab().Forget();
            MenuManager.Instance.ChangePartyData = false;
        }

        // 現在の編成を保存しますか？→いいえを押した場合
        private void OnClickCheckNoButton(Unit unit)
        {
            // 編成を保存せず破棄する
            RestorePreviousPartyData();

            m_View.SetCheckWindow(false);
        }

        /// <summary>
        /// 編成を元の状態に戻す処理
        /// </summary>
        private void RestorePreviousPartyData()
        {
            // 弄るまえの編成データを復元する
            PartyData partyData = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex];
            partyData.MemberList.Clear();
            partyData.MemberList.AddRange(previousMemberList);

            foreach (var prevData in PrevPositionList)
            {
                var cardData = CardManager.GetCardData(prevData.Item1);
                if (cardData != null)
                {
                    // 保存していた位置情報を復元
                    cardData.positionIndex = new List<int>(prevData.Item2);
                    Debug.Log($"[Restore] CardID: {prevData.Item1} のポジションを元の状態に戻しました: {string.Join(", ", prevData.Item2)}");
                }
            }

            // ビューの更新
            UpdateContents().Forget();
            m_GridView.UpdateContents(m_BeforeList);
            MenuManager.Instance.ChangePartyData = false;
        }
    }
}