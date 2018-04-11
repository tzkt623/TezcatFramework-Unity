using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezStorageSlot
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
        , IDropHandler
        , IDragHandler
        , IBeginDragHandler
        , IEndDragHandler
        , IPointerClickHandler
    {


        public override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}