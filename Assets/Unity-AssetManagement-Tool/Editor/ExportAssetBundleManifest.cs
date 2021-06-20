using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace UnityAssetManagementTool {

    public static class ExportAssetBundleManifest {

        [@MenuItem("Assets/Unity-AssetManagement-Tool/1.预处理（导出AssetBundleManifest.txt）", false, 5010)]
        public static void Export() {

            var guids = AssetDatabase.FindAssets("t:UnityAssetManagementTool.Settings");

            if (guids.Length == 0) {
                Debug.LogWarning("Not found 'UnityAssetManagementTool.Settings' asset.");
                return;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var settings = AssetDatabase.LoadAssetAtPath<UnityAssetManagementTool.Settings>(assetPath);
            settings.GenerateAssetBundleManifest();

        }

    }

}
