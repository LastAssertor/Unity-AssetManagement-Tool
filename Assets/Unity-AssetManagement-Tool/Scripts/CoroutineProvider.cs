using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityAssetManagementTool {

    [AddComponentMenu("")]
    public sealed class CoroutineProvider : MonoBehaviour {

        static CoroutineProvider m_Instance;

        public static CoroutineProvider Instance {
            get {
                if (m_Instance == null) {
                    m_Instance = new GameObject(typeof(CoroutineProvider).Name).AddComponent<CoroutineProvider>();
                    DontDestroyOnLoad(m_Instance.gameObject);
                }
                return m_Instance;
            }
        }

    }

    public static class CoroutineUtility {

        public static Coroutine StartCoroutine(this IEnumerator routine) {
            return CoroutineProvider.Instance.StartCoroutine(routine);
        }

        public static void StopCoroutine(this IEnumerator routine) {
            CoroutineProvider.Instance.StopCoroutine(routine);
        }

        public static void StopCoroutine(this Coroutine routine) {
            CoroutineProvider.Instance.StopCoroutine(routine);
        }


    }

}
