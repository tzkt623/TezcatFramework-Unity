using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.UI
{
    public class TezLocMenu : TezArea
    {
        [SerializeField]
        TezImageLabelButton m_AddName = null;
        [SerializeField]
        TezImageLabelButton m_AddDescription = null;
        [SerializeField]
        TezImageLabelButton m_Refresh = null;
        [SerializeField]
        TezImageLabelButton m_Save = null;

        protected override void onRefresh()
        {

        }
    }
}