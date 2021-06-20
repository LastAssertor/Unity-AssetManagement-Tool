
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Object = UnityEngine.Object;

namespace UnityAssetManagementTool {

    [AddComponentMenu("")]
    public sealed class AssetManagementTool : MonoBehaviour {

        static AssetManagementTool m_Instance;

        public static AssetManagementTool Instance {
            get {
                if (m_Instance == null) {
                    m_Instance = new GameObject(typeof(AssetManagementTool).Name).AddComponent<AssetManagementTool>();
                    DontDestroyOnLoad(m_Instance.gameObject);
                }
                return m_Instance;
            }
        }

        AssetBundleManifest m_AssetBundleManifest = new AssetBundleManifest();

        Dictionary<string, AssetBundleRefer> m_LoadedAssetBundles = new Dictionary<string, AssetBundleRefer>();
        Dictionary<string, AssetRefer> m_LoadedAssets = new Dictionary<string, AssetRefer>();

        SimpleActionSequence m_ActionSequence;
        SimpleActionSequence ActionSequence {
            get {
                if (m_ActionSequence == null) {
                    m_ActionSequence = new SimpleActionSequence(() => Working == false);
                }
                return m_ActionSequence;
            }
        }

        public bool Working { get; private set; }

        readonly Queue<List<LoadingTask>> m_CachedLoadingTasks = new Queue<List<LoadingTask>>();
        readonly Queue<Delegate> m_CachedLoadingCallbacks = new Queue<Delegate>();
        List<LoadingTask> m_LoadingTasks;
        Delegate m_LoadingCallback;

        Coroutine m_WorkingCoroutine;

        public AssetManagementTool SetAssetBundleManifest(AssetBundleManifest manifest) {
            Debug.Log("setup assetBundleManifest : new version ===> " + manifest.Version);
            m_AssetBundleManifest = manifest;
            return this;
        }

        public AssetManagementTool SetAssetBundlesFolder(string assetBundlesFolder) {
            Debug.Log("setup assetBundles folder : " + PathUtility.ASSETBUNDLES_FOLDER + " ===> " +
                (string.IsNullOrEmpty(assetBundlesFolder) ? "null" : assetBundlesFolder));
            PathUtility.ASSETBUNDLES_FOLDER = assetBundlesFolder;
            if (!string.IsNullOrEmpty(assetBundlesFolder)) {
                PathUtility.ASSETBUNDLES_FOLDER_NAME = assetBundlesFolder.Substring(0, assetBundlesFolder.Length - 1);
            } else {
                PathUtility.ASSETBUNDLES_FOLDER_NAME = string.Empty;
            }
            return this;
        }

        public AssetManagementTool LoadAssetAsync<T>(string assetPath, Action<T> completed = null) where T : Object {
            return LoadAssetsAsync<T>(new List<string> { assetPath }, assets => {
                if (completed != null) {
                    completed.Invoke(assets[0]);
                }
            });
        }

        public AssetManagementTool LoadAssetsAsync<T>(List<string> assetPaths, Action<List<T>> completed = null) where T : Object {
            m_CachedLoadingTasks.Enqueue(assetPaths.SpawnLoadingTasks(m_AssetBundleManifest));
            m_CachedLoadingCallbacks.Enqueue(completed);
            ActionSequence.Enqueue(() => InternalLoadAssets<T>(), true);
            return this;
        }

        public AssetManagementTool UnloadAssetAsync(string assetPath, bool unloadUnusedAssets = false, Action completed = null) {
            return UnloadAssetsAsync(new List<string> { assetPath }, unloadUnusedAssets, completed);
        }

        public AssetManagementTool UnloadAssetsAsync(List<string> assetPaths, bool unloadUnusedAssets = false, Action completed = null) {
            ActionSequence.Enqueue(() => InternalUnloadAssets(assetPaths, unloadUnusedAssets, completed), true);
            return this;
        }

        public AssetManagementTool UnloadAllAssetsAsync(bool unloadUnusedAssets = false, Action completed = null) {
            ActionSequence.Enqueue(() => InternalUnloadAllAssets(unloadUnusedAssets, completed), true);
            return this;
        }

        /// <summary>
        /// Stop all loading tasks, and release all references.(immediately)
        /// </summary>
        public AssetManagementTool Shutdown(bool unloadUnusedAssets = false) {
            ActionSequence.Clear();

            if (Working) {
                if (m_WorkingCoroutine != null) {
                    m_WorkingCoroutine.StopCoroutine();
                }
                Working = false;
            }
            m_WorkingCoroutine = null;

            if (m_LoadingTasks != null) {
                m_LoadingTasks.UnspawnLoadingTasks();
                m_LoadingTasks = null;
            }

            while (m_CachedLoadingTasks.Count > 0) {
                var list = m_CachedLoadingTasks.Dequeue();
                if (list != null) {
                    list.UnspawnLoadingTasks();
                }
            }
            m_CachedLoadingCallbacks.Clear();

            foreach (var kvp in m_LoadedAssets) {
                kvp.Value.Release(true);
                AssetRefer.Unspawn(kvp.Value);
            }
            foreach (var kvp in m_LoadedAssetBundles) {
                kvp.Value.Release(true);
                AssetBundleRefer.Unspawn(kvp.Value);
            }
            m_LoadedAssets.Clear();
            m_LoadedAssetBundles.Clear();

            if (unloadUnusedAssets) {
                Resources.UnloadUnusedAssets();
            }
            return this;
        }

        void InternalLoadAssets<T>() where T : Object {
            Working = true;
            m_LoadingTasks = m_CachedLoadingTasks.Dequeue();
            m_LoadingCallback = m_CachedLoadingCallbacks.Dequeue();
            m_WorkingCoroutine = m_LoadingTasks.LoadAssetsAsync<T>(m_LoadedAssets, m_LoadedAssetBundles, x => {
                Action<List<T>> completed = (Action<List<T>>)m_LoadingCallback;
                if (completed != null) {
                    completed.Invoke(x);
                }
                m_LoadingTasks = null;
                m_LoadingCallback = null;
                m_WorkingCoroutine = null;
                Working = false;
                ActionSequence.Dequeue();
            }, true).StartCoroutine();
        }

        void InternalUnloadAssets(List<string> assetPaths, bool unloadUnusedAssets, Action completed) {
            Working = true;
            assetPaths.UnloadAssets(m_LoadedAssets, m_LoadedAssetBundles, m_AssetBundleManifest, unloadUnusedAssets, completed);
            Working = false;
            ActionSequence.Dequeue();
        }

        void InternalUnloadAllAssets(bool unloadUnusedAssets, Action completed) {
            Working = true;
            foreach (var kvp in m_LoadedAssets) {
                kvp.Value.Release(true);
                AssetRefer.Unspawn(kvp.Value);
            }
            foreach (var kvp in m_LoadedAssetBundles) {
                kvp.Value.Release(true);
                AssetBundleRefer.Unspawn(kvp.Value);
            }
            m_LoadedAssets.Clear();
            m_LoadedAssetBundles.Clear();

            if (unloadUnusedAssets) {
                Resources.UnloadUnusedAssets();
            }

            if (completed != null) {
                completed.Invoke();
            }

            Working = false;
            ActionSequence.Dequeue();
        }

    }

}

