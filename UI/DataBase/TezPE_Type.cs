using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.TypeTraits;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
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

        public override void reset()
        {

        }

        protected override void onClose(bool self_close = true)
        {
            m_DropDown.onValueChanged.RemoveListener(this.onValueChanged);
            m_Types.Clear();
            m_Types = null;
        }

        protected override void onRefresh()
        {
            this.refreshData();
        }

        private void refreshData()
        {
            m_PropertyName.text = TezService.get<TezTranslator>().translateName(m_Property.name, m_Property.name);
            m_Types = TezTypeListManager.getList(m_Property.systemType);

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

        protected override void onHide()
        {

        }

        private void onValueChanged(int value)
        {
            ((TezPV_Type)m_Property).baseValue = m_Types[value];
        }
    }
}

