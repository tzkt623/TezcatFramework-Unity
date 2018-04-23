using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationNameEditor : TezPopup
    {
        [SerializeField]
        InputField m_Key = null;
        [SerializeField]
        InputField m_Localization = null;

        [SerializeField]
        TezImageLabelButton m_Confirm = null;
        [SerializeField]
        TezImageLabelButton m_Cancel = null;

        int m_Index = -1;

        protected override void Awake()
        {
            base.Awake();

            m_Confirm.onClick += onConfirmClick;
            m_Cancel.onClick += onCancelClick;

            m_Key.onEndEdit.AddListener(this.checkKey);
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        private void checkKey(string key)
        {
            string value;
            if (TezLocalization.getName(key, out value, out m_Index))
            {
                m_Localization.text = value;
            }
            else
            {
                m_Index = TezLocalization.addName(key, value);
                this.dirty = true;
            }
        }


        public void set(int index)
        {
            m_Key.readOnly = true;
            m_Index = index;
            this.dirty = true;
        }

        public void newItem()
        {
            m_Key.readOnly = false;
        }

        private void onCancelClick(PointerEventData.InputButton button)
        {
            this.close();
        }

        private void onConfirmClick(PointerEventData.InputButton button)
        {
            TezLocalization.saveName(m_Index, m_Localization.text);
            this.close();
        }

        protected override void onRefresh()
        {
            if (m_Index != -1)
            {
                string key, value;
                if (TezLocalization.getName(m_Index, out key, out value))
                {
                    m_Key.text = key;
                    m_Localization.text = value;
                }
            }
        }
    }
}