using tezcat.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
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

        }

        protected override void linkEvent()
        {
            m_Input.onEndEdit.AddListener(this.onValueSet);
        }

        protected override void unLinkEvent()
        {
            m_Input.onEndEdit.RemoveListener(this.onValueSet);
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        private void onValueSet(string value)
        {
            switch (m_Property.getParameterType())
            {
                case TezPropertyType.Float:
                    (m_Property as TezPV_Float).value = float.Parse(value);
                    break;
                case TezPropertyType.Int:
                    (m_Property as TezPV_Int).value = int.Parse(value);
                    break;
                case TezPropertyType.String:
                    (m_Property as TezPV_String).value = value;
                    break;
                case TezPropertyType.StaticString:
                    (m_Property as TezPV_StaticString).value = value;
                    break;
            }
        }

        protected override void onRefresh()
        {
            m_PropertyName.text = TezLocalization.getName(m_Property.name, m_Property.name);
            switch (m_Property.getParameterType())
            {
                case TezPropertyType.Float:
                    m_Input.contentType = InputField.ContentType.DecimalNumber;
                    m_Input.text = (m_Property as TezPV_Float).value.ToString();
                    break;
                case TezPropertyType.Int:
                    m_Input.contentType = InputField.ContentType.IntegerNumber;
                    m_Input.text = (m_Property as TezPV_Int).value.ToString();
                    break;
                case TezPropertyType.String:
                    m_Input.contentType = InputField.ContentType.Standard;
                    m_Input.text = (m_Property as TezPV_String).value;
                    break;
                case TezPropertyType.StaticString:
                    m_Input.contentType = InputField.ContentType.Standard;
                    m_Input.text = (m_Property as TezPV_StaticString).value;
                    break;
            }
        }
    }
}