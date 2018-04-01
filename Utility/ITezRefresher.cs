using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Utility
{
    public interface ITezRefresher
    {
        bool dirty { get; set; }
        void onRefresh();
    }
}

