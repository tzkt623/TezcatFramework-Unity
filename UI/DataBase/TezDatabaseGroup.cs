using tezcat.DataBase;
using tezcat.TypeTraits;
using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseGroup
        : TezArea
    {
        [SerializeField]
        TezTree m_Tree = null;

        [SerializeField]
        RectTransform m_Vernier = null;

        TezTreeNode m_SelectNode = null;

        class NodeData : TezTreeData
        {
            public TezType dataType { get; private set; }

            public NodeData(TezType e) : base(e.name)
            {
                dataType = e;
            }
        }

        TezDatabaseItemContainer m_Container = null;

        protected override void Start()
        {
            base.Start();
            m_Tree.onSelectNode += onSelectNode;
            m_Container = window.getArea<TezDatabaseItemContainer>();
            this.dirty = true;
        }

        protected override void onRefresh()
        {
            m_Tree.reset();

            TezTreeNode current_group = null;
            TezTreeNode current_type = null;
            TezDatabase.instance.foreachItemByGroup(

                (TezDatabase.GroupType group) =>
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

                (TezDatabase.CategoryType type) =>
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

                (TezItem item) =>
                {

                });
        }

        private void onSelectNode(TezTreeNode node)
        {
            m_Container.reset();

            if (node.parent != null)
            {
                m_SelectNode = node;
                m_Vernier.gameObject.SetActive(true);
                m_Vernier.SetParent(m_SelectNode.transform, false);
                TezUILayout.setLayout(m_Vernier, -2, -2, 2, 2);

                var group = node.parent.data as NodeData;
                var type = node.data as NodeData;

                ((TezDatabaseWindow)this.window).selectGroup = group.dataType as TezDatabase.GroupType;
                ((TezDatabaseWindow)this.window).selectCategory = type.dataType as TezDatabase.CategoryType;

                m_Container.loadItems(group.dataType, type.dataType);
            }
        }
    }
}

