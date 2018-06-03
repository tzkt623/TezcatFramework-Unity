using UnityEngine;

namespace tezcat.Game
{
    public interface ITezHoverable
    {
        void onEnter(ref RaycastHit hit);

        void onExit();

        void onHovering(ref RaycastHit hit);
    }
}