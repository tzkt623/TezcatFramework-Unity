using System.Collections.Generic;
using tezcat.DataBase;
using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezWindow
    {
        [Header("Prefab")]
        [SerializeField]
        TezItemEditor m_PrefabBaseItemEditor = null;
        [SerializeField]
        TezBasicItemEditor[] m_PrefabEditorArray = null;
        Dictionary<TezDatabase.CategoryType, TezBasicItemEditor> m_EditorDic = new Dictionary<TezDatabase.CategoryType, TezBasicItemEditor>();

        [Header("Root Menu")]
        [SerializeField]
        GameObject m_RootMenu = null;

        [Header("Area")]
        [SerializeField]
        TezDatabaseMenu m_Menu = null;
        [SerializeField]
        TezDatabaseGroup m_Group = null;
        [SerializeField]
        TezDatabaseItemContainer m_Container = null;

        TezBasicItemEditor m_CurrentEditor = null;

        protected override void initWidget()
        {
            base.initWidget();
            foreach (var item in m_PrefabEditorArray)
            {
                foreach (var category in item.supportCategory)
                {
                    m_EditorDic.Add(category, item);
                }
            }

            m_PrefabEditorArray = null;

            m_Menu.setGroup(m_Group);
            m_Menu.setContainer(m_Container);

            m_Container.setGroup(m_Group);

            m_Group.setContainer(m_Container);
        }

        protected override void onHide()
        {
            base.onHide();
            m_RootMenu.SetActive(true);
        }

        public void createItemEditor(TezDatabase.CategoryType category)
        {
            if(m_CurrentEditor)
            {
                return;
            }

            TezBasicItemEditor prefab = null;
            if(m_EditorDic.TryGetValue(category, out prefab))
            {
                m_CurrentEditor = Instantiate(prefab, this.layer.transform, false);
            }
            else
            {
                m_CurrentEditor = Instantiate(m_PrefabBaseItemEditor, this.layer.transform, false);
            }
            this.layer.addWindow(m_CurrentEditor);

            m_CurrentEditor.transform.localPosition = Vector3.zero;
            m_CurrentEditor.onClose.add(this.onEditorClose);
            m_CurrentEditor.bind(category);
            m_CurrentEditor.open();
        }

        public void editItem(TezItem item)
        {
            if (m_CurrentEditor)
            {
                return;
            }

            TezBasicItemEditor prefab = null;
            if (m_EditorDic.TryGetValue(item.categoryType, out prefab))
            {
                m_CurrentEditor = Instantiate(prefab, this.layer.transform, false);
            }
            else
            {
                m_CurrentEditor = Instantiate(m_PrefabBaseItemEditor, this.layer.transform, false);
            }
            this.layer.addWindow(m_CurrentEditor);

            m_CurrentEditor.transform.localPosition = Vector3.zero;
            m_CurrentEditor.onClose.add(this.onEditorClose);
            m_CurrentEditor.bind(item);
            m_CurrentEditor.open();
        }

        private void onEditorClose()
        {
            m_CurrentEditor = null;
        }
    }
}