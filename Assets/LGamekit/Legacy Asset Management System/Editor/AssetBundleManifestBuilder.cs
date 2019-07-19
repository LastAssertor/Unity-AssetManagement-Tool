using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace LGamekit.LegacyAssetManagementSystem {

    [CreateAssetMenu(fileName = "AssetBundleManifestBuilder.asset", menuName = "LGamekit/LegacyAssetManagementSystem/AssetBundleManifestBuilder")]
    public class AssetBundleManifestBuilder : ScriptableObject {

        public string inputPath;
        public string outputFolder;
        public string outputName = "AssetBundleManifest.txt";

        public void Execute() {
            Execute(inputPath, outputFolder + "/" + outputName);
        }

        public static void Execute(string inputPath, string outputPath) {

            AssetBundle assetbundle = null;

            if (File.Exists(inputPath)) {
                assetbundle = AssetBundle.LoadFromFile(inputPath);
            }

            UnityEngine.AssetBundleManifest manifest = null;

            if (assetbundle != null) {
                manifest = assetbundle.LoadAsset<UnityEngine.AssetBundleManifest>("AssetBundleManifest");
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

            File.WriteAllText(outputPath, json, Encoding.UTF8);

            AssetDatabase.Refresh();
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
