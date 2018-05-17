using tezcat.DataBase;
using tezcat.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezBasicItemEditor : TezWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezImageLabelButton m_Confirm = null;
        [SerializeField]
        TezImageLabelButton m_Save = null;
        [SerializeField]
        TezImageLabelButton m_Cancel = null;

        public TezEvent onClose { get; private set; } = new TezEvent();

        public abstract TezDatabase.CategoryType[] categoryTypes { get; }

        public abstract void bind(TezDatabase.CategoryType category_type);

        protected abstract TezItem getItem();

        protected override void initWidget()
        {
            base.initWidget();
            m_Confirm.onClick += confirm;
            m_Save.onClick += save;
            m_Cancel.onClick += cancel;
        }

        protected override void clear()
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
                        TezDatabase.registerInnateItem(this.getItem());
                    }

                    this.close();
                }
                else
                {
                    ///名称不能为空
                }
            }
        }

        private void save(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                if (!this.getItem().NID.isNullOrEmpty)
                {
                    if(this.getItem().GUID == -1)
                    {
                        TezDatabase.registerInnateItem(this.getItem());
                    }

                    this.dirty = true;
                }
                else
                {
                    ///名称不能为空
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
    }
}