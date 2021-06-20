using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace UnityAssetManagementTool {

    [CreateAssetMenu(fileName = "Settings.asset", menuName = "Unity-AssetManagement-Tool/Settings")]
    public class Settings : ScriptableObject {

        public string inputPath = "Assets/StreamingAssets/StandaloneOSXUniversal";
        public string outputPath = "Assets/Unity-AssetManagement-Tool/Example/Resources/AssetBundleManifest.txt";

        [ContextMenu("Generate AssetBundleManifest.txt")]
        public void GenerateAssetBundleManifest() {
            GenerateAssetBundleManifest(inputPath, outputPath);
        }

        public static void GenerateAssetBundleManifest(string inputPath, string outputPath) {

            AssetBundle assetbundle;

            if (File.Exists(inputPath)) {
                assetbundle = AssetBundle.LoadFromFile(inputPath);
            } else {
                Debug.LogWarning($"No found file : {inputPath}");
                return;
            }

            UnityEngine.AssetBundleManifest manifest;

            if (assetbundle != null) {
                manifest = assetbundle.LoadAsset<UnityEngine.AssetBundleManifest>("AssetBundleManifest");
            } else {
                Debug.LogWarning($"Faild to load assetbundle : {inputPath}");
                return;
            }

            if (manifest == null) {
                Debug.LogWarning($"Faild to load UnityEngine.AssetBundleManifest from assetbundle : {inputPath}");
                return;
            }

            var obj = Create(manifest);

            var json = UnityEngine.JsonUtility.ToJson(obj, true);

            if (File.Exists(outputPath)) {
                File.Delete(outputPath);
            }

            var dir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(outputPath, json);

            Resources.UnloadAsset(manifest);
            manifest = null;
            Resources.UnloadUnusedAssets();
            assetbundle.Unload(true);
            assetbundle = null;
            Resources.UnloadUnusedAssets();

            AssetDatabase.Refresh();

            Debug.Log($"Export UnityAssetManagementTool.AssetBundleManifest successed! {outputPath}");
        }

        public static AssetBundleManifest Create(UnityEngine.AssetBundleManifest manifest) {

            AssetBundleManifest result = new AssetBundleManifest() {
                Version = DateTime.Now.ToString("yyyyMMddHHmmss")
            };

            if (manifest == null) {
                return result;
            }

            List<string> assetbundleNames = new List<string>();
            assetbundleNames.AddRange(manifest.GetAllAssetBundles());
            foreach (var name in assetbundleNames) {
                AssetBundleInfo info = new AssetBundleInfo() {
                    AssetBundleName = name,
                    Hash128 = manifest.GetAssetBundleHash(name).ToString()
                };
                info.AssetPaths.AddRange(AssetDatabase.GetAssetPathsFromAssetBundle(name));
                var dependencies = manifest.GetAllDependencies(name);
                foreach (var depName in dependencies) {
                    info.Dependencies.Add(assetbundleNames.IndexOf(depName));
                }
                result.Infos.Add(info);
            }
            return result;

        }

    }

}
