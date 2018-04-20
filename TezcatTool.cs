using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.UI
{
    public class TezcatTool : TezWidget
    {
        [SerializeField]
        TezImageLabelButton m_Database;
        [SerializeField]
        TezImageLabelButton m_Localization;
        [SerializeField]
        TezImageLabelButton m_Option;
        [SerializeField]
        TezImageLabelButton m_Close;

        protected override void Awake()
        {
            base.Awake();

        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}