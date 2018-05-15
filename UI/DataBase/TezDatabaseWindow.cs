using System.Collections.Generic;
using tezcat.DataBase;
using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezWindow
    {
        [SerializeField]
        GameObject m_RootMenu = null;

        [Header("Prefab")]
        [SerializeField]
        TezItemEditor m_PrefabBaseItemEditor = null;
        [SerializeField]
        TezBaseItemEditor[] m_PrefabEditorArray = null;
        Dictionary<TezDatabase.CategoryType, TezBaseItemEditor> m_EditorDic = new Dictionary<TezDatabase.CategoryType, TezBaseItemEditor>();

        [Header("Area")]
        [SerializeField]
        TezDatabaseMenu m_Menu = null;
        [SerializeField]
        TezDatabaseGroup m_Group = null;
        [SerializeField]
        TezDatabaseItemContainer m_Container = null;

        protected override void initWidget()
        {
            base.initWidget();
            foreach (var item in m_PrefabEditorArray)
            {
                foreach (var category in item.categoryTypes)
                {
                    m_EditorDic.Add(category, item);
                }
            }

            m_PrefabEditorArray = null;

            m_Menu.setGroup(m_Group);
            m_Container.setGroup(m_Group);
            m_Group.setContainer(m_Container);
        }

        protected override void onRefresh()
        {
            base.onRefresh();
        }

        protected override void onHide()
        {
            base.onHide();
            m_RootMenu.SetActive(true);
        }

        public void createItemEditor(TezDatabase.CategoryType category)
        {
            TezBaseItemEditor prefab = null;
            if(m_EditorDic.TryGetValue(category, out prefab))
            {
                var editor = Instantiate(prefab, this.layer.transform, false);
                this.layer.addWindow(editor);

                editor.transform.localPosition = Vector3.zero;
                editor.bind(category);
                editor.open();
            }
            else
            {
                var editor = Instantiate(m_PrefabBaseItemEditor, this.layer.transform, false);
                this.layer.addWindow(editor);

                editor.transform.localPosition = Vector3.zero;
                editor.bind(category);
                editor.open();
            }
        }
    }
}