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
    public class PlayFabDropTableRegister
    {
        static readonly string RewardStageDataFilePath = Application.dataPath + "/FantecGames/Master/Json/RewardStageMaster.json";


        [MenuItem("Build/PlayFabData/RewardStage/Upload DropTable", priority = 1)]
        static void RewardStageTableRegister()
        {
            _ = _RewardStageTableRegister();
        }

        static async UniTaskVoid _RewardStageTableRegister()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            // JSON→アイテムリストへ変換
            StreamReader streamReader = new StreamReader(RewardStageDataFilePath);
            string dataStr = streamReader.ReadToEnd();
            var tableDataList = JsonConvert.DeserializeObject<RewardStageData[]>(dataStr);
            streamReader.Close();

            var tableList = new List<RandomResultTable>();

            foreach (var tableData in tableDataList)
            {
                if (string.IsNullOrEmpty(tableData.tableId))
                {
                    continue;
                }

                var nodeList = new List<ResultTableNode>();
                if (tableData.itemId1.Equals("-1") == false && string.IsNullOrEmpty(tableData.itemId1) == false)
                {
                    nodeList.Add(new ResultTableNode()
                    {
                        ResultItem = $"Item{tableData.itemId1}",
                        Weight = tableData.weight1,
                    });
                }
                if (tableData.itemId2.Equals("-1") == false && string.IsNullOrEmpty(tableData.itemId2) == false)
                {
                    nodeList.Add(new ResultTableNode()
                    {
                        ResultItem = $"Item{tableData.itemId2}",
                        Weight = tableData.weight2,
                    });
                }
                if (tableData.itemId3.Equals("-1") == false && string.IsNullOrEmpty(tableData.itemId3) == false)
                {
                    nodeList.Add(new ResultTableNode()
                    {
                        ResultItem = $"Item{tableData.itemId3}",
                        Weight = tableData.weight3,
                    });
                }

                if (nodeList.Count > 0)
                {
                    tableList.Add(new RandomResultTable()
                    {
                        Nodes = nodeList,
                        TableId = $"RewardStage{tableData.tableId}",
                    });

                    Debug.Log(tableData.tableId);
                }
            }

            var request = new UpdateRandomResultTablesRequest()
            {
                Tables = tableList,
                CatalogVersion = "Main",
            };

            var result = await PlayFabAdminAPI.UpdateRandomResultTablesAsync(request);

            if (result.Error != null)
            {
                throw new PlayFabErrorException(result.Error);
            }


            Debug.Log("PlayFabへデータの登録が完了しました。");
        }

   
    }
}

#endif