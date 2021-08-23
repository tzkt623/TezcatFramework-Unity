using UnityEngine;

namespace tezcat.Framework.Utility
{
    public class UnityKeyDownWrapper : UnityKeyWrapper
    {
        public override State state => State.Down;

        public override bool active()
        {
            return Input.GetKeyDown(keyCode);
        }
    }
}