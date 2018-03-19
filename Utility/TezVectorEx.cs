using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{

    public static class TezVectorEx
    {

        public static Vector2 toVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 divide(this Vector2 self, Vector2 other)
        {
            return new Vector2(self.x / other.x, self.y / other.y);
        }
    }
}
