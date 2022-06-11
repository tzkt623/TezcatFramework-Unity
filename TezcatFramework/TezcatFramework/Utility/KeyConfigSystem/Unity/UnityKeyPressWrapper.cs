using UnityEngine;

namespace tezcat.Framework.Utility
{
    public class UnityKeyPressWrapper : UnityKeyWrapper
    {
        public override State state => State.Press;

        public override bool active()
        {
            return Input.GetKey(keyCode);
        }
    }
}