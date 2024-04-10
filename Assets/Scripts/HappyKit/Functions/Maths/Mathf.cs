using UnityEngine;

namespace HappyKit
{
    public static class Mathf
    {
        public static float PointAndLineSegmentDistanceSqr(Vector2 a, Vector2 b, Vector2 p)
        {
            // Return minimum distance between line segment ab and point p

            // Consider the line extending the segment, parameterized as a + t (b - a).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-a) . (b-a)] / |b-a|^2
            Vector2 BminusA = b - a;
            float t = Vector2.Dot((p - a), BminusA) / BminusA.sqrMagnitude;
            if (0 <= t && t <= 1)
            {
                return Vector2.SqrMagnitude(p - (a + t * BminusA));
            }
            else if (t < 0)
                return Vector2.SqrMagnitude(p - a);
            else
                return Vector2.SqrMagnitude(p - b);
        }
    }
}