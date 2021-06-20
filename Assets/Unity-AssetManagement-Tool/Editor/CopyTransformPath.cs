using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace UnityAssetManagementTool {

    /// <summary>
    /// 拷贝物体在场景中的层次路径到粘贴板
    /// </summary>
    public static class CopyTransformPath {

        [@MenuItem("GameObject/Copy TransformPath", false, -1000)]
        private static void Copy() {
            if (Selection.activeTransform == null)
                return;
            var path = AnimationUtility.CalculateTransformPath(Selection.activeTransform, null);
            GUIUtility.systemCopyBuffer = path;
            Debug.Log(string.Format("copy buffer : {0}", path));
        }

    }

}
