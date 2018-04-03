using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezWindowPointerEvent
        : MonoBehaviour
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
    }
}