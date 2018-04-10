using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using tezcat.UI;

namespace tezcat.DataBase
{
    public class TezDatabaseWindow
        : TezWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezImageLabelButton m_AddGroup = null;
        [SerializeField]
        TezImageLabelButton m_AddType = null;
        [SerializeField]
        TezImageLabelButton m_AddItem = null;

        [Header("New Group")]
        [SerializeField]
        GameObject m_NewGroup = null;
        [SerializeField]
        TezImageLabelButton m_NewGroupConfirm = null;
        [SerializeField]
        TezImageLabelButton m_NewGroupCancel = null;
        [SerializeField]
        InputField m_NewGroupInput = null;

        [Header("Item List")]
        [SerializeField]
        TezTree m_Tree = null;

        protected override void Start()
        {
            base.Start();
            m_Tree.onSelectNode += onSelectNode;

            m_AddGroup.onClick += onAddGroup;

            m_NewGroupConfirm.onClick += onNewGroupConfirm;
            m_NewGroupCancel.onClick += onNewGroupCancel;
        }

        private void onSelectNode(TezTreeNode node)
        {
            Debug.Log(node.text);
        }

        private void onAddGroup(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                m_NewGroupInput.text = "";
                m_NewGroup.SetActive(true);
            }
        }

        private void onNewGroupCancel(PointerEventData.InputButton button)
        {
            m_NewGroup.SetActive(false);
            m_NewGroupInput.text = "";
        }

        private void onNewGroupConfirm(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                if (string.IsNullOrEmpty(m_NewGroupInput.text))
                {
                    return;
                }

                m_NewGroup.SetActive(false);
                m_Tree.addData(new TezTreeData(m_NewGroupInput.text));
            }
        }

        protected override void onRefresh()
        {
            base.onRefresh();
        }

        public override void clear()
        {

        }
    }
}