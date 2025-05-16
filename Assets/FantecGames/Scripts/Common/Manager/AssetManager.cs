using Cysharp.Threading.Tasks;
using fantec.Utilities;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace fantec.Common
{
    public class AssetManager:PersistentSingleton<AssetManager>
    {
        private Dictionary<string, Object> _loadedAssets = new();                // AudioClip以外
        private Dictionary<string, AudioClip> _loadedAudioClips = new();         // AudioClipのみ
        private Dictionary<string,GameObject>_loadedLive2DModels=new();          // Live2D専用

        /// <summary>
        /// 非同期でAddressablesアセットを読み込む
        /// </summary>
        public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cts = default) where T : Object
        {
            // AudioClipだけ別キャッシュを参照
            if (typeof(T) == typeof(AudioClip) && _loadedAudioClips.TryGetValue(address, out var audioObj))
            {
                return audioObj as T;
            }

            // Live2Dも別キャッシュを参照
            if(typeof(T) == typeof(GameObject)&&address.Contains("Live2D")&&_loadedLive2DModels.TryGetValue(address, out var live2DModel))
            {
                return live2DModel as T;
            }

            // 通常のキャッシュ参照
            if (_loadedAssets.TryGetValue(address, out Object cachedAsset) && cachedAsset is T cachedTyped)
            {
                return cachedTyped;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);

            try
            {
                await handle.ToUniTask(cancellationToken: cts);

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    try
                    {
                        var result = handle.Result;
                        if (result != null)
                        {
                            // AudioClipだけ別管理
                            if (result is AudioClip clip)
                            {
                                _loadedAudioClips[address] = clip;
                            }
                            else if(result is GameObject go&&address.Contains("Live2D"))
                            {
                                _loadedLive2DModels[address] = go;
                            }
                            else
                            {
                                _loadedAssets[address] = result;
                            }
                            return result;
                        }
                        else
                        {
                            Debug.LogError($"[AssetManager] Result is null even though status is Succeeded. Address: {address}");
                            return null;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[AssetManager] Exception while accessing handle.Result: {e}");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"[AssetManager] Failed to load: {address}, Status: {handle.Status}");
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AssetManager] Exception in load task for {address}: {ex}");
                return null;
            }
        }

        /// <summary>
        /// 読み込んだアセットを解放
        /// </summary>
        public void ReleaseAsset<T>(T asset) where T : Object
        {
            Addressables.Release(asset);
        }

        /// <summary>
        /// _loadedAssets に登録されているすべてのアセットを Release し、管理から除外する
        /// </summary>
        public void ReleaseAllLoadedAssets()
        {
            foreach (var kvp in _loadedAssets)
            {
                if (kvp.Value != null)
                {
                    Addressables.Release(kvp.Value);
                }
            }
            _loadedAssets.Clear();
        }

        /// <summary>
        /// AudioClipだけをリリース
        /// </summary>
        public void ReleaseAllAudioClips()
        {
            foreach (var kvp in _loadedAudioClips)
            {
                if (kvp.Value != null)
                {
                    Addressables.Release(kvp.Value);
                }
            }
            _loadedAudioClips.Clear();
        }

        /// <summary>
        /// Live2D関連だけをリリース
        /// </summary>
        public void ReleaseAllLive2DModels()
        {
            foreach(var kvp in _loadedLive2DModels)
            {
                if(kvp.Value != null)
                {
                    Addressables.Release(kvp.Value);
                }
            }
            _loadedLive2DModels.Clear();
        }

        public IReadOnlyDictionary<string, Object> GetLoadedAssets() => _loadedAssets;
        public IReadOnlyDictionary<string, AudioClip> GetLoadedAudioClips() => _loadedAudioClips;
        public IReadOnlyDictionary<string, GameObject> GetLoadedLive2DModels() => _loadedLive2DModels;

        public T LoadAsset<T>(string path) where T : Object
        {
            T obj = null;
            obj = Resources.Load<T>(path);
            if (obj == null) Debug.LogError(path);
            return obj;
        }
    }
}