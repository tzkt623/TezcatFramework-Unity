using UnityEngine;

namespace tezcat.Unity
{
    public static class TezVectorExtension
    {
        public static Vector2 toVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 divide(this Vector2 self, Vector2 other)
        {
            return new Vector2(self.x / other.x, self.y / other.y);
        }

        public static Vector3 add(this Vector3 v3, Vector2 v2)
        {
            v3.x += v2.x;
            v3.y += v2.y;
            return v3;
        }

        public static Vector3 sub(this Vector3 v3, Vector2 v2)
        {
            v3.x -= v2.x;
            v3.y -= v2.y;
            return v3;
        }
    }
}
