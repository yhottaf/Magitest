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
using System;

namespace fantec
{
    public class PlayFabItemRegister
    {
        static readonly string ItemDataJsonPath = Application.dataPath + "/FantecGames/Master/Json/ConsumeItemMaster.json";
        static readonly string InitialRewardDataFilePath = Application.dataPath + "/FantecGames/Master/Json/InitialRewardStageMaster.json";
        [MenuItem("Build/PlayFabData/ConsumeMaster")]
        static void ItemRegister()
        {
            _ = _ItemRegister();
        }

        static async UniTaskVoid _ItemRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            var result = new PlayFabResult<UpdateCatalogItemsResult>();

            // JSON→アイテムリストへ変換
            StreamReader streamReader = new StreamReader(ItemDataJsonPath);
            string dataStr = streamReader.ReadToEnd();
            var itemDataList = JsonConvert.DeserializeObject<ConsumeItemData[]>(dataStr);
            streamReader.Close();

            foreach (var itemData in itemDataList)
            {
                var request = new UpdateCatalogItemsRequest()
                {
                    Catalog = new List<CatalogItem>() { new CatalogItem()
                    {
                        // 消費型の定義
                        Consumable = new CatalogItemConsumableInfo()
                        {
                            UsageCount = 1,
                        },
                        // 説明
                        Description = itemData.description,
                        // 表示名
                        DisplayName = itemData.name,
                        // 限定商品として販売する
                        IsLimitedEdition = false,
                        // 最大所持数
                        InitialLimitedEditionCount = itemData.maxItem, 
                        // スタック可能
                        IsStackable = true,
                        // トレード可能
                        IsTradable = false,
                        // アイテムクラス
                        ItemClass = "Item",
                        // アイテムID
                        ItemId = $"Item{itemData.itemId}",
                        // アイテムイメージURL
                        ItemImageUrl = itemData.imageName,
                        // タグ
                        Tags = new List<string>() { "Item" },
                    } },
                    CatalogVersion = "Main",
                    SetAsDefaultCatalog = false,
                };

                Debug.Log($"アイテム：{request.Catalog[0].DisplayName}");

                result = await PlayFabAdminAPI.UpdateCatalogItemsAsync(request);

                if (result.Error != null)
                {
                    throw new PlayFabErrorException(result.Error);
                }
            }

            Debug.Log("PlayFabへアイテムデータの登録が完了しました。");
        }

        [MenuItem("Build/PlayFabData/InitialRewardStageMaster/Upload RewardData", priority = 1)]
        static void InitialRewardStoreRegister()
        {
            _ = _InitialRewardStore();
        }

        static async UniTaskVoid _InitialRewardStore()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            var result = new PlayFabResult<UpdateCatalogItemsResult>();

            // JSON→アイテムリストへ変換
            StreamReader streamReader = new StreamReader(InitialRewardDataFilePath);
            string dataStr = streamReader.ReadToEnd();
            var itemDataList = JsonConvert.DeserializeObject<InitialRewardStageData[]>(dataStr);
            streamReader.Close();

            foreach (var itemData in itemDataList)
            {
                var request = new UpdateCatalogItemsRequest()
                {
                    Catalog = new List<CatalogItem>() { new CatalogItem()
                    {
                        // 消費型の定義
                        Consumable = new CatalogItemConsumableInfo()
                        {
                               UsagePeriod = 5,
                        },
                        // 説明
                        Description = "",
                        // 表示名
                        DisplayName = "",
                        // 限定商品として販売する
                        IsLimitedEdition = false,
                        // 最大所持数
                        InitialLimitedEditionCount = 0, 
                        // スタック可能
                        IsStackable = false,
                        // トレード可能
                        IsTradable = false,
                        // アイテムクラス
                        ItemClass = "Reward",
                        // アイテムID
                        ItemId = $"InitialReward{itemData.tableId}",
                        // アイテムイメージURL
                        ItemImageUrl = "",
                        // タグ
                        Tags = new List<string>() { "Reward" },
                    } },
                    CatalogVersion = "Main",
                    SetAsDefaultCatalog = false,
                };

                Debug.Log($"InitialReward{itemData.tableId}");

                result = await PlayFabAdminAPI.UpdateCatalogItemsAsync(request);

                if (result.Error != null)
                {
                    throw new PlayFabErrorException(result.Error);
                }
            }

            Debug.Log("PlayFabへアイテムデータの登録が完了しました。");
        }
    }
}
#endif