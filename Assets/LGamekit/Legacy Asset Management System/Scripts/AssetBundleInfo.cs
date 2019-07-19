using System.Collections;
using System.Collections.Generic;

using System;

namespace LGamekit.LegacyAssetManagementSystem {

    [Serializable]
    public class AssetBundleInfo {
        public string Hash128;
        public string AssetBundleName;
        public List<string> AssetPaths = new List<string>();
        public List<int> Dependencies = new List<int>();
    }
}