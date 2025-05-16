using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Menu.Manager;
using fantec.PlayFabClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fantec.Menu.PartySortie
{
    public class PartySortieCellController : MonoBehaviour
    {
        [SerializeField]
        private List<PartySortieCharaObject> m_CharaObjects;
        [SerializeField]
        private List<GameObject> TargetPosition;
        [SerializeField]
        private List<Image> TargetImage; // 配置場所のレイを当てるためのImageComponent
                                         // パーティーの変更を通知するSubject
        public Subject<List<Tuple<int, List<int>>>> changeCardData = new Subject<List<Tuple<int, List<int>>>>();

        private int? m_CurrentDraggingIndex = null;

        private readonly int dummyPosition = 100;
        private void Start()
        {
            for (int i = 0; i < m_CharaObjects.Count; i++)
            {
                int cellIndex = i;
                var obj = m_CharaObjects[cellIndex];

                // 初回購読
                RegisterInputHandlers(cellIndex);

                // GameObject が有効化されたときに再購読
                obj.OnEnableAsObservable()
                    .Subscribe(_ =>
                    {
                        RegisterInputHandlers(cellIndex);
                    })
                    .AddTo(obj);
            }
        }

        private void Update()
        {
            if (m_CurrentDraggingIndex.HasValue)
            {
                // 長押しで生成されるゴーストの位置の更新

                Vector3 mouseWorldPos = Input.mousePosition;
                mouseWorldPos.z = 0f;

                // Canvasに合わせた変換（WorldSpaceでやる場合はカメラが必要）
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseWorldPos);
                worldPos.z = 0;

                m_CharaObjects[m_CurrentDraggingIndex.Value].UpdateGhostPosition(worldPos);
            }
        }

        /// <summary>
        /// 長押し時の処理
        /// </summary>
        private void OnLongTap(int cellIndex)
        {
            // SE再生処理？
            foreach (var obj in TargetImage)
            {
                obj.raycastTarget = true;
            }
            MenuManager.Instance.SelectCardId = GetCardId(cellIndex);

        }

        /// <summary>
        /// 長押しを離したとき
        /// </summary>
        /// <param name="cellIndex"></param>
        private async void OnLongTapEnd(int cellIndex)
        {
            // PointerEventData を取得して Raycast 結果からヒットした UI を特定
            PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            foreach (var result in raycastResults)
            {
                for (int i = 0; i < TargetImage.Count; i++)
                {
                    if (result.gameObject == TargetImage[i].gameObject)
                    {
                        // タップしていたMagi
                        CardData Tapcard = CardManager.GetCardData(GetCardId(cellIndex));

                        // 既にその位置を使っているMagiがいるか確認
                        int? targetIndex = null;
                        for (int j = 0; j < m_CharaObjects.Count; j++)
                        {
                            if (j == cellIndex) continue;

                            int cardId = GetCardId(j);
                            if (cardId == -1 || cardId == 0) continue; // -1 または 0 の場合はスキップ

                            if (m_CharaObjects[j].PositionIndex == i)
                            {
                                targetIndex = j;
                                break;
                            }
                        }

                        List<Tuple<int, List<int>>> updatedCardData = new List<Tuple<int, List<int>>>();

                        //　離した場所に別のMagiが既に配置されていた場合。
                        if (targetIndex.HasValue)
                        {
                            // 入れ替え処理
                            int swapIndex = targetIndex.Value;
                            CardData swapCard = CardManager.GetCardData(GetCardId(swapIndex));

                            int draggingOld = Tapcard.positionIndex[PlayerPrefsManager.SelectPartyIndex];
                            int swapOld = swapCard.positionIndex[PlayerPrefsManager.SelectPartyIndex];

                            // 入れ替え
                            Tapcard.positionIndex[PlayerPrefsManager.SelectPartyIndex] = swapOld;
                            swapCard.positionIndex[PlayerPrefsManager.SelectPartyIndex] = draggingOld;

                            m_CharaObjects[cellIndex].SetPositionIndex(swapOld);
                            m_CharaObjects[swapIndex].SetPositionIndex(draggingOld);

                            // Spine位置更新
                            m_CharaObjects[cellIndex].transform.position = TargetPosition[swapOld].transform.position;
                            m_CharaObjects[swapIndex].transform.position = TargetPosition[draggingOld].transform.position;

                            // サーバーへ保存
                            await UniTask.WhenAll(
                                CardManager.UpdateCardPositionIndex(Tapcard.cardId, Tapcard.positionIndex),
                                CardManager.UpdateCardPositionIndex(swapCard.cardId, swapCard.positionIndex)
                            );

                            // 更新データをリストに追加
                            updatedCardData.Add(new Tuple<int, List<int>>(Tapcard.cardId, Tapcard.positionIndex));
                            updatedCardData.Add(new Tuple<int, List<int>>(swapCard.cardId, swapCard.positionIndex));

                            Debug.Log($"カードID {Tapcard.cardId} と {swapCard.cardId} の positionIndex を入れ替えました");
                        }
                        else
                        {
                            // 通常配置（変更がある場合のみ）
                            if (Tapcard.positionIndex[PlayerPrefsManager.SelectPartyIndex] != i)
                            {
                                Tapcard.positionIndex[PlayerPrefsManager.SelectPartyIndex] = i;
                                m_CharaObjects[cellIndex].SetPositionIndex(i);

                                // サーバーに変更を送信
                                await CardManager.UpdateCardPositionIndex(Tapcard.cardId, Tapcard.positionIndex);
                                m_CharaObjects[cellIndex].transform.position = TargetPosition[i].transform.position;

                                updatedCardData.Add(new Tuple<int, List<int>>(Tapcard.cardId, Tapcard.positionIndex));

                                Debug.Log($"カードID {Tapcard.cardId} の 位置 を {i + 1}番目 に設定しました");
                            }
                            else
                            {
                                //Debug.LogWarning($"位置 {i+1}番目 は前回と同じ場所なので変更しませんでした。");
                            }
                        }

                        if (updatedCardData.Any())
                        {
                            changeCardData.OnNext(updatedCardData);
                        }

                        // 変更処理が終わったあとに9つのマスのレイキャストをオフにする
                        foreach (var obj in TargetImage)
                        {
                            obj.raycastTarget = false;
                        }

                        return;
                    }
                }
            }
            // 長押しを離した場所が何もなかった場合、そのまま9つのマスのレイキャストをオフに設定
            foreach (var obj in TargetImage)
            {
                obj.raycastTarget = false;
            }
        }

        /// <summary>
        /// 見た目の変更
        /// </summary>
        public async UniTask PartySetting(int PartyID,CancellationToken cts)
        {
            PartyData partyData = UserDataManager.PartyList[PartyID];

            // 一時保存用
            List<int> positionIndexes = new List<int>();
            List<CardData> cardDatas = new List<CardData>();

            for (int i = 0; i < m_CharaObjects.Count; i++)
            {
                int cardId = partyData.MemberList[i];

                if (cardId != -1)
                {
                    CardData cardData = CardManager.GetCardData(cardId);
                    cardDatas.Add(cardData);
                    positionIndexes.Add(cardData.positionIndex[PlayerPrefsManager.SelectPartyIndex]);
                }
                else
                {
                    CardData dummyData = new CardData(cardId: -1,
                                   rarityType: CardRarityType.R1,
                                                  totalExp: 1,
                                   purchaseDateTime: DateTime.Now,
                                            overrideSkillLevel: 1,
                          positionIndex: new List<int> { dummyPosition, dummyPosition, dummyPosition, dummyPosition, dummyPosition });

                    cardDatas.Add(dummyData);
                    positionIndexes.Add(dummyPosition);// 仮の数値
                    m_CharaObjects[i].gameObject.SetActive(false);
                }
            }


            //ダミーの場所以外の同じ数値が１つでも被っていたらtrue 
            bool anyValue = positionIndexes
                                   .Where(x => x != dummyPosition) // ダミーの場所は除外
                                   .GroupBy(x => x)
                                   .Any(g => g.Count() > 1); 

            if (anyValue)
            {
                int[] fixedPositions = { 6, 3, 0 };

                for (int i = 0; i < m_CharaObjects.Count && i < fixedPositions.Length; i++)
                {
                    cardDatas[i].positionIndex[PlayerPrefsManager.SelectPartyIndex] = fixedPositions[i];
                }
            }

            // 並列でSettingsを実行
            var settingTasks = new List<UniTask>();

            // 最終的な設定反映
            for (int i = 0; i < m_CharaObjects.Count; i++)
            {
                int index = i;
                int cardId = cardDatas[index].cardId;

                settingTasks.Add(m_CharaObjects[index].Settings(cardId, cts));
            }

            await UniTask.WhenAll(settingTasks);



            // transform.positionの設定（Settings後に行う）
            for (int i = 0; i < m_CharaObjects.Count; i++)
            {
                if (cardDatas[i].positionIndex[0] == 9)
                {
                    cardDatas[i].positionIndex[0] = 8;
                }

                m_CharaObjects[i].transform.position = TargetPosition[m_CharaObjects[i].PositionIndex].transform.position;
            }
        }

        // 一括で編成に必要な情報をPlayFabに保存する
        public async UniTask PartySlotSave(int PartyID)
        {
            PartyData partyData = UserDataManager.PartyList[PartyID];

            // 一時保存用
            List<int> positionIndexes = new List<int>();
            List<CardData> cardDatas = new List<CardData>();

            for (int i = 0; i < m_CharaObjects.Count; i++)
            {
                int cardId = partyData.MemberList[i];
                if (cardId != -1)
                {
                    CardData cardData = CardManager.GetCardData(cardId);
                    cardDatas.Add(cardData);
                    positionIndexes.Add(cardData.positionIndex[PlayerPrefsManager.SelectPartyIndex]);
                }
                else
                {
                    CardData dummyData = new CardData(cardId: -1,
                                   rarityType: CardRarityType.R1,
                                                  totalExp: 1,
                                   purchaseDateTime: DateTime.Now,
                                            overrideSkillLevel: 1,
                          positionIndex: new List<int> { dummyPosition, dummyPosition, dummyPosition, dummyPosition, dummyPosition });

                    cardDatas.Add(dummyData);
                    positionIndexes.Add(100);// 仮の数値
                    m_CharaObjects[i].gameObject.SetActive(false);
                }
            }

            // UI設定とTaskの準備
            var updateTasks = new List<UniTask>();

            for (int i = 0; i < m_CharaObjects.Count; i++)
            {
                int cardId = cardDatas[i].cardId;
                await m_CharaObjects[i].Settings(cardId);
                m_CharaObjects[i].transform.position = TargetPosition[m_CharaObjects[i].PositionIndex].transform.position;

                updateTasks.Add(CardManager.UpdateCardPositionIndex(cardId, cardDatas[i].positionIndex));
            }

            // すべての更新を並列で待つ
            await UniTask.WhenAll(updateTasks);
        }

        public int GetCardId(int value)
        {
            return m_CharaObjects[value].CardId;
        }

        public int GetCardPositionIndex(int value)
        {
            return m_CharaObjects[value].PositionIndex;
        }

        private void RegisterInputHandlers(int cellIndex)
        {
            var obj = m_CharaObjects[cellIndex];

            // 購読の競合や多重登録を防ぐために、一度破棄（AddTo(obj)していれば自動的にOK）
            obj.OnPointerDownAsObservable
                .Subscribe(_ =>
                {
                    OnLongTap(cellIndex);
                    m_CurrentDraggingIndex = cellIndex;
                })
                .AddTo(this); // GameObjectにバインドして再登録時に自動破棄

            obj.OnPointerUpAsObservable
                .Subscribe(_ =>
                {
                    OnLongTapEnd(cellIndex);
                    m_CurrentDraggingIndex = null;
                })
                .AddTo(this);
        }

        // 現在のディレクトリのパーティの中で誰がリーダーなのか、そのカードIDを返す
        public int GetLeaderCardID()
        {
            int leaderCardId = -1;
            PartyData partyData = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex];
            // 最初に -1 でないカードIDを探す
            for (int i = 0; i < partyData.MemberList.Count; i++)
            {
                if (partyData.MemberList[i] != -1)
                {
                    leaderCardId = partyData.MemberList[i];
                    break;
                }
            }
            Debug.Log($"リーダーは {leaderCardId} です");
            return leaderCardId;
        }
    }
}