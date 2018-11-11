﻿using System;
using tezcat.Framework.Extension;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public class TezNewTitle
        : TezUIWidget
        , ITezDragableWidget
        , ITezClickable
    {
        TezWindow m_Window = null;
        bool m_Dragging = false;

        protected override void initWidget()
        {
            m_Window = this.transform.parent.GetComponent<TezWindow>();
            if (m_Window == null)
            {
                throw new ArgumentNullException("ParenWidget Not Found");
            }
        }

        protected override void onRefresh(RefreshPhase phase)
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
            if (!m_Dragging)
            {
                return;
            }

            m_Window.transform.localPosition = m_Window.transform.localPosition.add(eventData.delta);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m_Window.setAsTopWindow();
            }
        }
    }
}