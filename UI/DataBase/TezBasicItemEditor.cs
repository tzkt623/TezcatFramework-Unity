using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
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

        public event TezEventExtension.Action onEventClose;

        public abstract int[] supportCategory { get; }
// 
         public abstract void bind(int category);
//        public abstract void bind(TezItem item);

        protected abstract TezDatabaseItem getItem();

        protected override void initWidget()
        {
            base.initWidget();
            m_Confirm.onClick += confirm;
            m_Save.onClick += save;
            m_Cancel.onClick += cancel;
        }

        protected override void onClose()
        {
            m_Confirm.onClick -= confirm;
            m_Save.onClick -= save;
            m_Cancel.onClick -= cancel;

            onEventClose = null;
            base.onClose();
        }

        private void confirm(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!this.getItem().NID.isNullOrEmpty())
                {

                    this.onClose();
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

                    this.refreshPhase = TezRefreshPhase.P_Custom1;
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
                this.onClose();
            }
        }

        protected abstract void saveFailed();
    }
}