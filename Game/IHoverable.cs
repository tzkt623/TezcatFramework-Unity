using UnityEngine;

namespace tezcat
{
    public interface ITezHoverable
    {
        void onEnter(ref RaycastHit hit);

        void onExit();

        void onHovering(ref RaycastHit hit);
    }
}