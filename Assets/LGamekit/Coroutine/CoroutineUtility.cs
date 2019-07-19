using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace LGamekit {

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