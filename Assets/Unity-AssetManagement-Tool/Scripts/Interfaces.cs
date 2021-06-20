using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityAssetManagementTool {

    public interface IAssetBundleManifest {
        List<string> GetAllAssetBundles();

        string GetAssetBundleHash(string assetBundleName);
        List<string> GetAllDependencies(string assetBundleName, bool includeSelf = false);

        string GetAssetBundleByAssetPath(string assetPath);
        string GetAssetBundleHashByAssetPath(string assetPath);
        List<string> GetAllDependenciesByAssetPath(string assetPath, bool includeSelf = false);
    }

    public interface IAssetBundleUpdate {
        bool CheckUpdate(AssetBundleManifest client,
            AssetBundleManifest server, out List<string> deletes, out List<string> downloads);
    }

    public interface IReferCounter {
        int Count { get; }
        int Retain();
        int Release(bool force = false);
    }

}
