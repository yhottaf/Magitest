#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using fantec.Master;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.AdminModels;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace fantec
{
    public class PlayFabCardRegister
    {
        static readonly string ItemDataJsonPath = Application.dataPath + "/FantecGames/Master/Json/PlayerCardMaster.json";

        [MenuItem("Build/PlayFabData/CardMaster")]
        static void CardRegister()
        {
            _ = _CardRegister();
        }

        static async UniTaskVoid _CardRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            // JSON→アイテムリストへ変換
            StreamReader streamReader = new StreamReader(ItemDataJsonPath);
            string dataStr=streamReader.ReadToEnd();
            var itemDataList = JsonConvert.DeserializeObject<PlayerCardData[]>(dataStr);
            streamReader.Close();

            var catalogItemList = new List<CatalogItem>();
            foreach(var itemData in itemDataList)
            {
                catalogItemList.Add(new CatalogItem()
                {
                    // 表示名
                    DisplayName=$"{itemData.charaName}",
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
                });
            }

            var request = new UpdateCatalogItemsRequest()
            {
                Catalog = catalogItemList,
                CatalogVersion = "Main",
                SetAsDefaultCatalog = false,
            };
            var result =await PlayFabAdminAPI.UpdateCatalogItemsAsync(request);

            if(result.Error!=null)
            {
                throw new PlayFabErrorException(result.Error);
            }

            Debug.Log("PlayFabへカードデータの登録が完了しました。");
        }
    }
}
#endif