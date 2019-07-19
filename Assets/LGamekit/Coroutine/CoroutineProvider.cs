using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGamekit {
	
	public sealed class CoroutineProvider : MonoBehaviour {

		static CoroutineProvider m_Instance;

		public static CoroutineProvider Instance {
			get {
				if (m_Instance == null) {
					m_Instance = FindObjectOfType<CoroutineProvider>();
				}
				if (m_Instance == null) {
					m_Instance = new GameObject(typeof(CoroutineProvider).Name).AddComponent<CoroutineProvider>();
                    DontDestroyOnLoad(m_Instance.gameObject);
				}
                return m_Instance;
			}
		}

	}


}
