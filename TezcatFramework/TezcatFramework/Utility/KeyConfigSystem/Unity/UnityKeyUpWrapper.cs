using UnityEngine;

namespace tezcat.Framework.Utility
{
    public class UnityKeyUpWrapper : UnityKeyWrapper
    {
        public override State state => State.Up;

        public override bool active()
        {
            return Input.GetKeyUp(keyCode);
        }
    }
}