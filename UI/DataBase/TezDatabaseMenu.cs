using tezcat.DataBase;
using tezcat.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezDatabaseMenu : TezArea
    {
        [SerializeField]
        TezImageLabelButton m_AddItem = null;
        [SerializeField]
        TezImageLabelButton m_RemoveItem = null;
        [SerializeField]
        TezImageLabelButton m_EditItem = null;

        [SerializeField]
        TezImageLabelButton m_Save = null;
        [SerializeField]
        TezImageLabelButton m_RefreshDataBase = null;

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

        private void onAddItem(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                if (m_Group.categoryType != null)
                {
                    window.createItemEditor(m_Group.categoryType);
                }
            }
        }

        private void onRemoveItem(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                m_Container.removeItem();
            }
        }

        private void onEditItem(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                if(m_Container.currentItem != null)
                {
                    window.editItem(m_Container.currentItem);
                }
            }
        }

        private void onSave(PointerEventData.InputButton button)
        {
            TezJsonWriter writer = new TezJsonWriter(true);
            TezDatabase.sortItems();
            TezDatabase.foreachItemByGUID((TezItem item) =>
            {
                if(item)
                {
                    writer.beginObject(item.GUID);
                    item.serialization(writer);
                    writer.endObject(item.GUID);
                }
            });
            writer.save(TezcatGameEngine.rootPath + TezcatGameEngine.databaseFile);
        }

        private void onRefreshDataBase(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                m_Group.dirty = true;
            }
        }

        protected override void onRefresh()
        {

        }

        public override void clear()
        {
            m_Save.onClick -= onSave;
            m_AddItem.onClick -= onAddItem;
            m_RefreshDataBase.onClick -= onRefreshDataBase;

            base.clear();
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }
    }
}