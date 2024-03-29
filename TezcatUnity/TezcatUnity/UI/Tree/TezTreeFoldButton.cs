﻿using tezcat.Framework.Core;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Unity.UI
{
    public class TezTreeFoldButton
        : TezUIWidget
        , ITezClickableWidget
    {
        Image m_Flag = null;

        TezTreeNode m_Node = null;

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {
            m_Node = this.transform.parent.GetComponent<TezTreeNode>();
            m_Flag = this.GetComponent<Image>();

            if (m_Node.isLeaf)
            {
                m_Flag.gameObject.SetActive(false);
            }
        }

        protected override void onHide()
        {

        }

        protected override void onClose(bool self_close)
        {
            m_Node = null;
        }

        public override void reset()
        {

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!m_Node.isLeaf)
            {
                m_Node.foldOrUnfold();
                if (m_Node.isOn)
                {
                    m_Flag.sprite = m_Node.tree.flagOn;
                }
                else
                {
                    m_Flag.sprite = m_Node.tree.flagOff;
                }
            }
        }
    }
}