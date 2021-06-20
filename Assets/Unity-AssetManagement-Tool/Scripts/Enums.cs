using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityAssetManagementTool {

    public enum URLHead {
        None,
        /// <summary>
        /// file://
        /// </summary>
        File,
        /// <summary>
        /// http://
        /// </summary>
        Http,
        /// <summary>
        /// https://
        /// </summary>
        Https,
        /// <summary>
        /// ftp://
        /// </summary>
        Ftp,
    }

    public enum LoadingTaskType {
        LoadingAssetBundle,
        LoadingAssetFromAssetBundle,
        LoadingAssetFromResources,
        LoadingScene,
        LoadingSceneFromAssetBundle
    }

}
