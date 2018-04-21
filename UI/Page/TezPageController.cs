using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezPageController : TezWidget
    {
        [SerializeField]
        int m_Count = 20;

        [SerializeField]
        LayoutGroup m_Layout = null;

        HorizontalLayoutGroup m_HLayout = null;
        VerticalLayoutGroup m_VLayout = null;
        GridLayoutGroup m_GLayout = null;



        protected override void Awake()
        {
            base.Awake();

            m_HLayout = m_Layout as HorizontalLayoutGroup;
            m_VLayout = m_Layout as VerticalLayoutGroup;
            m_GLayout = m_Layout as GridLayoutGroup;


        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}