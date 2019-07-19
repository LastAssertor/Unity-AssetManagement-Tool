using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGamekit.LegacyAssetManagementSystem {

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
}