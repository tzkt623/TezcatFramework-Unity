using tezcat.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezPropertyEditor : TezWidget
    {
        [SerializeField]
        Text m_PropertyName = null;
        [SerializeField]
        InputField m_Input = null;

        public void setInfo(string name, TezPropertyType type)
        {
            m_PropertyName.text = name;
            switch (type)
            {
                case TezPropertyType.Float:
                    m_Input.contentType = InputField.ContentType.DecimalNumber;
                    break;
                case TezPropertyType.Int:
                    m_Input.contentType = InputField.ContentType.IntegerNumber;
                    break;
                case TezPropertyType.Bool:
                    break;
                case TezPropertyType.String:
                    m_Input.contentType = InputField.ContentType.Standard;
                    break;
                default:
                    break;
            }
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}