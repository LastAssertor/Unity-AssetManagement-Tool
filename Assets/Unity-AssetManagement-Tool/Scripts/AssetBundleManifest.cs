using System.Collections;
using System.Collections.Generic;

using System;

namespace UnityAssetManagementTool {

    [Serializable]
    public class AssetBundleManifest : IAssetBundleManifest {
        public string Version;
        public List<AssetBundleInfo> Infos = new List<AssetBundleInfo>();

        public List<string> GetAllAssetBundles() {
            return Infos.ConvertAll(x => x.AssetBundleName);
        }

        public string GetAssetBundleHash(string assetBundleName) {
            var info = Infos.Find(x => x.AssetBundleName == assetBundleName);
            if (info != null) {
                return info.Hash128;
            }
            return string.Empty;
        }

        public List<string> GetAllDependencies(string assetBundleName, bool includeSelf = false) {
            List<string> result = new List<string>();
            if (includeSelf) {
                result.Add(assetBundleName);
            }
            var info = Infos.Find(x => x.AssetBundleName == assetBundleName);
            if (info != null) {
                result.AddRange(info.Dependencies.ConvertAll(x => Infos[x].AssetBundleName));
            }
            return result;
        }

        public string GetAssetBundleByAssetPath(string assetPath) {
            var info = Infos.Find(x => x.AssetPaths.Contains(assetPath));
            if (info != null) {
                return info.AssetBundleName;
            }
            return string.Empty;
        }

        public string GetAssetBundleHashByAssetPath(string assetPath) {
            var info = Infos.Find(x => x.AssetPaths.Contains(assetPath));
            if (info != null) {
                return info.Hash128;
            }
            return string.Empty;
        }

        public List<string> GetAllDependenciesByAssetPath(string assetPath, bool includeSelf = false) {
            List<string> result = new List<string>();
            var info = Infos.Find(x => x.AssetPaths.Contains(assetPath));
            if (info != null) {
                if (includeSelf) {
                    result.Add(info.AssetBundleName);
                }
                result.AddRange(info.Dependencies.ConvertAll(x => Infos[x].AssetBundleName));
            }
            return result;
        }

    }

}
