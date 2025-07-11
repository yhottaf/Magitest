using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using fantec.Extensions;

namespace fantec
{

    //AssetBundleの情報管理クラス
    [AddComponentMenu("fantec/Lib/File/AssetBundleInfoManager")]
    public class AssetBundleInfoManager : MonoBehaviour
    {
        //ロード失敗したときのリトライ回数
        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }
        [SerializeField]
        int retryCount = 5;

        //ロードのタイムアウト時間
        public int TimeOut
        {
            get { return retryCount; }
            set { retryCount = value; }
        }
        [SerializeField]
        float timeOut = 5;

        //DLしたマニフェストをキャッシュ書き込みする
        public bool UseCacheManifest
        {
            get { return useCacheManifest; }
            set { useCacheManifest = value; }
        }
        [SerializeField]
        bool useCacheManifest = true;

        //DLしたマニフェストを書き込むフォルダ名
        public string CacheDirectoryName
        {
            get { return cacheDirectoryName; }
            set { cacheDirectoryName = value; }
        }
        [SerializeField]
        string cacheDirectoryName = "Cache";

        //アセットバンドルをキャッシュからロードする
        public bool CacheLoad
        {
            get { return cacheLoad; }
            set { cacheLoad = value; }
        }
        [SerializeField]
        bool cacheLoad = true;


        AssetFileManager AssetFileManager { get { return this.GetComponentCache<AssetFileManager>(ref assetFileManager); } }
        [SerializeField]
        AssetFileManager assetFileManager;

        FileIOManager FileIOManager { get { return AssetFileManager.FileIOManager; } }

        //大文字と小文字を無視するDictionary
        Dictionary<string, AssetBundleInfo> dictionary = new Dictionary<string, AssetBundleInfo>(StringComparer.OrdinalIgnoreCase);

        //アセットバンドルのマニフェスト名
        const string AssetBundleManifestName = "assetbundlemanifest";

        //アセットバンドルの情報を取得
        public AssetBundleInfo FindAssetBundleInfo(string path)
        {
            //ファイル情報を取得or作成
            AssetBundleInfo info;
            //大文字と小文字を無視するDictionaryでアセットバンドルの小文字化に対応している
            if (!dictionary.TryGetValue(path, out info))
            {
                string key = FilePathUtil.ChangeExtension(path, ".asset");
                if (!dictionary.TryGetValue(key, out info))
                {
                    return null;
                }
            }
            return info;
        }

        //アセットバンドルの情報を追加(カスタムしたアセットバンドルの情報を設定する場合はここを使う)
        public void AddAssetBundleInfo(string resourcePath, string assetBunleUrl, int assetBunleVersion, int assetBunleSize = 0)
        {
            AddAssetBundleInfo(resourcePath, new AssetBundleInfo(assetBunleUrl, assetBunleVersion, assetBunleSize));
        }

        //アセットバンドルの情報を追加(カスタムしたアセットバンドルの情報を設定する場合はここを使う)
        public void AddAssetBundleInfo(string resourcePath, string assetBunleUrl, Hash128 assetBunleHash, int assetBunleSize = 0)
        {
            AddAssetBundleInfo(resourcePath, new AssetBundleInfo(assetBunleUrl, assetBunleHash, assetBunleSize));
        }

        //アセットバンドルの情報を追加(キャッシュを使わない場合)
        public void AddAssetBundleInfo(string resourcePath, string assetBundleUrl)
        {
            AddAssetBundleInfo(resourcePath, new AssetBundleInfo(assetBundleUrl));
        }

        //アセットバンドルマニフェストの情報を追加
        public void AddAssetBundleManifest(string rootUrl, AssetBundleManifest manifest)
        {
            foreach (string name in manifest.GetAllAssetBundles())
            {
                string path = FilePathUtil.Combine(rootUrl, name);
                if (CacheLoad)
                {
                    Hash128 assetBundleHash = manifest.GetAssetBundleHash(name);
                    AddAssetBundleInfo(path, new AssetBundleInfo(path, assetBundleHash));
                }
                else
                {
                    AddAssetBundleInfo(path, new AssetBundleInfo(path));
                }
            }
        }

        void AddAssetBundleInfo(string key, AssetBundleInfo info)
        {
            try
            {
                dictionary.Add(key, info);
            }
            catch
            {
                Debug.LogError(key + "is already contains in AssetBundleManger");
            }
        }

        //キャッシュのパスを取得
        string GetCachePath(string relativeUrl)
        {
            string path = FilePathUtil.Combine(FileIOManager.SdkTemporaryCachePath, cacheDirectoryName, relativeUrl);
            return path;
        }

        //キャッシュすべて削除
        public void DeleteAllCache()
        {
            FileIOManager.DeleteDirectory(FilePathUtil.Combine(FileIOManager.SdkTemporaryCachePath, cacheDirectoryName) + "/");
            WrapperUnityVersion.CleanCache();
        }
    }
}