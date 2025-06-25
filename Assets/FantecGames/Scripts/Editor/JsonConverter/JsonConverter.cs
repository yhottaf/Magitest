#if UNITY_EDITOR
using UnityEngine;
using fantec.Master;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


namespace fantec
{
    public class JsonConverter
    {
        private const string MASTER_LOCAL_FOLDER_PATH = "Assets/FantecGames/Master/Local/";
        private const string MASTER_JSON_FOLDER_PATH = "/FantecGames/Master/Json/";
        static readonly string JsonFolderPath = Application.dataPath + MASTER_JSON_FOLDER_PATH;

        [MenuItem("Build/Master/CreateJson")]
        static void CreateJson()
        {
            // TODO: アルファベット順で並べる
            // TODO: マスターデータを作るたびにJson化を忘れない！！！
            MasterToJson<ConsumeItemMaster, ConsumeItemData>();
            MasterToJson<OverrideSkillMaster,OverrideSkillData>();
            MasterToJson<EnemyCardMaster, EnemyCardData>();
            MasterToJson<InitialRewardStageMaster, InitialRewardStageData>();
            MasterToJson<NoticeMaster,NoticeData>();
            MasterToJson<OtherSkillMaster, OtherSkillData>();
            MasterToJson<PlayerCardMaster, PlayerCardData>();
            MasterToJson<RewardStageMaster,RewardStageData>();
            MasterToJson<StageMaster, StageData>();

            // 出力先のフォルダをちゃんと作っているか確認しておく
            MasterToJson<ExpMaster, ExpData>("Exp");
            MasterToJson<GrowthMaster, GrowthData>("Growth");
            MasterToJson<WaveMaster, WaveData>("Wave");
        }

        static void MasterToJson<TMaster, TData>()
           where TMaster : MasterBase<TData>
           where TData : IData
        {
            var masterName = typeof(TMaster).Name;
            var assetPath = $"{MASTER_LOCAL_FOLDER_PATH}{masterName}.asset";
            var master = (TMaster)AssetDatabase.LoadAssetAtPath(assetPath, typeof(TMaster));
            if (master != null)
            {
                MasterToJson<TMaster, TData>(master);
            }
            else
            {
                Debug.LogError($"{assetPath} という名の ScriptableObject が見つかりません。");
            }
        }

        static void MasterToJson<TMaster, TData>(string additionalDirectory)
            where TMaster : MasterBase<TData>
            where TData : IData
        {
            var assetFolderPath = MASTER_LOCAL_FOLDER_PATH + additionalDirectory;
            var masters = FindAllAsset<TMaster>(assetFolderPath);
            if (masters.Count > 0)
            {
                foreach (var master in masters)
                {
                    MasterToJson<TMaster, TData>(master, additionalDirectory);
                }
            }
            else
            {
                Debug.LogError($"{assetFolderPath} 内に対象の ScriptableObject が見つかりません。");
            }
        }

        static void MasterToJson<TMaster, TData>(MasterBase<TData> master, string additionalDirectory = "")
            where TMaster : MasterBase<TData>
            where TData : IData
        {
            var masterName = master.name;
            var jsonString = JsonHelper.ToJson<TData>(master.dataList);
            var jsonFileName = $"{masterName}.json";
            var jsonFilePath = Path.Combine(JsonFolderPath + additionalDirectory, jsonFileName);
            File.WriteAllText(jsonFilePath, jsonString);
            Debug.Log($"Converted {masterName} {jsonFilePath}");
        }

        static IReadOnlyList<T> FindAllAsset<T>(string directoryPath) where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            var fileNames = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            foreach (var fileName in fileNames)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(fileName);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
    }
}

#endif