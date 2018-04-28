using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezTitle
        : TezWidget
        , IPointerEnterHandler
        , IPointerExitHandler
        , IDragHandler
    {
        [SerializeField]
        Text m_TitleName = null;
        [SerializeField]
        Button m_CloseButton = null;
        [SerializeField]
        Button m_HideButton = null;
        [SerializeField]
        Toggle m_PinToggle = null;

        TezWidget m_ParenWidget = null;
        bool m_Pin = false;

        protected override void Start()
        {
            base.Start();

            m_ParenWidget = this.transform.parent.GetComponent<TezWidget>();
            if (m_ParenWidget == null)
            {
                throw new ArgumentNullException("ParenWidget Not Found");
            }

            if (m_TitleName == null)
            {
                m_TitleName = this.GetComponentInChildren<Text>();
            }

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

        protected override void onRefresh()
        {

        }

        protected override void clear()
        {
            m_ParenWidget = null;
        }

        void closeParent()
        {
            m_ParenWidget.close();
        }

        void hideParent()
        {
            m_ParenWidget.hide();
        }

        void pin(bool pin)
        {
            m_Pin = pin;
        }

        public void setName(string name)
        {
            m_TitleName.text = name;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (m_Pin)
            {
                return;
            }

            var offset = eventData.delta;
            m_ParenWidget.transform.localPosition += new Vector3(offset.x, offset.y, 0);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }
    }
}