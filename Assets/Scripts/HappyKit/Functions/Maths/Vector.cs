using UnityEngine;

namespace HappyKit
{
    public static class Vector
    {
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