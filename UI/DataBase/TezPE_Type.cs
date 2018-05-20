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

        }

        protected override void linkEvent()
        {
            m_DropDown.onValueChanged.AddListener(this.onValueChanged);
        }

        protected override void unLinkEvent()
        {
            m_DropDown.onValueChanged.RemoveListener(this.onValueChanged);
        }

        public override void reset()
        {

        }

        public override void clear()
        {
            m_Types.Clear();
            m_Types = null;
        }

        protected override void onRefresh()
        {
            m_PropertyName.text = TezLocalization.getName(m_Property.name, m_Property.name);
            m_Types = TezTypeRegisterHelper.getList(m_Property.propertyType);

            m_DropDown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i = 0; i < m_Types.Count; i++)
            {
                options.Add(new Dropdown.OptionData(m_Types[i].name));
            }
            m_DropDown.AddOptions(options);

            m_DropDown.value = ((TezPV_Type)m_Property).baseValue.ID;
            m_DropDown.captionText.text = ((TezPV_Type)m_Property).baseValue.name;
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

