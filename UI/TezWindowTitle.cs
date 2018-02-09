﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace tezcat
{
    public class TezWindowTitle
        : TezUIObjectMB
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

        TezWindow m_Window = null;
        bool m_Pin = false;

        protected override void onAwake()
        {

        }

        protected override void onStart()
        {
            if(m_TitleName == null)
            {
                m_TitleName = this.GetComponentInChildren<Text>();
            }

            if (m_Window == null)
            {
                m_Window = this.GetComponentInParent<TezWindow>();
            }

            if (m_CloseButton)
            {
                m_CloseButton.onClick.AddListener(close);
            }

            if (m_HideButton)
            {
                m_HideButton.onClick.AddListener(hide);
            }

            if (m_PinToggle)
            {
                m_PinToggle.isOn = m_Pin;
                m_PinToggle.onValueChanged.AddListener(pin);
            }
        }

        protected override void onDelete()
        {

        }

        protected override void onDisable()
        {

        }

        protected override void onEnable()
        {

        }

        void close()
        {
            m_Window.close();
        }

        void hide()
        {
            m_Window.close();
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
            if(m_Pin)
            {
                return;
            }

            var offset = eventData.delta;
            m_Window.transform.localPosition += new Vector3(offset.x, offset.y, 0);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }
    }
}