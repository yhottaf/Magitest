using Cysharp.Threading.Tasks;
using fantec.Common;
using fantec.Menu.Manager;
using fantec.Menu.PartyEdit;
using fantec.Menu.PartySortie.View;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using fantec.PlayFabClient;
using fantec.Menu.PartySelect.View;
using System;
using DG.Tweening;
using fantec.Menu.NameChange.Controller;
using System.Threading;
using fantec.Menu.Card.View;

namespace fantec.Menu.PartySortie.Presenter
{
    public class PartySelectPresenter : MonoBehaviour
    {
        [SerializeField]
        private PartySortieView m_View;

        [SerializeField]
        private PartySortieCellController m_PartySortie;

        [SerializeField]
        private PartySelectContentView m_ContentView;
        [SerializeField]
        private ScreenSwipeObserver m_SwipeObserver;

        [SerializeField]
        private CommonProgressView m_ProgressView;

        [SerializeField]
        private GameObject m_StageImage;

        [SerializeField]
        private GameObject m_SortieWindow;

        [SerializeField]
        private CardListView m_CardListView;

        /// <summary>
        /// 情報表示タブの種類
        /// </summary>
        public enum InfoType
        {
            Enemy = 0,
            Reward = 1,
            Item = 2,
        }

        private InfoType m_InfoType = InfoType.Enemy;

        private const int PartySelectMin = 0;
        private const int PartySelectMax = Define.PARTY_NUM - 1;
        private readonly Vector3 partySortieInitialPos = new Vector3(0, 210.0f, 0);
        private readonly Vector3 stageImageInitialPos = new Vector3(-501.8f, 0, 0);

        // パーティ番号がどちらの方向に変化したか識別するためのenum
        public enum PartyChangeState { Next,Prev };

        private bool isClickable = true;
        private bool isAnimating = false;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private async void Start()
        {
            MenuManager.Instance.onLive2DState.OnNext(Unit.Default); // 非アクティブ状態にするため通知
            MenuManager.Instance.MoveQuestSelect = false;

            // ←スワイプで+, →スワイプで-
            m_SwipeObserver.OnSwipeLeft.Where(_=>!isAnimating).Subscribe(async unit => await OnSwipeLeft(unit)).AddTo(this);
            m_SwipeObserver.OnSwipeRight.Where(_=>!isAnimating).Subscribe(async unit => await OnSwipeRight(unit)).AddTo(this);

            Master.StageData stageData = MasterDataManager.Instance.StageMaster.GetData(MenuManager.Instance.SelectStageId);

            // プログレスビューの初期設定
            m_ProgressView.Initialize(Define.PARTY_NUM, PlayerPrefsManager.SelectPartyIndex);

            // 何番目かを渡す
            await m_PartySortie.PartySetting(PlayerPrefsManager.SelectPartyIndex, _cts.Token);

            m_View.UpdateQuestName(stageData.stageName);
            m_View.UpdateDirectoryName(PlayerPrefsManager.GetPartyName(PlayerPrefsManager.SelectPartyIndex));
            m_View.OnClickCloseButtonObservable.Subscribe(OnClickCloseButton).AddTo(this);
            m_View.OnClickBackButtonObservable.Subscribe(OnClickBackButton).AddTo(this);
            m_View.OnClickEditButtonObservable.Subscribe(OnActivateEditWindow).AddTo(this);
            m_View.OnClickRightButtonObservable.Where(_=>!isAnimating).Subscribe(async unit => await OnSwipeRight(unit)).AddTo(this);
            m_View.OnClickLeftButtonObservable.Where(_=>!isAnimating).Subscribe(async unit => await OnSwipeLeft(unit)).AddTo(this);
            m_View.OnClickSubmitButtonObservable.ThrottleFirst(TimeSpan.FromSeconds(1)).Where(_=>isClickable).Subscribe(_=>OnClickSubmitButton().Forget()).AddTo(this);
            m_View.OnClickDirectoryNameChangeObservable.Subscribe(OnClickNameChangeButton).AddTo(this);
            m_View.OnValueChangeEnemyInfoToggleObservable.Subscribe(isOn => { if (isOn) OnValueChangedToggle(InfoType.Enemy); }).AddTo(this);
            m_View.OnValueChangeRewardToggleObservable.Subscribe(isOn => { if (isOn) OnValueChangedToggle(InfoType.Reward); }).AddTo(this);
            m_View.OnValueChangedItemInfoToggleObservable.Subscribe(isOn => { if (isOn) OnValueChangedToggle(InfoType.Item); }).AddTo(this);

            // 出現敵情報
            Dictionary<int, List<int>> m_EnemyInfoList = new Dictionary<int, List<int>>();
            string waveKey = stageData.waveMasterKey;
            Master.WaveMaster enemyDatas = MasterDataManager.Instance.GetMaster<Master.WaveMaster>(waveKey);
            int maxIndex = enemyDatas.dataList.Max(x => x.waveIndex) + 1;
            for(int i=0;i<maxIndex;i++)
            {
                List<int> waveInfoList = new List<int>();
                var waveEnemyData=enemyDatas.dataList.Where(x=>x.waveIndex==i).ToList();
                foreach(var waveData in waveEnemyData)
                {
                    waveInfoList.Add(waveData.cardId);
                }
                m_EnemyInfoList.Add(i,waveInfoList);
            }

            // 報酬
            List<int>m_DropRewardInfoList=new List<int>();
            if (stageData.rewardTableId > 0)
            {
                Master.RewardStageData rewardData = MasterDataManager.Instance.RewardStageMaster.GetDataLocal(stageData.rewardTableId);
                int itemId = 0;
                if(int.TryParse(rewardData.itemId1,out itemId))
                {
                    m_DropRewardInfoList.Add(itemId);
                }
                if(int.TryParse(rewardData.itemId2,out itemId))
                {
                    m_DropRewardInfoList.Add(itemId);
                }
                if(int.TryParse(rewardData.itemId3, out itemId))
                {
                    m_DropRewardInfoList.Add(itemId);
                }
            }

            // アイテム
            var questItemList = InventoryManager.ConsumeItems.Where(x => MasterDataManager.Instance.ConsumeItemMaster.GetData(x.Key).effectType == ConsumeItemEffectType.クエスト);
            List<ItemInfoCell.Data> itemDataList = new List<ItemInfoCell.Data>();
            foreach(var questItem in questItemList)
            {
                ItemInfoCell.Data data=new ItemInfoCell.Data();
                data.itemId = MasterDataManager.Instance.ConsumeItemMaster.GetData(questItem.Key).itemId;
                data.itemNum = questItem.Value.RemainingUses.Value;
                itemDataList.Add(data);
            }

            await m_ContentView.Setup(m_EnemyInfoList, m_DropRewardInfoList, itemDataList);

            // 初めは敵情報を表示するようにする
            m_ContentView.ShowEnemyInfoContentView();
        }

        /// <summary>
        /// チーム選択の左スワイプ時の処理
        /// </summary>
        /// <param name="unit"></param>
        private async UniTask OnSwipeLeft(Unit unit)
        {
            if (CheckChangePartyData()) return;

            if (isAnimating) return;

            PlayerPrefsManager.SelectPartyIndex -= 1;
            if(PlayerPrefsManager.SelectPartyIndex<PartySelectMin)
            {
                PlayerPrefsManager.SelectPartyIndex = PartySelectMax;
            }

            await UpdateContents();
                
            // ディレクトリ名の更新
            m_View.UpdateDirectoryName(PlayerPrefsManager.GetPartyName(PlayerPrefsManager.SelectPartyIndex));

            // アニメーション
            ContentsAnim(PartyChangeState.Prev);

            // CardListPresenterへパーティ内容が変更されたことを通知する
            m_CardListView.ChangePartyData();

        }

        /// <summary>
        /// チーム選択の右スワイプ時の処理
        /// </summary>
        /// <param name="unit"></param>
        private async UniTask OnSwipeRight(Unit unit)
        {
            if(CheckChangePartyData())return;

            if (isAnimating) return;

            PlayerPrefsManager.SelectPartyIndex += 1;
            if(PlayerPrefsManager.SelectPartyIndex>PartySelectMax)
            {
                PlayerPrefsManager.SelectPartyIndex = PartySelectMin;
            }

            // 次のディレクトリの反映
            await UpdateContents();

            // ディレクトリ名の更新
            m_View.UpdateDirectoryName(PlayerPrefsManager.GetPartyName(PlayerPrefsManager.SelectPartyIndex));

            // アニメーション
            ContentsAnim(PartyChangeState.Next);

            // CardListPresenterへパーティ内容が変更されたことを通知する
            m_CardListView.ChangePartyData();
        }

        /// <summary>
        /// 出撃ボタン押下時
        /// </summary>
        private async UniTaskVoid OnClickSubmitButton()
        {
            int stamina = MasterDataManager.Instance.StageMaster.GetData(MenuManager.Instance.SelectStageId).stamina;

            //  if (stamina <= VirtualCurrencyManager.Stamina)
            if (stamina >= 0)
            {
                isClickable = false;

                await VirtualCurrencyManager.SubtractStaminaAsync(stamina);

                BGMManager.Instance.Stop();

                PlayerPrefsManager.LastTimeQuestId = MenuManager.Instance.SelectStageId;

                var lotteryForBattleData = await DummyServerForBattle.GetLotteryAsync(MenuManager.Instance.SelectStageId);

                // インゲーム側へデータを返す
                BridgingData brindgingData = BridgingDataProvider.Get;
                brindgingData.SetTeamData(UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex].GetTeamData());
                brindgingData.SetStageData(MasterDataManager.Instance.StageMaster.GetData(MenuManager.Instance.SelectStageId));
                brindgingData.SetLotteryData(lotteryForBattleData);
                BridgingDataProvider.Get.SetData(brindgingData);

                Loading.Show(0.5f, 1f, () => { ExSceneManager.Instance.LoadScene(SceneIndex.BATTLE, UnityEngine.SceneManagement.LoadSceneMode.Single); });
            }
            else
            {
                Debug.Log("消費するスタミナが足りません：現在回復アイテムによるスタミナ回復機能は未実装");
            }
        }

        /// <summary>
        /// バックボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickBackButton(Unit unit)
        {
            // 現在ロード中のアドレス一覧を確認
            var loading = AssetManager.Instance.GetLoadedAssets();
            foreach (var addr in loading)
            {
                Debug.Log($"Now loading: {addr}");
            }
            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.NoContents);
            // 自身の画面を破棄する
            //MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.PartySelect);
            //MenuWindowManager.Instance.OrganizeSibling(FooterType.Home);
            //MenuManager.Instance.MoveQuestSelect = true;

          //  AssetManager.Instance.ReleaseAllLoadedAssets();
        }

        /// <summary>
        /// 閉じるボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private async void OnClickCloseButton(Unit unit)
        {
            MenuManager.Instance.onLive2DState.OnNext(Unit.Default);
            await m_PartySortie.PartySlotSave(PlayerPrefsManager.SelectPartyIndex);
            MenuWindowManager.Instance.Remove(MenuWindowManager.CreateType.PartySelect);
            MenuWindowManager.Instance.OrganizeSibling(FooterType.Home);
            MenuManager.Instance.MoveQuestSelect = true;
        }

        /// <summary>
        /// ディレクトリの見た目を反映 (現在3つのデッキを用意していてデッキの切替を行い見た目を反映する)
        /// </summary>
        private async UniTask UpdateContents()
        {
            Debug.Log($"パーティ番号: {PlayerPrefsManager.SelectPartyIndex}");

            // パーティの内容を更新
            await m_PartySortie.PartySetting(PlayerPrefsManager.SelectPartyIndex, _cts.Token);
            // トグルの更新
            m_ProgressView.ChangeProgress(PlayerPrefsManager.SelectPartyIndex);

            //デバッグ 現在のリーダーが誰なのかを判別する
            m_PartySortie.GetLeaderCardID();
        }

        private async void ContentsAnim(PartyChangeState state)
        {
            isAnimating = true;

            // Tweenの強度(ベクトルに乗算する数値)
            float magnitude = 15f;
            // Tweenの持続時間
            float duration = 0.5f;

            // 既に再生中のアニメーションを完了状態にする(位置ズレを防ぐため)
            m_PartySortie.transform.DOKill();
            m_StageImage.transform.DOKill();
            m_PartySortie.transform.localPosition = partySortieInitialPos;
            m_StageImage.transform.localPosition = stageImageInitialPos;
            m_PartySortie.transform.DOComplete(true);
            m_StageImage.transform.DOComplete(true);

            // アニメーションの開始
            Tween sortieTween = null;
            Tween stageTween = null;

            switch (state)
            {
                case PartyChangeState.Next:
                    sortieTween = m_PartySortie.transform.DOPunchPosition(new Vector3(-10, 0, 0) * magnitude, duration, 2);
                    stageTween = m_StageImage.transform.DOPunchPosition(new Vector3(-10, 0, 0) * magnitude, duration, 2);
                    break;
                case PartyChangeState.Prev:
                    sortieTween = m_PartySortie.transform.DOPunchPosition(new Vector3(10, 0, 0) * magnitude, duration, 2);
                    stageTween = m_StageImage.transform.DOPunchPosition(new Vector3(10, 0, 0) * magnitude, duration, 2);
                    break;
            }

            // Tweenの終了を待つ
            await UniTask.WhenAll(
              sortieTween.AsyncWaitForCompletion().AsUniTask(),
              stageTween.AsyncWaitForCompletion().AsUniTask()
                                  );

            isAnimating = false;
        }

        /// <summary>
        /// ディレクトリ名変更ボタン押下時の処理 (デッキ名)
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickNameChangeButton(Unit unit)
        {
            if(MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.NameChange))
            {
                // 名前が返ってくる
                NameChangeController.Instance.m_DirectoryNameChange.Subscribe(item=>
                {
                    m_View.UpdateDirectoryName(item);
                    PlayerPrefsManager.SetPartyName(item,PlayerPrefsManager.SelectPartyIndex);
                }).AddTo(this);
            }
        }

        /// <summary>
        /// トグル値変更時
        /// </summary>
        /// <param name="type"></param>
        private void OnValueChangedToggle(InfoType type)
        {
            switch(type)
            {
                case InfoType.Enemy:
                    m_ContentView.ShowEnemyInfoContentView();
                    break;
                case InfoType.Reward:
                    m_ContentView.ShowRewardInfoContentView();
                    break;
                case InfoType.Item:
                    m_ContentView.ShowItemInfoContentView();
                    break;
            }
        }

        private void OnActivateEditWindow(Unit unit)
        {
            m_SortieWindow.SetActive(true);
        }

        /// <summary>
        /// スワイプする際にパーティー内容が変わっていたらダイアログを表示させる
        /// </summary>
        private bool CheckChangePartyData()
        {
            if (MenuManager.Instance.ChangePartyData)
            {
                PartyData partyData = UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex];
                bool empty = MenuManager.Instance.IsEmptyMember(partyData);
                if (empty)// 何も編成されていなかったらtrue
                {
                    // 1体もマギが編成されていませんのダイアログ表示
                    m_CardListView.SetNoticeWindow(true);
                    return true;
                }
                else // 現在の編成を保存しますかのダイアログを表示
                {
                    m_CardListView.SetCheckWindow(true);
                    return true;
                }
            }
            return false;
        }
    }
}