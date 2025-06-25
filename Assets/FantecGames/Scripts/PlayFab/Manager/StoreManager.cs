using Cysharp.Threading.Tasks;
using fantec.PlayfabCilent;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fantec.PlayFabClient
{
    public static class StoreManager
    {
        public static Dictionary<StoreId, Dictionary<string, StoreItem>> StoreItems { get; private set; }
        = new Dictionary<StoreId, Dictionary<string, StoreItem>>
        {
            { StoreId.MainStore,null },
            { StoreId.GachaStore,null },
            { StoreId.DummyStore,null }
        };

        /// <summary>
        /// PlayFabから最新のデータを取得してローカルにキャッシュする。
        /// </summary>
        /// <returns></returns>
        public static async UniTask SyncPlayFabToClientAsync()
        {
            var (mainStoreResponse, gachaStoreResponse, dummyStoreResponse) = await UniTask.WhenAll(
                PlayFabClientAPI.GetStoreItemsAsync(new GetStoreItemsRequest { StoreId=StoreId.MainStore.ToString()}).AsUniTask(),
                PlayFabClientAPI.GetStoreItemsAsync(new GetStoreItemsRequest { StoreId=StoreId.GachaStore.ToString()}).AsUniTask(),
                PlayFabClientAPI.GetStoreItemsAsync(new GetStoreItemsRequest { StoreId=StoreId.DummyStore.ToString()}).AsUniTask()
                );

            if(mainStoreResponse.Error!=null)
            {
                throw new PlayFabErrorException(mainStoreResponse.Error);
            }

            if(gachaStoreResponse.Error!=null)
            {
                throw new PlayFabErrorException(gachaStoreResponse.Error);
            }

            if(dummyStoreResponse.Error!=null)
            {
                throw new PlayFabErrorException(dummyStoreResponse.Error);
            }

            StoreItems[StoreId.MainStore] = mainStoreResponse.Result.Store.OrderBy(x => x.DisplayPosition).ToDictionary(y=>y.ItemId);
            StoreItems[StoreId.GachaStore] = gachaStoreResponse.Result.Store.OrderBy(x => x.DisplayPosition).ToDictionary(y=>y.ItemId);
            StoreItems[StoreId.DummyStore] = dummyStoreResponse.Result.Store.OrderBy(x => x.DisplayPosition).ToDictionary(y=>y.ItemId);
        }

        /// <summary>
        /// ストアで商品を購入する
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="itemId"></param>
        /// <param name="vc"></param>
        /// <returns></returns>

        public static async UniTask<PurchaseItemResult> PurchaseItemAsync(StoreId storeId,string itemId,string vc,bool login=true)
        {
            var item = StoreItems[storeId][itemId];
            var price = (int)item.VirtualCurrencyPrices[vc];
            var request = new PurchaseItemRequest { StoreId = storeId.ToString(), ItemId = itemId, VirtualCurrency = vc, Price = price };

            var response =await PlayFabClientAPI.PurchaseItemAsync(request);
            if(response.Error!=null)
            {
                throw new PlayFabErrorException(response.Error);
            }

            if(login)
            {
                await LoginManager.LoginAndUpdateLocalCacheAsync();
            }

            return response.Result;
        }


        /// <summary>
        /// 引数から該当するお知らせを読み、それに対応した報酬をプレイヤーに渡す
        /// </summary>
        /// <param name="noticeKey"></param>
        /// <returns></returns>
        public static async UniTask ClaimNoticeRewardAsync(string noticeKey)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "ClaimNoticeReward",
                FunctionParameter = new Dictionary<string, object>
                {
                   { "key", noticeKey }
                },
                GeneratePlayStreamEvent = true
            };

            var response = await PlayFabClientAPI.ExecuteCloudScriptAsync(request);
            if (response.Error != null)
            {
                Debug.LogError("報酬受け取り失敗: " + response.Error.GenerateErrorReport());
                return;
            }

            string rawJson = response.Result.FunctionResult?.ToString();
            Debug.Log("CloudScript Result Raw JSON: " + rawJson);
            var functionResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
            if (functionResult==null)
            {
                Debug.LogWarning("CloudScriptの結果が不正");
                return;
            }

            string status = functionResult["status"]as string;
            if(status=="granted"&&functionResult.TryGetValue("claimedKey", out var claimedKeyObj))
            {
                string claimedkey = claimedKeyObj as string;

                // ローカルUserに反映 (Dictionaryに記録して保存)
                if(!UserDataManager.User.ClaimedNoticeDictionary.ContainsKey(claimedkey))
                {
                    UserDataManager.User.ClaimedNoticeDictionary[claimedkey] = true;
                    await UserDataManager.UpdatePlayFab(); // JSONで保存
                }
            }
            else if (status=="already_claimed")
            {
                Debug.Log("すでに報酬は受け取り済みです");
                UserDataManager.User.ClaimedNoticeDictionary[noticeKey] = true;
                await UserDataManager.UpdatePlayFab(); // JSONで保存
            }
        }
    }
}