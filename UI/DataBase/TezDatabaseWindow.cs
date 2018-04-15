using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using tezcat.UI;
using tezcat.Utility;

namespace tezcat.DataBase
{
    public class TezDatabaseWindow : TezWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezImageLabelButton m_AddGroup = null;
        [SerializeField]
        TezImageLabelButton m_AddType = null;
        [SerializeField]
        TezImageLabelButton m_AddItem = null;
        [SerializeField]
        TezImageLabelButton m_RefreshDataBase = null;

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

        [Header("Item Content")]
        [SerializeField]
        RectTransform m_Content = null;

        [Header("Item Pool")]
        RectTransform m_Pool = null;


        class NodeData : TezTreeData
        {
            public TezEnum tenum { get; private set; }

            public NodeData(TezEnum e) : base(e.name)
            {
                tenum = e;
            }
        }

        protected override void Start()
        {
            base.Start();
            m_Tree.onSelectNode += onSelectNode;

            m_AddGroup.onClick += onAddGroup;
            m_RefreshDataBase.onClick += onRefreshDataBase;

            m_NewGroupConfirm.onClick += onNewGroupConfirm;
            m_NewGroupCancel.onClick += onNewGroupCancel;


            this.onRefreshDataBase();
        }

        private void onSelectNode(TezTreeNode node)
        {
            foreach (RectTransform child in m_Content)
            {
                Destroy(child.gameObject);
            }

            if (node.parent != null)
            {
                var group = node.parent.data as NodeData;
                var type = node.data as NodeData;

                List<ITezItem> items = null;
                TezDatabase.instance.tryGetItems(group.tenum.ID, type.tenum.ID, out items);

                for (int i = 0; i < items.Count; i++)
                {
                    GameObject go = new GameObject();
                    go.AddComponent<Image>();
                    go.transform.SetParent(m_Content, false);
                }
            }
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

        private void onRefreshDataBase()
        {
            TezTreeNode current_group = null;
            TezTreeNode current_type = null;
            TezDatabase.instance.foreachItemByGroup(
                (TezDatabase.GroupEnum group) =>
                {
                    if (group == null)
                    {
                        return;
                    }

                    current_group = m_Tree.addData(new NodeData(group));
#if UNITY_EDITOR
                    TezDebug.info("TezDatabaseWindow", "Add Group : " + group.name);
#endif
                },

                (TezDatabase.TypeEnum type) =>
                {
                    if (type == null)
                    {
                        return;
                    }

                    current_type = current_group.addData(new NodeData(type));
#if UNITY_EDITOR
                    TezDebug.info("TezDatabaseWindow", "Add Type : " + type.name);
#endif
                },

                (ITezItem item) =>
                {

                });
        }

        private void onRefreshDataBase(PointerEventData.InputButton button)
        {
            this.onRefreshDataBase();
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