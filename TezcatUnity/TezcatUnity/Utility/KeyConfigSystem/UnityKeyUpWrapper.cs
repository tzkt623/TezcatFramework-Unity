using UnityEngine;

namespace tezcat.Unity.Utility
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