using System;
using tezcat.Extension;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezTitle
        : TezWidget
        , ITezDragableWidget
        , ITezClickable
    {
        [SerializeField]
        Text m_TitleName = null;
        [SerializeField]
        Button m_CloseButton = null;
        [SerializeField]
        Button m_HideButton = null;
        [SerializeField]
        Toggle m_PinToggle = null;

        TezBasicWidget m_ParentWidget = null;
        bool m_Pin = false;
        bool m_Dragging = false;

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {
            m_ParentWidget = this.transform.parent.GetComponent<TezBasicWidget>();
            if (m_ParentWidget == null)
            {
                throw new ArgumentNullException("ParenWidget Not Found");
            }

            if (m_TitleName == null)
            {
                m_TitleName = this.GetComponentInChildren<Text>();
            }
        }

        protected override void linkEvent()
        {
            if (m_CloseButton)
            {
                m_CloseButton.onClick.AddListener(this.closeParent);
            }

            if (m_HideButton)
            {
                m_HideButton.onClick.AddListener(this.hideParent);
            }

            if (m_PinToggle)
            {
                m_PinToggle.isOn = m_Pin;
                m_PinToggle.onValueChanged.AddListener(pin);
            }
        }

        protected override void unLinkEvent()
        {
            if (m_CloseButton)
            {
                m_CloseButton.onClick.RemoveListener(this.closeParent);
            }

            if (m_HideButton)
            {
                m_HideButton.onClick.RemoveListener(this.hideParent);
            }

            if (m_PinToggle)
            {
                m_PinToggle.onValueChanged.RemoveListener(pin);
            }
        }

        public override void reset()
        {

        }

        protected override void onRefresh()
        {

        }

        public override void clear()
        {
            m_ParentWidget = null;
        }

        void closeParent()
        {
            m_ParentWidget.close();
        }

        void hideParent()
        {
            m_ParentWidget.hide();
        }

        void pin(bool pin)
        {
            m_Pin = pin;
        }

        public void setName(string name)
        {
            m_TitleName.text = name;
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m_Dragging = true;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            m_Dragging = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (m_Pin || !m_Dragging)
            {
                return;
            }

            m_ParentWidget.transform.localPosition = m_ParentWidget.transform.localPosition.add(eventData.delta);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (m_ParentWidget.transform.childCount != this.transform.GetSiblingIndex())
                {
                    m_ParentWidget.transform.SetAsLastSibling();
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}