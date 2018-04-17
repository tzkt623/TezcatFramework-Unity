using System.Collections.Generic;
using tezcat.UI;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.DataBase
{
    public class TezDatabaseWindow : TezWindow
    {
        [Header("Item Pool")]
        RectTransform m_Pool = null;

        public TezRTTI selectGroupRTTI { get; set; }
        public TezRTTI selectTypeRTTI { get; set; }


        protected override void Start()
        {
            base.Start();
        }

        protected override void onRefresh()
        {
            base.onRefresh();
        }
    }
}