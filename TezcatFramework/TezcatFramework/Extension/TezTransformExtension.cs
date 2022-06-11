using UnityEngine;

namespace tezcat.Framework.Extension
{
    public static class TezTransformExtension
    {
        public static void reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

    }
}