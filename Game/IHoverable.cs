using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public interface ITezHoverable
    {
        void onEnter();

        void onExit();

        void onHovering(ref RaycastHit hit);
    }
}