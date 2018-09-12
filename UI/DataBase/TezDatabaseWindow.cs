using System.Collections.Generic;
using tezcat.Core;
using tezcat.DataBase;
using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezToolWindow
    {
        [Header("Area")]
        [SerializeField]
        TezDatabaseMenu m_Menu = null;
        [SerializeField]
        TezDatabaseGroup m_Group = null;
        [SerializeField]
        TezDatabaseItemContainer m_Container = null;

//        Dictionary<TezDatabase.Container, TezBasicItemEditor> m_EditorDic = new Dictionary<TezDatabase.Container, TezBasicItemEditor>();
        List<TezBasicItemEditor> m_EditorDic = new List<TezBasicItemEditor>();
        TezBasicItemEditor m_CurrentEditor = null;

        protected override void preInit()
        {
            base.preInit();

            m_Menu.setGroup(m_Group);
            m_Container.setGroup(m_Group);

            m_Menu.setContainer(m_Container);
            m_Group.setContainer(m_Container);
        }

        protected override void initWidget()
        {
            base.initWidget();

            TezPrefabDatabase.foreachPrefab((TezPrefabDatabase.Prefab prefab) =>
            {
                var editor = prefab.prefab as TezBasicItemEditor;
                if (editor)
                {
                    foreach (var category in editor.supportCategory)
                    {
                        while(m_EditorDic.Count <= category)
                        {
                            m_EditorDic.Add(null);
                        }

                        m_EditorDic[category] = editor;
                    }
                }
            });
        }

        public void createItemEditor(int category)
        {
            if(m_CurrentEditor)
            {
                return;
            }

            TezBasicItemEditor prefab = null;
            if(m_EditorDic.Count > category)
            {
                m_CurrentEditor = TezcatFramework.instance.createWindow(prefab, "TezItemEditor", this.layer) as TezBasicItemEditor;
            }
            else
            {
                m_CurrentEditor = TezcatFramework.instance.createWindow<TezItemEditor>("TezItemEditor", this.layer);
            }

//             if (m_EditorDic.TryGetValue(category, out prefab))
//             {
//                 m_CurrentEditor = TezcatFramework.instance.createWindow(prefab, "TezItemEditor", this.layer) as TezBasicItemEditor;
//             }
//             else
//             {
//                 m_CurrentEditor = TezcatFramework.instance.createWindow<TezItemEditor>("TezItemEditor", this.layer);
//             }

            m_CurrentEditor.transform.SetAsLastSibling();
            m_CurrentEditor.onClose.add(this.onEditorClose);
            m_CurrentEditor.bind(category);
            m_CurrentEditor.open();
        }

        public void editItem(TezItem item)
        {
//             if (m_CurrentEditor)
//             {
//                 return;
//             }
// 
//             TezBasicItemEditor prefab = null;
//             if (m_EditorDic.TryGetValue(item.categoryType, out prefab))
//             {
//                 m_CurrentEditor = TezcatFramework.instance.createWindow(prefab, "TezItemEditor", this.layer) as TezBasicItemEditor;
//             }
//             else
//             {
//                 m_CurrentEditor = TezcatFramework.instance.createWindow<TezItemEditor>("TezItemEditor", this.layer);
//             }
// 
//             m_CurrentEditor.transform.SetAsLastSibling();
//             m_CurrentEditor.onClose.add(this.onEditorClose);
//             m_CurrentEditor.bind(item);
//             m_CurrentEditor.open();
        }

        private void onEditorClose()
        {
            m_CurrentEditor = null;
        }

        public override void clear()
        {
            base.clear();
            TezcatFramework.instance.createWindow<TezcatToolWindow>("TezcatToolWindow", TezLayer.last).open();
        }
    }
}