using UnityEngine;

namespace tezcat.Unity.Utility
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