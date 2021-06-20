using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace UnityAssetManagementTool {

    public sealed class LoadingTask {

        static ObjectPool<LoadingTask> m_Pool;
        static ObjectPool<LoadingTask> Pool {
            get {
                if (m_Pool == null) {
                    m_Pool = ObjectPool<LoadingTask>.Create(Factory, null, InternalUnspawn, null, 128);
                }
                return m_Pool;
            }
        }

        static LoadingTask Factory() {
            return new LoadingTask();
        }

        static void InternalUnspawn(LoadingTask loadingTask) {
            loadingTask.AssetPath = string.Empty;
            loadingTask.AssetBundleName = string.Empty;
            loadingTask.ScenceName = string.Empty;
            loadingTask.ResourcesPath = string.Empty;
        }

        public LoadingTaskType TaskType { get; private set; }
        public string AssetPath { get; private set; }
        public string AssetBundleName { get; private set; }
        public string ScenceName { get; private set; }
        public string ResourcesPath { get; private set; }

        private LoadingTask() {

        }

        public static void Unspawn(LoadingTask loadingTask) {
            Pool.Unspawn(loadingTask);
        }

        public static LoadingTask LoadingAssetBundle(string assetbundleName) {
            var loadingTask = Pool.Spawn();
            loadingTask.TaskType = LoadingTaskType.LoadingAssetBundle;
            loadingTask.AssetBundleName = assetbundleName;
            return loadingTask;
        }

        public static LoadingTask LoadingAssetFromAssetBundle(string assetPath, string assetbundleName) {
            var loadingTask = Pool.Spawn();
            loadingTask.TaskType = LoadingTaskType.LoadingAssetFromAssetBundle;
            loadingTask.AssetPath = assetPath;
            loadingTask.AssetBundleName = assetbundleName;
            return loadingTask;
        }

        public static LoadingTask LoadingAssetFromResources(string assetPath, string resourcesPath) {
            var loadingTask = Pool.Spawn();
            loadingTask.TaskType = LoadingTaskType.LoadingAssetFromResources;
            loadingTask.AssetPath = assetPath;
            loadingTask.ResourcesPath = resourcesPath;
            return loadingTask;
        }

        public static LoadingTask LoadingScene(string scenceName) {
            var loadingTask = Pool.Spawn();
            loadingTask.TaskType = LoadingTaskType.LoadingScene;
            loadingTask.ScenceName = scenceName;
            return loadingTask;
        }

        public static LoadingTask LoadingSceneFromAssetBundle(string scenceName, string assetbundleName) {
            var loadingTask = Pool.Spawn();
            loadingTask.TaskType = LoadingTaskType.LoadingSceneFromAssetBundle;
            loadingTask.ScenceName = scenceName;
            loadingTask.AssetBundleName = assetbundleName;
            return loadingTask;
        }

    }

}
