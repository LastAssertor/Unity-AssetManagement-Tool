using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGamekit.LegacyAssetManagementSystem {

    public interface IAssetBundleManifest {

        List<string> GetAllAssetBundles();

        string GetAssetBundleHash(string assetBundleName);
        List<string> GetAllDependencies(string assetBundleName, bool includeSelf = false);

        string GetAssetBundleByAssetPath(string assetPath);
        string GetAssetBundleHashByAssetPath(string assetPath);
        List<string> GetAllDependenciesByAssetPath(string assetPath, bool includeSelf = false);

    }

}
