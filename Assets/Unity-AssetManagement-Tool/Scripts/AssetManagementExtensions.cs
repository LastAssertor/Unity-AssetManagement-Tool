using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;
using Object = UnityEngine.Object;

using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace UnityAssetManagementTool {

    /// <summary>
    /// An asset management system for legacy version unity
    /// 
    /// Require:
    /// Unity Version : 5.6.7f1
    /// Package : AssetBundle Browser (https://github.com/Unity-Technologies/AssetBundles-Browser)
    /// </summary>

    public static class AssetManagementExtensions {

        public static Coroutine LoadAssetsAsync<T>(this List<string> assetPaths, Dictionary<string, AssetRefer> loadedAssets,
            Dictionary<string, AssetBundleRefer> loadedAssetBundles, AssetBundleManifest manifest,
            Action<List<T>> completed, bool async = true) where T : Object {
            var loadingTasks = assetPaths.SpawnLoadingTasks(manifest);
            return LoadAssetsAsync<T>(loadingTasks, loadedAssets, loadedAssetBundles, completed, async).StartCoroutine();
        }

        public static IEnumerator LoadAssetsAsync<T>(this List<LoadingTask> loadingTasks, Dictionary<string, AssetRefer> loadedAssets,
            Dictionary<string, AssetBundleRefer> loadedAssetBundles, Action<List<T>> completed, bool async = true) where T : Object {

            for (int i = 0, len = loadingTasks.Count; i < len; i++) {

                var task = loadingTasks[i];

                switch (task.TaskType) {
                    case LoadingTaskType.LoadingSceneFromAssetBundle:
                    case LoadingTaskType.LoadingScene: {

                            // load scene.

                            var scenceName = task.ScenceName;

                            if (async) {
                                var asyncOperation = SceneManager.LoadSceneAsync(scenceName);

                                yield return asyncOperation;

                                asyncOperation.allowSceneActivation = true;

                            } else {

                                SceneManager.LoadScene(scenceName);
                            }

                        }
                        break;
                    case LoadingTaskType.LoadingAssetFromResources: {

                            var assetPath = task.AssetPath;

                            if (loadedAssets.ContainsKey(assetPath)) {

                                // asset is loaded, do nothing.

                            } else {

                                var resourcesPath = task.ResourcesPath;

                                Object asset;
                                if (async) {

                                    var request = Resources.LoadAsync<T>(resourcesPath);

                                    while (!request.isDone) {
                                        yield return null;
                                    }

                                    asset = request.asset;

                                } else {

                                    asset = Resources.Load<T>(resourcesPath);
                                }

                                if (asset == null) {

                                    Debug.LogWarning("load asset from resources failed, resources path = " + resourcesPath);

                                } else {

                                    // load asset from resources ok.

                                    loadedAssets.Add(assetPath, AssetRefer.Spawn(asset));
                                }

                            }

                        }
                        break;
                    case LoadingTaskType.LoadingAssetFromAssetBundle: {

                            var assetPath = task.AssetPath;

                            if (loadedAssets.ContainsKey(assetPath)) {

                                // asset is loaded, do nothing.

                            } else {

                                var assetBundleName = task.AssetBundleName;

                                AssetBundleRefer assetBundleRefer;
                                if (loadedAssetBundles.TryGetValue(assetBundleName, out assetBundleRefer)) {

                                    Object asset;
                                    if (async) {

                                        var request = assetBundleRefer.Target.LoadAssetAsync<T>(assetPath);

                                        while (!request.isDone) {
                                            yield return null;
                                        }

                                        asset = request.asset;

                                    } else {

                                        asset = assetBundleRefer.Target.LoadAsset<T>(assetPath);
                                    }

                                    if (asset == null) {

                                        Debug.LogWarning("load asset from assetbundle failed, asset path = " + assetPath);

                                    } else {

                                        // load asset from assetbundle ok.

                                        loadedAssets.Add(assetPath, AssetRefer.Spawn(asset));
                                    }


                                } else {

                                    Debug.LogWarning("not found loaded assetbundle, assetbundle name = " + assetBundleName);
                                }
                            }

                        }
                        break;
                    case LoadingTaskType.LoadingAssetBundle: {

                            var assetBundleName = task.AssetBundleName;

                            if (loadedAssetBundles.ContainsKey(assetBundleName)) {

                                // assetbundle is loaded, do nothing.

                            } else {

                                // caculate assetbundle file path

                                var persistentDataPath = assetBundleName.GetAssetBundlePath();

                                if (File.Exists(persistentDataPath)) {

                                    AssetBundle assetBundle;
                                    if (async) {

                                        var request = AssetBundle.LoadFromFileAsync(persistentDataPath);
                                        while (!request.isDone) {
                                            yield return null;
                                        }

                                        assetBundle = request.assetBundle;

                                    } else {

                                        assetBundle = AssetBundle.LoadFromFile(persistentDataPath);
                                    }

                                    if (assetBundle == null) {

                                        Debug.LogWarning("load assetbundle from file faile, file path = " + persistentDataPath);

                                    } else {

                                        // load assetbundle from file ok.
                                        loadedAssetBundles.Add(assetBundleName, AssetBundleRefer.Spawn(assetBundle));
                                    }

                                } else {

                                    if (async) {

                                        var streamingAssetsPath = assetBundleName.GetAssetBundlePath(false);

#if UNITY_2018_4_OR_NEWER
                                        UnityWebRequest unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(streamingAssetsPath);
                                        yield return unityWebRequest.SendWebRequest();

                                        var error = unityWebRequest.error;

                                        if (string.IsNullOrEmpty(error)) {

                                            var download = unityWebRequest.downloadHandler as DownloadHandlerAssetBundle;
                                            if (download != null) {

                                                var assetBundle = download.assetBundle;

                                                if (assetBundle == null) {

                                                    Debug.LogWarning("load assetbundle from streamingAssetPath failed, url = " + streamingAssetsPath);

                                                } else {

                                                    loadedAssetBundles.Add(assetBundleName, AssetBundleRefer.Spawn(assetBundle));
                                                }

                                            } else {

                                                Debug.LogWarning("download assetbundle from streamingAssetPath failed, url = " + streamingAssetsPath);


                                            }


                                        } else {
                                            Debug.LogWarning("load assetbundle failed, UnityWebRequest.error = " + error + ", url = " + streamingAssetsPath);
                                        }
#else
                                        using (var www = new WWW(streamingAssetsPath)) {

                                            while (!www.isDone) {
                                                yield return null;
                                            }

                                            var error = www.error;

                                            if (string.IsNullOrEmpty(error)) {

                                                var assetBundle = www.assetBundle;

                                                if (assetBundle == null) {

                                                    Debug.LogWarning("load assetbundle from streamingAssetPath failed, url = " + streamingAssetsPath);

                                                } else {

                                                    loadedAssetBundles.Add(assetBundleName, AssetBundleRefer.Spawn(assetBundle));
                                                }

                                            } else {

                                                Debug.LogWarning("load assetbundle failed, WWW.error = " + error + ", url = " + streamingAssetsPath);
                                            }
                                        }

#endif

                                    } else {

                                        Debug.LogWarning("not found assetbundle file, file path = " + persistentDataPath);
                                    }

                                }

                            }

                        }
                        break;
                } // end switch

            } // end for

            List<T> result = new List<T>();

            for (int i = 0, len = loadingTasks.Count; i < len; i++) {

                var task = loadingTasks[i];

                switch (task.TaskType) {
                    case LoadingTaskType.LoadingAssetFromResources:
                    case LoadingTaskType.LoadingAssetFromAssetBundle: {

                            AssetRefer assetRefer;
                            if (loadedAssets.TryGetValue(task.AssetPath, out assetRefer)) {

                                assetRefer.Retain();
                                result.Add(assetRefer.GetAsset<T>());

                            } else {

                                result.Add(null);
                            }
                        }
                        break;
                    case LoadingTaskType.LoadingScene:
                    case LoadingTaskType.LoadingSceneFromAssetBundle: {
                            result.Add(null);
                        }
                        break;
                    case LoadingTaskType.LoadingAssetBundle: {

                            AssetBundleRefer assetBundleRefer;
                            if (loadedAssetBundles.TryGetValue(task.AssetBundleName, out assetBundleRefer)) {

                                assetBundleRefer.Retain();

                            } else {

                                // loading assetbundle, but seems failed.
                            }
                        }
                        break;
                } // end switch

            } // end for

            loadingTasks.UnspawnLoadingTasks();

            if (completed != null) {
                completed.Invoke(result);
            }

        }

        public static void UnloadAssets(this List<string> assetPaths, Dictionary<string, AssetRefer> loadedAssets,
            Dictionary<string, AssetBundleRefer> loadedAssetBundles, AssetBundleManifest manifest, bool unloadUnusedAssets = false,
            Action completed = null) {

            var dirty = false;

            for (int i = 0, len = assetPaths.Count; i < len; i++) {

                var assetPath = assetPaths[i];

                AssetRefer assetRefer;
                if (loadedAssets.TryGetValue(assetPath, out assetRefer)) {
                    if (assetRefer.Release() == 0) {

                        // release assetBundle references.

                        loadedAssets.Remove(assetPath);
                        AssetRefer.Unspawn(assetRefer);
                        dirty = true;
                    } else {

                        // not need to release the assetBundle, because the reference count is bigger than zero.
                    }
                }

                var assetBundleNames = manifest.GetAllDependenciesByAssetPath(assetPath, true);

                for (int j = 0, len2 = assetBundleNames.Count; j < len2; j++) {
                    var assetBundleName = assetBundleNames[j];
                    AssetBundleRefer assetBundleRefer;
                    if (loadedAssetBundles.TryGetValue(assetBundleName, out assetBundleRefer)) {
                        if (assetBundleRefer.Release() == 0) {

                            // release asset references.

                            loadedAssetBundles.Remove(assetBundleName);
                            AssetBundleRefer.Unspawn(assetBundleRefer);
                            dirty = true;
                        } else {

                            // not need to release the asset, because the reference count is bigger than zero.
                        }
                    }
                }
            }

            if (dirty && unloadUnusedAssets) {
                Resources.UnloadUnusedAssets();
            }

            if (completed != null) {
                completed.Invoke();
            }
        }

        public static List<LoadingTask> SpawnLoadingTasks(this List<string> assetPaths, AssetBundleManifest manifest) {

            List<LoadingTask> loadingTasks = new List<LoadingTask>();
            for (int i = 0, len = assetPaths.Count; i < len; i++) {

                var assetPath = assetPaths[i];

                string targetName = manifest.GetAssetBundleByAssetPath(assetPath);

                if (!string.IsNullOrEmpty(targetName)) {

                    LoadingTask lastTask;
                    if (assetPath.IsScenePath()) {

                        var scenceName = assetPath.GetScenceName();

                        lastTask = LoadingTask.LoadingSceneFromAssetBundle(scenceName, targetName);

                    } else {

                        lastTask = LoadingTask.LoadingAssetFromAssetBundle(assetPath, targetName);
                    }

                    var assetBundleNames = manifest.GetAllDependencies(targetName, true);

                    assetBundleNames.Reverse();

                    for (int j = 0, len2 = assetBundleNames.Count; j < len2; j++) {

                        loadingTasks.Add(LoadingTask.LoadingAssetBundle(assetBundleNames[j]));
                    }

                    loadingTasks.Add(lastTask);

                } else {

                    if (assetPath.IsScenePath()) {

                        var scenceName = assetPath.GetScenceName();

                        loadingTasks.Add(LoadingTask.LoadingScene(scenceName));

                    } else {

                        var resoucesPath = assetPath.GetResourcesPath();

                        loadingTasks.Add(LoadingTask.LoadingAssetFromResources(assetPath, resoucesPath));

                    }
                }
            }
            return loadingTasks;
        }

        public static void UnspawnLoadingTasks(this List<LoadingTask> loadingTasks) {
            for (int i = 0, len = loadingTasks.Count; i < len; i++) {
                LoadingTask.Unspawn(loadingTasks[i]);
            }
            loadingTasks.Clear();
        }

    }

}
