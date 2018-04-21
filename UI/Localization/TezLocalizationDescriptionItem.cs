using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationDescriptionItem
        : TezWidget
    {
        Image m_Flag = null;
        Text m_Name = null;

        protected override void Awake()
        {
            base.Awake();
            m_Flag = this.GetComponentInChildren<Image>();
            m_Name = this.GetComponentInChildren<Text>();
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}