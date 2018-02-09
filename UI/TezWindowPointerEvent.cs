using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat
{
    public class TezWindowPointerEvent
        : MonoBehaviour
        , IPointerDownHandler
        , IPointerUpHandler
    {
        [SerializeField]
        TezWindow m_Window = null;

        private void Start()
        {
            if(m_Window == null)
            {
                m_Window = this.GetComponentInParent<TezWindow>();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (m_Window.currentPointer != null)
            {
                m_Window.currentPointer.onPointerDown(eventData);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (m_Window.currentPointer != null)
            {
                m_Window.currentPointer.onPointerUp(eventData);
            }
        }
    }
}