using System.Collections;
using UnityEngine;

namespace HappyKit
{
    public static class Coroutines
    {
        /// <summary>
        /// Usage example: StartCoroutine(LerpCoroutine(x => slider.value = x, 0, 1, 0.1f));
        /// </summary>
        /// <param name="setterFunction">function to run with the interpolated value</param>
        /// <param name="original">starting value</param>
        /// <param name="target">target value</param>
        /// <param name="durationSeconds">duration in seconds</param>
        /// <returns></returns>
        public static IEnumerator LerpCoroutine(System.Action<float> setterFunction, float original, float target, float durationSeconds)
        {
            float eTime = 0f;
            while (eTime < durationSeconds)
            {
                setterFunction(UnityEngine.Mathf.Lerp(original, target, eTime / durationSeconds));
                yield return null;
                eTime += Time.deltaTime;
            }
            setterFunction(target);
        }
        /// <summary>
        /// Usage example: StartCoroutine(LerpCoroutine(x => slider.value = x, 0, 1, 0.1f, t => t*t));
        /// </summary>
        /// <param name="setterFunction">function to run with the interpolated value</param>
        /// <param name="original">starting value</param>
        /// <param name="target">target value</param>
        /// <param name="durationSeconds">duration in seconds</param>
        /// <param name="curve">the curve of interpolation speed (0~1)</param>
        /// <returns></returns>
        public static IEnumerator LerpCoroutine(System.Action<float> setterFunction, float original, float target, float durationSeconds, System.Func<float, float> curve)
        {
            float eTime = 0f;
            while (eTime < durationSeconds)
            {
                setterFunction(UnityEngine.Mathf.Lerp(original, target, eTime / durationSeconds));
                yield return null;
                eTime += Time.deltaTime;
            }
            setterFunction(target);
        }
    }
}