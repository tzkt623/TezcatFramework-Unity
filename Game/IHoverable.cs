using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public interface IHoverable
    {
        void onEnter();

        void onExit();

        void onHovering(ref RaycastHit hit);
    }
}