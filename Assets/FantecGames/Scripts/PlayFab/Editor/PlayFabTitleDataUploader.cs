#if UNITY_EDITOR

using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.AdminModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace fantec
{
    public class PlayFabTitleDataUploader
    {
        [MenuItem("Build/PlayFabData/AppendAssetVersion")]
        public static void AppendAssetVersionToPlayFab()
        {
            _ = _AppendAssetVersionToPlayFab();
        }

        private static async UniTaskVoid _AppendAssetVersionToPlayFab()
        {
            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            string currentVersion = PlayerSettings.bundleVersion;

            var getRequest = new GetTitleDataRequest();
            var getResult = await PlayFabAdminAPI.GetTitleDataAsync(getRequest);

            if(getResult.Error!=null)
            {
                Debug.LogError("TitleData取得失敗: "+getResult.Error.GenerateErrorReport());
                return;
            }

            var data=getResult.Result.Data ?? new Dictionary<string, string>();

            List<string>versions= new List<string>();

            if(data.TryGetValue("AssetVersions",out string existingValue))
            {
                versions= existingValue
                    .Split(new[] { ',' },System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .ToList();  
            }

            // すでに含まれていない場合のみ追加
            if(!versions.Contains(currentVersion))
            {
                versions.Add(currentVersion);
            }

            // セマンティック順にソート
            versions = versions
                .Select(v => new Version(v))
                .OrderBy(v => v)
                .Select(v => v.ToString())
                .ToList();


            string newValue =string.Join(",",versions);

            var setRequest = new SetTitleDataRequest
            {
                Key = "AssetVersions",
                Value = newValue
            };

            var setResult=await PlayFabAdminAPI.SetTitleDataAsync(setRequest);

            if(setResult.Error!=null)
            {
                Debug.LogError("TitleData更新失敗: " + setResult.Error.GenerateErrorReport());
                return;
            }

            Debug.Log($"AssetVersions にバージョン {currentVersion} を追加保存しました: {newValue}");
        }
    }
}
#endif