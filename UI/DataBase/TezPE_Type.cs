using System.Collections.Generic;
using tezcat.TypeTraits;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezPE_Type : TezPropertyEditor
    {
        [SerializeField]
        TezText m_PropertyName = null;
        [SerializeField]
        Dropdown m_DropDown = null;

        List<TezType> m_Types = null;

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {
            m_DropDown.onValueChanged.AddListener(this.onValueChanged);
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        public override void reset()
        {

        }

        protected override void clear()
        {
            m_Types.Clear();
            m_Types = null;
            m_DropDown.onValueChanged.RemoveListener(this.onValueChanged);
        }

        protected override void onRefresh()
        {
            m_PropertyName.text = TezLocalization.getName(m_Property.name, m_Property.name);
            m_Types = TezTypeRegisterHelper.getList(m_Property.propertyType);
            m_DropDown.options.Clear();
            for (int i = 0; i < m_Types.Count; i++)
            {
                m_DropDown.options.Add(new Dropdown.OptionData(m_Types[i].name));
            }
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        private void onValueChanged(int value)
        {
            ((TezPV_Type)m_Property).baseValue = m_Types[value];
        }
    }
}

