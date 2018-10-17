using tezcat.Wrapper;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezStorageSlot<T>
        : TezUIWidget
        , ITezFocusableWidget
        , ITezClickable
        where T : ITezItemWrapper
    {
        TezStorageItem m_StorageItem = null;
        public T wrapper { get; protected set; }

        protected override void preInit()
        {
            m_StorageItem = this.GetComponent<TezStorageItem>();
        }

        public override void clear()
        {
            this.wrapper.close();
            this.wrapper = default(T);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }

        public abstract void OnPointerUp(PointerEventData eventData);
        public abstract void OnPointerDown(PointerEventData eventData);
    }
}