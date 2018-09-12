﻿using tezcat.TypeTraits;
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

        TezDatabaseItemContainer m_Container = null;
        TezTreeNode m_SelectNode = null;

        public int categoryType { get; set; }

        class NodeData : TezTreeData
        {
            public TezType dataType { get; private set; }

            public NodeData(TezType e) : base(e.name)
            {
                dataType = e;
            }
        }

        protected override void initWidget()
        {
            base.initWidget();
            m_Tree.onSelectNode += onSelectNode;
        }

        public override void clear()
        {
            base.clear();
            m_Tree.onSelectNode -= onSelectNode;
        }

        protected override void onRefresh()
        {
            if (m_Vernier.gameObject.activeSelf)
            {
                m_Vernier.SetParent(this.transform, false);
                m_Vernier.gameObject.SetActive(false);
            }

            m_Tree.reset();

            TezTreeNode current_group = null;
            TezTreeNode current_type = null;
//             TezService.get<TezDatabase>().foreachData(
// 
//                 (TezDatabase.Group group) =>
//                 {
//                     if (group == null)
//                     {
//                         return;
//                     }
// 
//                     current_group = m_Tree.addData(new NodeData(group));
// #if UNITY_EDITOR
//                     TezService.debug.info("TezDatabaseWindow", "Add Group : " + group.name);
// #endif
//                 },
// 
//                 (TezDatabase.Container type) =>
//                 {
//                     if (type == null)
//                     {
//                         return;
//                     }
// 
//                     current_type = current_group.addData(new NodeData(type));
// #if UNITY_EDITOR
//                     TezService.debug.info("TezDatabaseWindow", "Add Type : " + type.name);
// #endif
//                 });
        }

        public void setContainer(TezDatabaseItemContainer container)
        {
            m_Container = container;
        }

        private void onSelectNode(TezTreeNode node)
        {
            if (node.parent != null)
            {
                m_SelectNode = node;
                m_Vernier.gameObject.SetActive(true);
                m_Vernier.SetParent(m_SelectNode.transform, false);
                TezLayout.setLayout(m_Vernier, -2, -2, 2, 2);

                var group = node.parent.data as NodeData;
                var type = node.data as NodeData;


            }
            else
            {

            }

            m_Container.dirty = true;
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }
    }
}

