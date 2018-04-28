using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezPropertyView : TezWidget
    {
        [SerializeField]
        Text m_PropertyName = null;
        [SerializeField]
        Text m_PorpertyValue = null;

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        public void setInfo(string name, string value)
        {
            m_PropertyName.text = name;
            m_PorpertyValue.text = value;
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}