using UnityEngine;

namespace tezcat.Framework.Game
{
    public interface ITezHoverable
    {
        void onEnter(ref RaycastHit hit);

        void onExit();

        void onHovering(ref RaycastHit hit);
    }
}