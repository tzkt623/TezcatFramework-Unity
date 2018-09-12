using tezcat.DataBase;
using tezcat.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezBasicItemEditor : TezToolWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezImageLabelButton m_Confirm = null;
        [SerializeField]
        TezImageLabelButton m_Save = null;
        [SerializeField]
        TezImageLabelButton m_Cancel = null;

        public TezAction onClose { get; private set; } = new TezAction();

        public abstract int[] supportCategory { get; }
// 
         public abstract void bind(int category);
//        public abstract void bind(TezItem item);

        protected abstract TezItem getItem();

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

        private void confirm(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                if (!this.getItem().NID.isNullOrEmpty)
                {
                    if (this.getItem().GUID == -1)
                    {
//                        TezService.DB.registerItem(this.getItem());
                    }

                    this.close();
                }
                else
                {
                    this.saveFailed();
                }
            }
        }

        private void save(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                if (!this.getItem().NID.isNullOrEmpty)
                {
                    if (this.getItem().GUID == -1)
                    {
//                        TezService.DB.registerItem(this.getItem());
                    }

                    this.dirty = true;
                }
                else
                {
                    this.saveFailed();
                }
            }
        }

        private void cancel(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.close();
            }
        }

        protected abstract void saveFailed();
    }
}