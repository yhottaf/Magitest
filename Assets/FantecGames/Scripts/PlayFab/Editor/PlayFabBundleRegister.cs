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

namespace fantec
{
    public class PlayFabBundleRegister
    {
        static readonly string RewardStageDataFilePath = Application.dataPath + "/FantecGames/Master/Json/RewardStageMaster.json";
        static readonly string InitialRewardDataFilePath = Application.dataPath + "/FantecGames/Master/Json/InitialRewardStageMaster.json";

        [MenuItem("Build/PlayFabData/RewardStage/Upload Bundle", priority = 2)]
        static void RewardStageBundleRegister()
        {
            _ = _RewardStageBundleRegister();
        }

        static async UniTaskVoid _RewardStageBundleRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";
        
            var result=new PlayFabResult<UpdateCatalogItemsResult>();

            // JSON→バンドルデータへ変換
            StreamReader streamReader = new StreamReader(RewardStageDataFilePath);
            string dataStr=streamReader.ReadToEnd();
            var bundleDataList = JsonConvert.DeserializeObject<RewardStageData[]>(dataStr);
            streamReader.Close();

            List<string>bundledResultTables=new List<string>();

            foreach (var itemData in bundleDataList)
            {
                bundledResultTables.Clear();

                bundledResultTables.Add("RewardStage" + itemData.tableId);

                var request = new UpdateCatalogItemsRequest()
                {
                    Catalog = new List<CatalogItem>()
                    {
                        new CatalogItem()
                        {
                            Bundle = new CatalogItemBundleInfo()
                            {
                                BundledResultTables = bundledResultTables,
                            },
                            // 消費型の定義
                            Consumable = new CatalogItemConsumableInfo()
                            {
                                UsagePeriod = 5,
                            },
                            //スタック可能
                            IsStackable=false,
                            // トレード可能
                            IsTradable=false,
                            // アイテムクラス
                            ItemClass="Reward",
                            // アイテムID
                            ItemId="RewardStage"+itemData.tableId,
                            // タグ
                            Tags=new List<string>(){"Reward"},
                        }
                    },
                    CatalogVersion="Main",
                    SetAsDefaultCatalog=false,
                };
                Debug.Log("RewardStage"+itemData.tableId);
                result=await PlayFabAdminAPI.UpdateCatalogItemsAsync(request);
            }

            bundledResultTables.Clear();

            if (result.Error != null)
            {
                throw new PlayFabErrorException(result.Error);
            }
            else
            {
                Debug.Log("PlayFabへバンドルデータの登録が完了しました。");
            }
        }

        [MenuItem("Build/PlayFabData/InitialRewardStageMaster/Upload Bundle", priority = 2)]
        static void InitialRewardStageBundleRegister()
        {
            _ = _InitialRewardStageBundleRegister();
        }

        static async UniTaskVoid _InitialRewardStageBundleRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            var result = new PlayFabResult<UpdateCatalogItemsResult>();

            // JSON→バンドルデータへ変換
            StreamReader streamReader = new StreamReader(InitialRewardDataFilePath);
            string dataStr = streamReader.ReadToEnd();
            var bundleDataList = JsonConvert.DeserializeObject<InitialRewardStageData[]>(dataStr);
            streamReader.Close();

            List<string >bundledResultTables=new List<string>();

            foreach(var itemData in bundleDataList)
            {
                bundledResultTables.Clear();

                // アイテムの登録
                if(itemData.itemId1.Equals("-1")==false)
                {
                    for(int i=0;i<itemData.quantity1;i++)
                    {
                        bundledResultTables.Add("Item" + itemData.itemId1);
                    }
                }
                if (itemData.itemId2.Equals("-1") == false)
                {
                    for (int i = 0; i < itemData.quantity2; i++)
                    {
                        bundledResultTables.Add("Item" + itemData.itemId2);
                    }
                }
                if (itemData.itemId3.Equals("-1") == false)
                {
                    for (int i = 0; i < itemData.quantity3; i++)
                    {
                        bundledResultTables.Add("Item" + itemData.itemId3);
                    }
                }
                // Magi(ユニット)の登録
                for(int i=0;i<itemData.cardId.Length;i++)
                {
                    bundledResultTables.Add(itemData.cardId[i].ToString());
                }

                // 無償石の登録
                var virtualCurrency = new Dictionary<string, uint>();
                if(itemData.VC>0)
                {
                    virtualCurrency.Add("FS", (uint)itemData.VC);
                }

                var request = new UpdateCatalogItemsRequest()
                {
                    Catalog = new List<CatalogItem>() { new CatalogItem()
                    {
                        Bundle=new CatalogItemBundleInfo()
                        {
                            BundledItems=bundledResultTables,
                            BundledVirtualCurrencies=virtualCurrency,
                        },
                        // 消費型の定義
                        Consumable=new CatalogItemConsumableInfo()
                        {
                            UsagePeriod=5,
                        },
                        // スタック可能
                        IsStackable=true,
                        // トレード可能
                        IsTradable=false,
                        // アイテムクラス
                        ItemClass="Card",
                        // アイテムID
                        ItemId=$"{itemData.cardId}",
                        // タグ
                        Tags=new List<string>() { "Card"},
                    } },
                    CatalogVersion="Main",
                    SetAsDefaultCatalog=false,
                };

                result=await PlayFabAdminAPI.UpdateCatalogItemsAsync(request);

                if(result.Error != null)
                {
                    throw new PlayFabErrorException(result.Error);
                }
            }

            bundledResultTables.Clear();

            Debug.Log("PlayFabへバンドルデータの登録が完了しました。");
        }
    }
}
#endif