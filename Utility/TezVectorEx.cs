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
    }
}
