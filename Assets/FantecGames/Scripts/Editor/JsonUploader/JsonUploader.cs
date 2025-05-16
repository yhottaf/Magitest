#if UNITY_EDITOR && ENABLE_PLAYFABADMIN_API && !DISABLE_PLAYFAB_STATIC_API && ENABLE_PLAYFABSERVER_API

using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.AdminModels;

namespace fantec
{
    public class JsonUploader
    {
        private const string MASTER_JSON_FOLDER_PATH = "/FantecGames/Master/Json/";
        static readonly string JsonFolderPath = Application.dataPath + MASTER_JSON_FOLDER_PATH;

        static CancellationTokenSource CancellationTokenSource;


        [MenuItem("Build/Master/UploadJson/Start", priority = 1)]
        static void UploadJson()
        {
            _ = Upload(JsonFolderPath);
        }

        static async UniTaskVoid Upload(string folderPath)
        {
            CancellationTokenSource = new CancellationTokenSource();

            PlayFabSettings.staticSettings.TitleId = "E23B7";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "KXYWIP4D4FG5SOTJ8WCJW1FX8UBK3H69FYWP5UICQG8F3SQ8RC";

            foreach (var data in GetKeyValue(folderPath))
            {
                if (CancellationTokenSource.IsCancellationRequested) return;
                var resuest = new SetTitleDataRequest()
                {
                    Key = data.Item1,
                    Value = data.Item2,
                };
                await PlayFabAdminAPI.SetTitleDataAsync(resuest, CancellationTokenSource);
                Debug.Log($"Uploaded / FileName : {data.Item1} json : {data.Item2}");
            }

            // å„énññ
            CancellationTokenSource.Dispose();
            CancellationTokenSource = null;


            Debug.Log("Upload completed.");
        }

        static (string, string)[] GetKeyValue(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories);
            var result = new (string, string)[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                StreamReader sr = new StreamReader(files[i], System.Text.Encoding.UTF8);
                result[i].Item1 = Path.GetFileNameWithoutExtension(files[i]);
                result[i].Item2 = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }



        [MenuItem("Build/Master/UploadJson/Cancel", priority = 2)]
        static void Cancel()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
                Debug.Log("Cancel completed.");
            }
        }
    }
}

#endif