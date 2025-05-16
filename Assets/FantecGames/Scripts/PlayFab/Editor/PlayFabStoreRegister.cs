#if UNITY_EDITOR

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.AdminModels;
using fantec.Master;

namespace LeftRpg
{
    public class PlayFabStoreRegister
    {
        static readonly string RewardStageDataFilePath = Application.dataPath + "/FantecGames/Master/Json/RewardStageMaster.json";
        static readonly string InitialRewardDataFilePath = Application.dataPath + "/FantecGames/Master/Json/InitialRewardStageMaster.json";

     

        [MenuItem("Build/PlayFabData/RewardStage/Upload Store", priority = 3)]
        static void RewardStageStoreRegister()
        {
            _ = _RewardStageStoreRegister();
        }

        static async UniTaskVoid _RewardStageStoreRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            var result = new PlayFabResult<UpdateStoreItemsResult>();

            // JSON→アイテムリストへ変換
            StreamReader streamReader = new StreamReader(RewardStageDataFilePath);
            string dataStr = streamReader.ReadToEnd();
            var gachaDataList = JsonConvert.DeserializeObject<RewardStageData[]>(dataStr);
            streamReader.Close();

            List<StoreItem> storeItemList = new List<StoreItem>();

            foreach (var itemData in gachaDataList)
            {
                if (string.IsNullOrEmpty(itemData.tableId) == false)
                {
                    storeItemList.Add(new StoreItem()
                    {
                        ItemId = "RewardStage" + itemData.tableId,
                        VirtualCurrencyPrices = new Dictionary<string, uint>() { { "FS", 0 } },
                    });
                }
            }

            var request = new UpdateStoreItemsRequest()
            {
                CatalogVersion = "Main",

                MarketingData = new StoreMarketingModel()
                {
                    Description = "",
                    DisplayName = ""
                },

                Store = storeItemList,

                StoreId = "DummyStore",
            };

            result = await PlayFabAdminAPI.UpdateStoreItemsAsync(request);

            if (result.Error != null)
            {
                throw new PlayFabErrorException(result.Error);
            }

            Debug.Log("PlayFabへバンドルデータの登録が完了しました。");
        }

     
        [MenuItem("Build/PlayFabData/InitialRewardStageMaster/Upload Store", priority = 2)]
        static void InitialRewardStoreRegister()
        {
            _ = _InitialRewardStoreRegister();
        }

        static async UniTaskVoid _InitialRewardStoreRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            var result = new PlayFabResult<UpdateStoreItemsResult>();

            // JSON→アイテムリストへ変換
            StreamReader streamReader = new StreamReader(InitialRewardDataFilePath);
            string dataStr = streamReader.ReadToEnd();
            var dataList = JsonConvert.DeserializeObject<InitialRewardStageData[]>(dataStr);
            streamReader.Close();

            List<StoreItem> storeItemList = new List<StoreItem>();

            foreach (var itemData in dataList)
            {
                if (string.IsNullOrEmpty(itemData.tableId) == false)
                {
                    storeItemList.Add(new StoreItem()
                    {
                        ItemId = "InitialReward" + itemData.tableId,
                        VirtualCurrencyPrices = new Dictionary<string, uint>() { { "FS", 0 } },
                    });

                    Debug.Log(itemData.tableId);
                }
            }

            var request = new UpdateStoreItemsRequest()
            {
                CatalogVersion = "Main",

                MarketingData = new StoreMarketingModel()
                {
                    Description = "",
                    DisplayName = ""
                },

                Store = storeItemList,

                StoreId = "DummyStore",
            };

            result = await PlayFabAdminAPI.UpdateStoreItemsAsync(request);

            if (result.Error != null)
            {
                throw new PlayFabErrorException(result.Error);
            }

            Debug.Log("PlayFabへバンドルデータの登録が完了しました。");
        }
    }
}
#endif