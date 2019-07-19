using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace LGamekit {

    /// <summary>
    /// 拷贝资源的工程目录路径到粘贴板
    /// </summary>
    public static class CopyAssetPath {

        [@MenuItem("Assets/Copy AssetPath", false, 2000)]
        private static void Copy() {
            if (EditorApplication.isCompiling || Selection.activeInstanceID == 0)
                return;
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            GUIUtility.systemCopyBuffer = path;
            Debug.Log(string.Format("copy buffer : {0}", path));
        }

    }

}
