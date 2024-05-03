using UnityEngine;

namespace HappyKit
{
    public static class VectorExtensions
    {
        #region Conversions
        /// <summary>
        /// Vector2(vec.x, vec.y)
        /// </summary>
        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        /// <summary>
        /// Vector2(vec.x, vec.z)
        /// </summary>
        public static Vector2 ToVector2XZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
        /// <summary>
        /// Vector3(vec.x, vec.y, z)
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vec, float z = 0)
        {
            return new Vector3(vec.x, vec.y, z);
        }
        /// <summary>
        /// Vector3(vec.x, y, vec.y)
        /// </summary>
        public static Vector3 ToVector3XZ(this Vector2 vec, float y = 0)
        {
            return new Vector3(vec.x, y, vec.y);
        }
        #endregion

    }
}