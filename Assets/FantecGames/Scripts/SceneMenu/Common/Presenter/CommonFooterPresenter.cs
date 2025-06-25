using fantec.Menu.Common.View;
using UnityEngine;
using UniRx;
using fantec.Menu.Manager;
using fantec.Common;
using fantec.PlayFabClient;

namespace fantec.Menu.Common.Presenter
{
    public class CommonFooterPresenter : MonoBehaviour
    {
        [SerializeField]
        private CommonFooterView m_View;

        private FooterType footerType = FooterType.NONE;

        private bool isClickable = true;
        void Start()
        {
            m_View.OnClickHomeButtonObservable.Subscribe(OnClickHomeButton).AddTo(this);
            m_View.OnClickQuestButtonObservable.Subscribe(OnClickTestQuestButton).AddTo(this);

            OnClickHomeButton(Unit.Default);
        }

        /// <summary>
        /// ホームボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickHomeButton(Unit unit)
        {
            if (footerType != FooterType.Home)
            {
                // SEManager.Instance.Play();

                MenuWindowManager.Instance.RemoveAll();
                MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.Home);
            }
        }

        // フッターのクエストボタンを押すと強制戦闘機能(サンプル)に遷移できるようにしておく
        private async void OnClickTestQuestButton(Unit unit)
        {
            if (footerType != FooterType.Quest)
            {
                // SEManager.Instance.Play();

                // スタミナが足りているか
                int stamina = MasterDataManager.Instance.StageMaster.GetData(MenuManager.Instance.SelectStageId).stamina;

                // if (stamina <= VirtualCurrencyManager.Stamina)
                //if (stamina >=0)
                //{
                //    isClickable = false;

                //    await VirtualCurrencyManager.SubtractStaminaAsync(stamina);

                //    BGMManager.Instance.Stop();

                //    PlayerPrefsManager.LastTimeQuestId = MenuManager.Instance.SelectStageId;

                //    var lotteryForBattleData = await DummyServerForBattle.GetLotteryAsync(MenuManager.Instance.SelectStageId);

                //    // インゲーム側へデータを返す
                //    BridgingData brindgingData = BridgingDataProvider.Get;
                //    brindgingData.SetTeamData(UserDataManager.PartyList[PlayerPrefsManager.SelectPartyIndex].GetTeamData());
                //    brindgingData.SetStageData(MasterDataManager.Instance.StageMaster.GetData(MenuManager.Instance.SelectStageId));
                //    brindgingData.SetLotteryData(lotteryForBattleData);
                //    BridgingDataProvider.Get.SetData(brindgingData);

                //    Loading.Show(0.5f, 1f, () => { ExSceneManager.Instance.LoadScene(SceneIndex.BATTLE, UnityEngine.SceneManagement.LoadSceneMode.Single); });
                //}
                //else
                //{
                //    Debug.Log("消費するスタミナが足りません：現在回復アイテムによるスタミナ回復機能は未実装");
                //}

                MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.PartySelect);
            }
        }
    }
}