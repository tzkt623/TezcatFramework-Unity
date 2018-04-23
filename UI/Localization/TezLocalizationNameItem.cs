using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace tezcat.UI
{
    public class TezLocalizationNameItem
        : TezWidget
        , ITezClickable
    {
        [SerializeField]
        Text m_KeyName = null;
        [SerializeField]
        Text m_LocalizationName = null;
        [SerializeField]
        TezImageButton m_Edit = null;

        public TezLocalizationNameList listArea { get; set; }

        public int index { get; private set; } = -1;

        protected override void Awake()
        {
            base.Awake();
            m_Edit.onClick += onEditClick;
        }

        private void onEditClick(PointerEventData.InputButton button)
        {
            listArea.edit(index);
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
            m_KeyName = null;
            m_LocalizationName = null;
            this.listArea = null;
        }

        protected override void onRefresh()
        {
            string key = null, value = null;
            if(TezLocalization.getName(index, out key, out value))
            {
                m_KeyName.text = key;
                m_LocalizationName.text = value;
            }
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