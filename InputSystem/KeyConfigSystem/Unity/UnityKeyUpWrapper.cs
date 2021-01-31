using UnityEngine;

namespace tezcat.Framework.InputSystem
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