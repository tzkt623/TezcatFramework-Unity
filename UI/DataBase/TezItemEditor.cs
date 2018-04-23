using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezItemEditor : TezPopup
    {
        [SerializeField]
        TezPropertyEditor m_Prefab = null;

        [SerializeField]
        TezImageLabelButton m_Confirm = null;
        [SerializeField]
        TezImageLabelButton m_Cancel = null;

        [SerializeField]
        RectTransform m_Content = null;

        TezItem m_NewItem = null;

        protected override void Awake()
        {
            base.Awake();

            m_Confirm.onClick += onConfirmClick;
            m_Cancel.onClick += onCancelClick;
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        private void onCancelClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                this.close();
            }
        }

        private void onConfirmClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {

            }
        }

        public void bind(TezItem item)
        {
            m_NewItem = item;
            this.dirty = true;
        }

        TezPropertyEditor create(TezPropertyType type, TezPropertyValue property)
        {
            var editor = Instantiate(m_Prefab, m_Content, false);
            editor.setInfo(
                TezLocalization.getName(property.propertyName.key_name, property.propertyName.key_name),
                TezPropertyType.Float);

            return editor;
        }

        protected override void onRefresh()
        {
            if(m_NewItem != null)
            {
                var properties = m_NewItem.properties;

                for (int i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    TezPropertyEditor editor = null;
                    switch (property.getParameterType())
                    {
                        case TezPropertyType.Bool:

                            break;
                        case TezPropertyType.Float:
                            editor = Instantiate(m_Prefab, m_Content, false);
                            editor.setInfo(
                                TezLocalization.getName(property.propertyName.key_name, property.propertyName.key_name),
                                TezPropertyType.Float);
                            break;
                        case TezPropertyType.Int:
                            editor = Instantiate(m_Prefab, m_Content, false);
                            editor.setInfo(
                                TezLocalization.getName(property.propertyName.key_name, property.propertyName.key_name),
                                TezPropertyType.Int);
                            break;
                        case TezPropertyType.String:
                            editor = Instantiate(m_Prefab, m_Content, false);
                            editor.setInfo(
                                TezLocalization.getName(property.propertyName.key_name, property.propertyName.key_name),
                                TezPropertyType.String);
                            break;
                    }

                    if(editor != null)
                    {
                        editor.open();
                    }
                }
            }
        }
    }
}

