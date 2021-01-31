using UnityEngine;

namespace tezcat.Framework.InputSystem
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