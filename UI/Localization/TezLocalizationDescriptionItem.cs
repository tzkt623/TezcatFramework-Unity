using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using tezcat.Utility;
namespace tezcat.UI
{
    public class TezLocalizationDescriptionItem
        : TezWidget
        , IPointerEnterHandler
        , IPointerExitHandler
        , ITezClickable
    {
        [SerializeField]
        TezImageButton m_Edit = null;

        public TezLocalizationDescriptionList listArea { get; set; }

        Image m_Flag = null;
        Text m_KeyName = null;
        public int index { get; private set; } = -1;

        protected override void Awake()
        {
            base.Awake();
            m_Flag = this.GetComponentInChildren<Image>();
            m_KeyName = this.GetComponentInChildren<Text>();

            m_Edit.onClick += onEditClick;
        }

        private void onEditClick(PointerEventData.InputButton button)
        {
            listArea.editItem(index);
        }

        protected override void Start()
        {
            base.Start();

            this.dirty = true;
        }

        public void set(int index)
        {
            this.index = index;
            this.dirty = true;
        }

        protected override void clear()
        {
            m_Flag = null;
            m_KeyName = null;
            listArea = null;
        }

        protected override void onRefresh()
        {
            string key = null, value = null;
            if (TezLocalization.getDescription(index, out key, out value))
            {
                m_KeyName.text = key;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            string key = null, value = null;
            if (TezLocalization.getDescription(index, out key, out value))
            {
                TezTipManager.instance
                    .setDescription(value)
                    .show();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            TezTipManager.instance.hide();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.listArea.onFocus(this);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}