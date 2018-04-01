using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public enum PickState
    {
        RayCast,
        MouseLeft,
        MouseRight,
    }

    public interface ITezPickable 
    {
        void onPicked(PickState pick_state);
    }
}