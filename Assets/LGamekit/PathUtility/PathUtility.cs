using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

namespace LGamekit.LegacyAssetManagementSystem {

	/// <summary>
	/// 
	/// Unity path utility
	/// 
	/// Reference:
	/// [0]https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Application-dataPath.html
	/// [1]https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Application-persistentDataPath.html
	/// [2]https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Application-streamingAssetsPath.html
	/// [3]https://docs.unity3d.com/2019.1/Documentation/Manual/StreamingAssets.html
	/// [4]https://github.com/kimsama/Unity-Paths
	/// </summary>

    public static class PathUtility {

        public static string ASSETBUNDLES_FOLDER = "AssetBundles/";
        public static string ASSETS_FOLDER = "Assets/";
        public static string RESOURCES_FOLDER = "Resources/";
        public static string STREAMINGASSETS_FOLDER = "StreamingAssets/";

        public static string ASSETBUNDLES_FOLDER_NAME = "AssetBundles";
        public static string ASSETS_FOLDER_NAME = "Assets";
        public static string RESOURCES_FOLDER_NAME = "Resources";
        public static string STREAMINGASSETS_FOLDER_NAME = "StreamingAssets";

        public static string SCENE_FILE_EXTENSION = ".unity";

        public static string Substring(this string str, string sub) {
            var index = str.IndexOf(sub, StringComparison.Ordinal);
            return index < 0 ? str : str.Substring(index + sub.Length);
        }

        public static bool IsAssetPath(this string path) {
            return path.Contains(PathUtility.ASSETS_FOLDER_NAME + "/");
        }

        public static bool IsResourcesPath(this string path) {
            return path.Contains(PathUtility.RESOURCES_FOLDER_NAME + "/");
        }

        public static bool IsScenePath(this string path) {
            return Path.GetExtension(path) == PathUtility.SCENE_FILE_EXTENSION;
        }

        public static string GetAssetPath(this string fullPath) {
            return ASSETS_FOLDER + fullPath.Substring(ASSETS_FOLDER);
        }

#if UNITY_EDITOR
        public static string GetAssetPath(this UnityEngine.Object asset) {
            return UnityEditor.AssetDatabase.GetAssetPath(asset);
        }
#endif

        /// <summary>
        /// Retrieves Application.persistentDataPath depends on the given platform.
        /// 
        /// iOS            - /var/mobile/Applications/[program_ID]/Documents : read/write
        /// Android        - [External] /mnt/sdcard/Android/data/[bundle id]/files : read/write
        ///                  [Internal] /data/data/[bundle id]/files/ : read/write
        /// WEB Player     - /                
        /// Windows Player - [UserDirectory]/AppData/LocalLow/[Company]/[Product Name] : read/write
        /// OSX            - [UserDirectory]/Library/Caches/unity.[Company].[Product] : read/write
        /// Windows Edtor  - [UserDirectory]/AppData/LocalLow/[Company]/[Product] : read/write
        /// Mac Editor     - [UserDirectory]/Library/Caches/unity.[Company].[Product] : read/write
        /// </summary>
        public static string GetPersistentDataPath() {
            return Application.persistentDataPath;
        }

        public static string GetPersistentDataPath(this string relativePath) {
            return Path.Combine(Application.persistentDataPath, relativePath);
        }

        public static string GetResourcesPath(this string assetPath) {
            /*
            //Load a text file (Assets/Resources/Text/textFile01.txt)
            var textFile = Resources.Load<TextAsset>("Text/textFile01");

            //Load text from a JSON file (Assets/Resources/Text/jsonFile01.json)
            var jsonTextFile = Resources.Load<TextAsset>("Text/jsonFile01");
            //Then use JsonUtility.FromJson<T>() to deserialize jsonTextFile into an object

            //Load a Texture (Assets/Resources/Textures/texture01.png)
            var texture = Resources.Load<Texture2D>("Textures/texture01");

            //Load a Sprite (Assets/Resources/Sprites/sprite01.png)
            var sprite = Resources.Load<Sprite>("Sprites/sprite01");

            //Load an AudioClip (Assets/Resources/Audio/audioClip01.mp3)
            var audioClip = Resources.Load<AudioClip>("Audio/audioClip01");
             */
            return assetPath.Substring(RESOURCES_FOLDER).Replace(Path.GetExtension(assetPath), string.Empty);
        }

        /// <summary>
        /// Retrieves Application.streamingAssetsPath depends on the given platform.
        /// 
        /// iOS            - /var/mobile/Applications/[program_ID]/[appname].app/Data/Raw  (read only)
        /// Android        - [External] jar:file:///data/app/[bundle id].apk!/assets  (read only, accessiable via www)
        ///                  [Internal] jar:file:///data/app/[bundle id].apk!/assets  (read only, accessiable via www)
        /// WEB            - Application.streamingAssetsPath : empty
        /// Windows Player - [Exe file]/[Exe file]_Data/StreamingAssets : read/write
        /// OSX Player     - [Exe file].app/Contents/Data/StreamingAssets : read/write
        /// Windows Edtor  - [ProjectDirectory]/Assets/StreamingAssets	: read/write
        /// Mac Editor     - [ProjectDirectory]/Assets/StreamingAssets : read/write
        /// </summary>
        public static string GetStreamingAssetsPath() {
            return Application.streamingAssetsPath;
        }

        public static string GetStreamingAssetsPath(this string relativePath, URLHead urlHead = URLHead.None) {
			// https://docs.unity3d.com/2018.4/Documentation/ScriptReference/WWW.html
            switch (urlHead) {
            case URLHead.None:
                break;
            case URLHead.File: {
                    if (Application.platform != RuntimePlatform.Android)
                        return Path.Combine("file://" + Application.streamingAssetsPath, relativePath);
                }
                break;
            case URLHead.Http:
                return Path.Combine("http://" + Application.streamingAssetsPath, relativePath);
            case URLHead.Https:
                return Path.Combine("https://" + Application.streamingAssetsPath, relativePath);
            case URLHead.Ftp:
                return Path.Combine("ftp://" + Application.streamingAssetsPath, relativePath);
            }
            return Path.Combine(Application.streamingAssetsPath, relativePath);
        }

        public static string GetAssetBundlePath(this string assetBundleName, bool persistentData = true, URLHead urlHead = URLHead.File) {
            if (persistentData) {
                return GetPersistentDataPath(Path.Combine(ASSETBUNDLES_FOLDER, assetBundleName));
            }
            return GetStreamingAssetsPath(Path.Combine(ASSETBUNDLES_FOLDER, assetBundleName), urlHead);
        }

        public static string GetScenceName(this string assetPath) {
            return Path.GetFileNameWithoutExtension(assetPath);
        }

    }

}
