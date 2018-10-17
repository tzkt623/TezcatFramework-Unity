using tezcat.DataBase;
using tezcat.Event;
using tezcat.Extension;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezBasicItemEditor : TezToolWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezLabelButtonWithBG m_Confirm = null;
        [SerializeField]
        TezLabelButtonWithBG m_Save = null;
        [SerializeField]
        TezLabelButtonWithBG m_Cancel = null;

        public TezAction onClose { get; private set; } = new TezAction();

        public abstract int[] supportCategory { get; }
// 
         public abstract void bind(int category);
//        public abstract void bind(TezItem item);

        protected abstract TezDataBaseItem getItem();

        protected override void initWidget()
        {
            base.initWidget();
            m_Confirm.onClick += confirm;
            m_Save.onClick += save;
            m_Cancel.onClick += cancel;
        }

        public override void clear()
        {
            m_Confirm.onClick -= confirm;
            m_Save.onClick -= save;
            m_Cancel.onClick -= cancel;

            onClose.invoke();
            onClose.clear();
            onClose = null;
            base.clear();
        }

        private void confirm(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!this.getItem().NID.isNullOrEmpty())
                {

                    this.close();
                }
                else
                {
                    this.saveFailed();
                }
            }
        }

        private void save(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!this.getItem().NID.isNullOrEmpty())
                {

                    this.dirty = true;
                }
                else
                {
                    this.saveFailed();
                }
            }
        }

        private void cancel(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.close();
            }
        }

        protected abstract void saveFailed();
    }
}