using Cysharp.Threading.Tasks;
using fantec.Menu.Notice;
using fantec.PlayfabCilent;
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



        public static async UniTask<NoticeResult> ClaimNoticeRewardAsync(string noticeKey)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "ClaimNoticeReward",
                FunctionParameter = new { key = noticeKey },
                GeneratePlayStreamEvent = true
            };

            var response = await PlayFabClientAPI.ExecuteCloudScriptAsync(request);
            if (response.Error != null)
            {
                Debug.LogError("報酬受け取り失敗: " + response.Error.GenerateErrorReport());
                return null;
            }

            string json = response.Result.FunctionResult.ToString();
            Debug.Log("CloudScriptの返却結果: " + json);

            // JSONをUnityのクラスに変換
            return JsonUtility.FromJson<NoticeResult>(json);
        }
    }
}