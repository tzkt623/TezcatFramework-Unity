using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace tezcat.UI
{
    public class TezLocalizationNameItem : TezWidget
    {
        [SerializeField]
        Text m_KeyName = null;
        [SerializeField]
        InputField m_LocalizationName = null;
        [SerializeField]
        TezImageButton m_Edit = null;

        int m_Index = -1;

        protected override void Awake()
        {
            base.Awake();
            m_Edit.onClick += onEditClick;

            m_LocalizationName.readOnly = true;
            m_LocalizationName.onEndEdit.AddListener((string content) =>
            {
                TezLocalization.saveName(m_Index, content);
            });
        }

        private void onEditClick(PointerEventData.InputButton button)
        {
            m_LocalizationName.readOnly = false;
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        public void set(int index)
        {
            m_Index = index;
            this.dirty = true;
        }

        protected override void clear()
        {
            m_KeyName = null;
            m_LocalizationName = null;
        }

        protected override void onRefresh()
        {
            string key = null, value = null;
            if(TezLocalization.getName(m_Index, out key, out value))
            {
                m_KeyName.text = key;
                m_LocalizationName.text = value;
            }
        }
    }
}