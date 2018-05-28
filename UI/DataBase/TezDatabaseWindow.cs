﻿using System.Collections.Generic;
using tezcat.DataBase;
using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezWindow
    {
        [Header("Prefab")]
        [SerializeField]
        TezItemEditor m_PrefabItemEditor = null;

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

        Dictionary<TezDatabase.CategoryType, TezBasicItemEditor> m_EditorDic = new Dictionary<TezDatabase.CategoryType, TezBasicItemEditor>();
        TezBasicItemEditor m_CurrentEditor = null;

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
                        m_EditorDic.Add(category, editor);
                    }
                }
            });

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
                m_CurrentEditor = Instantiate(m_PrefabItemEditor, this.layer.transform, false);
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
                m_CurrentEditor = Instantiate(m_PrefabItemEditor, this.layer.transform, false);
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