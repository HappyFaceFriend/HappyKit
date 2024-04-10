using System.Collections.Generic;
using UnityEngine;

namespace HappyKit
{
    public static partial class Random
    {
        /// <summary>
        /// Shuffles the array with Fisher-Yates algorithm
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i >= 1; i--)
            {
                int r = UnityEngine.Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }
        }
        /// <summary>
        /// Shuffles the list with Fisher-Yates algorithm
        /// </summary>
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int r = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }
        public static int SelectOneIndexWeighted(float[] weights)
        {
            float w = 0f;
            for (int i = 0; i < weights.Length; i++)
                w += weights[i];
            float r = UnityEngine.Random.Range(0, w);

            w = 0;
            int index = 0;
            for (; index < weights.Length; index++)
            {
                w += weights[index];
                if (r < w)
                    break;
            }
            return index;
        }
        public static T[] SelectN<T>(List<T> list, int n)
        {
            int[] indicies = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
                indicies[i] = i;
            Shuffle(indicies);
            T[] result = new T[n];
            for (int i = 0; i < n; i++)
            {
                if (i >= list.Count)
                    result[i] = list[indicies[UnityEngine.Random.Range(0, list.Count)]];
                else
                    result[i] = list[indicies[i]];
            }
            return result;
        }
        public static T[] SelectNWeighted<T>(List<T> list, float[] weights, int n)
        {
            float totalWeight = 0f;
            for (int i = 0; i < weights.Length; i++)
                totalWeight += weights[i];

            T[] result = new T[n];
            for (int i = 0; i < n; i++)
            {
                float r = UnityEngine.Random.Range(0, totalWeight);
                int index = 0;
                for (; index < weights.Length; index++)
                {
                    r -= weights[index];
                    if (r < 0)
                        break;
                }
                result[i] = list[index];
                totalWeight -= weights[index];
                weights[index] = 0;
            }
            return result;
        }
    }
}