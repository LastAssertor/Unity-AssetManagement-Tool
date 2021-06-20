using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityAssetManagementTool {

    public class LoadCube : MonoBehaviour {

        public string assetPath = "Assets/Unity-AssetManagement-Tool/Example/Prefab/Cube.prefab";

        void Start() {
            string json = Resources.Load<TextAsset>("AssetBundleManifest").text;
            var manifest = JsonUtility.FromJson<AssetBundleManifest>(json);
            AssetManagementTool.Instance.SetAssetBundleManifest(manifest)
                .SetAssetBundlesFolder(string.Empty)
                .LoadAssetAsync<GameObject>(assetPath, asset => {
                    if (asset == null) {
                        // load failed.
                    } else {
                        Instantiate(asset);
                    }
                }).UnloadAssetAsync(assetPath);
        }

    }

}