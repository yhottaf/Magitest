using Cysharp.Threading.Tasks;
using fantec.PlayfabCilent;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace fantec.PlayFabClient
{
    public static class VirtualCurrencyManager
    {
        /// <summary>
        /// スタミナ
        /// </summary>
        public static int Stamina => UserDataManager.MaxStamina > StaminaRaw ? StaminaRaw : UserDataManager.MaxStamina;
        private static int StaminaRaw { get; set; }

        /// <summary>
        /// 経験値
        /// </summary>
        public static int Exp { get; private set; }

        /// <summary>
        /// 有償石、無償石の合計数
        /// </summary>
        public static int Stone
        {
            get
            {
                return PaidStone+FreeStone;
            }
        }

        /// <summary>
        /// 有償石
        /// </summary>
        public static int PaidStone { get; private set; }

        /// <summary>
        /// 無償石
        /// </summary>
        public static int FreeStone { get; private set; }
        
        /// <summary>
        /// マネー
        /// </summary>
        public static int Money { get; private set; }

        /// <summary>
        /// フレンドポイント
        /// </summary>
        public static int FriendPoint { get; private set; }

        /// <summary>
        /// PlayFabから最新のデータを取得してローカルにキャッシュする
        /// </summary>
        /// <param name="currency"></param>
        public static void SyncPlayFabToClient(Dictionary<string,int>currency)
        {
            StaminaRaw = currency.TryGetValue(VirtualCurrencyNames.ST.Code, out var st) ? st : 0;
            Exp = currency.TryGetValue(VirtualCurrencyNames.EP.Code, out var ep) ? ep : 0;
            PaidStone = currency.TryGetValue(VirtualCurrencyNames.PS.Code, out var ps) ? ps : 0;
            FreeStone=currency.TryGetValue(VirtualCurrencyNames.FS.Code,out var fs) ? fs : 0;
            Money = currency.TryGetValue(VirtualCurrencyNames.MN.Code, out var MN) ? MN : 0;
            FriendPoint = currency.TryGetValue(VirtualCurrencyNames.FP.Code, out var fp) ? fp : 0;
        }

        /// <summary>
        /// スタミナを消費する
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async UniTask SubtractStaminaAsync(int amount)
        {
            // PlayFab 内部ではスタミナがユーザーごとのスタミナ最大値を超えていることがあるのでその分もここで考慮して減算する。
            int overMaxStaminaNum = StaminaRaw - UserDataManager.MaxStamina;
            if(overMaxStaminaNum>0)
            {
                amount = overMaxStaminaNum + amount;
            }

            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = VirtualCurrencyNames.ST.Code,
                Amount = amount
            };

            var response=await PlayFabClientAPI.SubtractUserVirtualCurrencyAsync(request);
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }

            StaminaRaw = response.Result.Balance;
        }

        /// <summary>
        /// スタミナを回復する。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async UniTask<bool>AddStaminaAsync(int amount)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = VirtualCurrencyNames.ST.Code,
                Amount = amount
            };

            var response = await PlayFabClientAPI.AddUserVirtualCurrencyAsync(request);
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
                return false;
            }

            StaminaRaw=response.Result.Balance;

            return true;
        }

        /// <summary>
        /// 経験値を増加する
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async UniTask<bool>AddExpAsync(int amount,bool login=true)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = VirtualCurrencyNames.EP.Code,
                Amount = amount,
            };

            var response = await PlayFabClientAPI.AddUserVirtualCurrencyAsync(request);
            if (response.Error!=null)
                throw new PlayFabErrorException(response.Error);

            Exp=response.Result.Balance;
            bool isLevelUp = Exp >= UserDataManager.NextLevelInfo.exp;
            if(isLevelUp)
            {
                await AddStaminaAsync(UserDataManager.NextLevelInfo.stamina);
            }

            if(login)
            {
                await LoginManager.LoginAndUpdateLocalCacheAsync();
            }

            return isLevelUp;
        }

        /// <summary>
        /// 石を消費する(実際はクライアントから変更できないので仮実装)
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async UniTask<bool>SubtractStoneAsync(int amount)
        {
            //　石があるかの確認
            if(amount>Stone)
            {
                return false;
            }

            // 有償石を消費するか
            bool isPaid = FreeStone < amount;

            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = isPaid ? VirtualCurrencyNames.PS.Code : VirtualCurrencyNames.FS.Code,
                Amount = amount
            };

            var response = await PlayFabClientAPI.SubtractUserVirtualCurrencyAsync(request);
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }

            if(isPaid)
            {
                PaidStone=response.Result.Balance;
            }
            else
            {
                FreeStone=response.Result.Balance;
            }

            return response.Error == null;
        }
    }
}