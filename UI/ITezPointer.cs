using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat
{
    public interface ITezPointer
    {
        void onPointerDown(PointerEventData eventData);
        void onPointerUp(PointerEventData eventData);
    }
}