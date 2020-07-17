using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezPE_IFS : TezPropertyEditor
    {
        [SerializeField]
        TezText m_PropertyName = null;
        [SerializeField]
        InputField m_Input = null;

        protected override void preInit()
        {
        }

        protected override void initWidget()
        {
            m_Input.onEndEdit.AddListener(this.onValueSet);
        }

        protected override void onHide()
        {
            m_Input.onEndEdit.RemoveListener(this.onValueSet);
        }

        public override void reset()
        {

        }

        private void onValueSet(string value)
        {
            switch (m_Property.valueType)
            {
                case TezValueType.Float:
                    (m_Property as TezValueWrapper<float>).value = float.Parse(value);
                    break;
                case TezValueType.Int:
                    (m_Property as TezValueWrapper<int>).value = int.Parse(value);
                    break;
                case TezValueType.String:
                    (m_Property as TezValueWrapper<string>).value = value;
                    break;
                case TezValueType.IDString:
                    (m_Property as TezValueWrapper<TezIDString>).value = value;
                    break;
            }
        }

        protected override void onRefresh()
        {
            this.refreshData();
        }

        private void refreshData()
        {
            m_PropertyName.text = TezService.get<TezTranslator>().translateName(m_Property.name, m_Property.name);
            switch (m_Property.valueType)
            {
                case TezValueType.Float:
                    m_Input.contentType = InputField.ContentType.DecimalNumber;
                    m_Input.text = (m_Property as TezValueWrapper<float>).value.ToString();
                    break;
                case TezValueType.Int:
                    m_Input.contentType = InputField.ContentType.IntegerNumber;
                    m_Input.text = (m_Property as TezValueWrapper<int>).value.ToString();
                    break;
                case TezValueType.String:
                    m_Input.contentType = InputField.ContentType.Standard;
                    m_Input.text = (m_Property as TezValueWrapper<string>).value;
                    break;
                case TezValueType.IDString:
                    m_Input.contentType = InputField.ContentType.Standard;
                    m_Input.text = (m_Property as TezValueWrapper<TezIDString>).value;
                    break;
            }
        }
    }
}