using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

using fantec.Common;
using fantec.Utilities;
using fantec.PlayFabClient;
using UniRx;
using System.Data;
using fantec.Menu.Manager;

namespace fantec.Menu.Common
{
    /// <summary>
    /// ホームのヘッダー管理クラス
    /// </summary>
    public class HeaderManager:Singleton<HeaderManager>
    {
        [SerializeField] private Button m_MenuButton;
        [SerializeField] private Text m_RankNum;
        [SerializeField] private Image m_ExpMeter;
        [SerializeField] private Text m_MoneyText;
        [SerializeField] private Text m_StoneText;
        [SerializeField] private Text m_UserNameText;
        [SerializeField] private Text m_StaminaText;
        [SerializeField] private Text m_StaminaRecoveryTimeText;
        [SerializeField] private Text m_StaminaRecoveryInfoText;
        [SerializeField] private Image m_StaminaImage;

        /// <summary>
        /// スタミナのリチャージ時間
        /// </summary>
        private DateTime m_NextFreeTicket = new DateTime();

        /// <summary>
        /// スタミナが最大かどうか
        /// </summary>
        private bool isStaminaCapped;

        /// <summary>
        /// API実行中かどうか
        /// </summary>
        private bool isApiProgress;

        void Start()
        {
         //   UpdateRankText();
            UpdateExpUI();
            UpdateMoneyText();
            UpdateStoneText();
            UpdateUserNameText();

            m_MenuButton.OnClickAsObservable().Subscribe(OnClickMenuButton).AddTo(this);
        }

        async UniTask Update()
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                // スタミナが最大ではない場合
                if (isStaminaCapped == false)
                {
                    // リチャージ時間を迎えた場合はスタミナを再取得
                    if (m_NextFreeTicket.Subtract(DateTime.Now).TotalSeconds <= 0)
                    {
                        if (isApiProgress == false)
                        {
                            await GetInventory();
                        }
                    }
                    else
                    {
                        // 残り時間をカウントダウン
                        UpdateRecoveryTimeText(m_NextFreeTicket.Subtract(DateTime.Now));
                    }
                }
            }
        }

        /// <summary>
        /// スタミナ回復時間の更新
        /// </summary>
        /// <returns></returns>
        async UniTask GetInventory()
        {
            isApiProgress = true;
            var response=await PlayFabClientAPI.GetUserInventoryAsync(new GetUserInventoryRequest());
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }
            isApiProgress = false;

            //クライアントキャッシュの更新
            VirtualCurrencyManager.SyncPlayFabToClient(response.Result.VirtualCurrency);

            if (response.Result.VirtualCurrencyRechargeTimes.TryGetValue(VirtualCurrencyNames.ST.Code, out VirtualCurrencyRechargeTime rechargeDetails))
            {
                if(VirtualCurrencyManager.Stamina<UserDataManager.MaxStamina)
                {
                    m_NextFreeTicket = DateTime.Now.AddSeconds(rechargeDetails.SecondsToRecharge);
                    TimeSpan rechargeTime = m_NextFreeTicket.Subtract(DateTime.Now);

                //    m_StaminaRecoveryInfoText.text = $"全回復まで";
                    isStaminaCapped = false;
                }
                else
                {
                    isStaminaCapped = true;
                   // m_StaminaRecoveryInfoText.text = string.Empty;
                   //m_StaminaRecoveryTimeText.text = string.Empty;
                }

                //スタミナゲージの設定
                //  m_StaminaText.text = $"{VirtualCurrencyManager.Stamina}/{UserDataManager.MaxStamina}";
                // m_StaminaImage.fillAmount = (float)VirtualCurrencyManager.Stamina / (float)UserDataManager.MaxStamina;
                Debug.Log($"{VirtualCurrencyManager.Stamina}/{UserDataManager.MaxStamina}:現在のスタミナ:MAXスタミナ ");
            }

        }

        /// <summary>
        /// ランク更新時の処理
        /// </summary>
        public void UpdateRankText()
        {
            m_RankNum.text=UserDataManager.Level.ToString();
        }

        public void UpdateExpUI()
        {
            float percent = (float)VirtualCurrencyManager.Exp / (float)UserDataManager.NextLevelInfo.exp;
            m_ExpMeter.fillAmount = percent;

            int remaing = UserDataManager.NextLevelInfo.exp - VirtualCurrencyManager.Exp;
           //次のレベルまであと～～経験値が必要の処理を書いているならこの下にUpdateViewを書く
        }

        /// <summary>
        /// マネー更新時の処理
        /// </summary>
        public void UpdateMoneyText()
        {
            m_MoneyText.text = VirtualCurrencyManager.Money.ToString("N0");
        }

        /// <summary>
        /// 石更新時の処理
        /// </summary>
        public void UpdateStoneText()
        {
            m_StoneText.text = VirtualCurrencyManager.Stone.ToString("N0");
        }

        /// <summary>
        /// スタミナ回復時間のテキスト更新
        /// </summary>
        /// <param name="timeSpan"></param>
        private void UpdateRecoveryTimeText(TimeSpan timeSpan)
        {
         //   Debug.Log($"{timeSpan.Hours.ToString("D2")}:{timeSpan.Minutes.ToString("D2")}:{timeSpan.Seconds.ToString("D2")}");
        //    m_StaminaRecoveryTimeText.text = $"{timeSpan.Hours.ToString("D2")}:{timeSpan.Minutes.ToString("D2")}:{timeSpan.Seconds.ToString("D2")}";
        }

        /// <summary>
        /// スタミナ回復ボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickStaminaHealButton(Unit unit)
        {
            Debug.Log("スタミナ回復ボタン押下");
        }

        /// <summary>
        /// メニューボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickMenuButton(Unit unit)
        {
            Debug.Log("メニューボタン押下");
            MenuWindowManager.Instance.Create(MenuWindowManager.CreateType.Notice);
        }

        /// <summary>
        /// 石購入ボタン押下時
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickStonePlusButton(Unit unit)
        {
            Debug.Log("石購入ボタン押下時");

        }

        /// <summary>
        /// コイン追加ボタン
        /// </summary>
        /// <param name="unit"></param>
        private void OnClickCoinPlusButton(Unit unit)
        {
            Debug.Log("コイン追加ボタン押下時");
        }

        /// <summary>
        /// プレイヤー名を更新する
        /// </summary>
        public void UpdateUserNameText()
        {
            m_UserNameText.text = PlayerProfileManager.UserDisplayName;
        }
    }
}