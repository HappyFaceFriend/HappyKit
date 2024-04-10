using UnityEngine;

namespace HappyKit
{
    public static class Vector
    {
        #region Conversions
        /// <summary>
        /// Vector2(x, y)
        /// </summary>
        static Vector2 ToVector2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        /// <summary>
        /// Vector2(x, z)
        /// </summary>
        static Vector2 ToVector2XZ(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
        /// <summary>
        /// Vector3(x, y, 0)
        /// </summary>
        static Vector3 ToVector3(Vector2 vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }
        /// <summary>
        /// Vector3(x, 0, y)
        /// </summary>
        static Vector3 ToVector3XZ(Vector2 vec)
        {
            return new Vector3(vec.x, 0, vec.y);
        }
        #endregion

        public static bool IsPositionCrossed(Vector3 targetPos, Vector3 currentPos, Vector3 lastPos)
        {
            Vector3 a = currentPos - targetPos;
            Vector3 b = lastPos - targetPos;

            return a.x * b.x < 0 || a.y * b.y < 0;
        }
        public static Vector2 AngleToVector(float degrees)
        {

            return new Vector2(UnityEngine.Mathf.Cos(degrees * UnityEngine.Mathf.Deg2Rad), UnityEngine.Mathf.Sin(degrees * UnityEngine.Mathf.Deg2Rad));
        }
    }
}