﻿using tezcat.Framework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public class TezDatabaseMenu : TezSubwindow
    {
        [SerializeField]
        TezButton m_AddItem = null;
        [SerializeField]
        TezButton m_RemoveItem = null;
        [SerializeField]
        TezButton m_EditItem = null;

        [SerializeField]
        TezButton m_Save = null;
        [SerializeField]
        TezButton m_RefreshDataBase = null;

        TezDatabaseGroup m_Group = null;
        new TezDatabaseWindow window
        {
            get { return (TezDatabaseWindow)base.window; }
        }
        TezDatabaseItemContainer m_Container = null;

        protected override void initWidget()
        {
            base.initWidget();

            m_AddItem.onClick += onAddItem;
            m_RemoveItem.onClick += onRemoveItem;
            m_EditItem.onClick += onEditItem;

            m_Save.onClick += onSave;
            m_RefreshDataBase.onClick += onRefreshDataBase;
        }

        public void setGroup(TezDatabaseGroup group)
        {
            m_Group = group;
        }

        public void setContainer(TezDatabaseItemContainer container)
        {
            m_Container = container;
        }

        private void onAddItem(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (m_Group.categoryType != -1)
                {
                    window.createItemEditor(m_Group.categoryType);
                }
            }
        }

        private void onRemoveItem(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m_Container.removeItem();
            }
        }

        private void onEditItem(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (m_Container.currentItem != null)
                {
                    window.editItem(m_Container.currentItem);
                }
            }
        }

        private void onSave(TezButton button, PointerEventData eventData)
        {
            ///TODO:保存数据库

            //             TezJsonWriter writer = new TezJsonWriter(true);
            //             TezService.DB.sortItems();
            //             TezService.DB.foreachItemByGUID((TezItem item) =>
            //             {
            //                 if(item)
            //                 {
            //                     writer.beginObject(item.GUID);
            //                     item.serialize(writer);
            //                     writer.endObject(item.GUID);
            //                 }
            //             });
            //             writer.save(TezcatFramework.rootPath + TezcatFramework.databaseFile);
        }

        private void onRefreshDataBase(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m_Group.refreshPhase = TezRefreshPhase.Refresh;
            }
        }

        protected override void onClose(bool self_close)
        {
            m_Save.onClick -= onSave;
            m_AddItem.onClick -= onAddItem;
            m_RefreshDataBase.onClick -= onRefreshDataBase;

            base.onClose(self_close);
        }

        protected override void onHide()
        {

        }
    }
}