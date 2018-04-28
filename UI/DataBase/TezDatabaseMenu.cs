using tezcat.DataBase;
using tezcat.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{

    public class TezDatabaseMenu : TezArea
    {
        [SerializeField]
        TezImageLabelButton m_Save = null;
        [SerializeField]
        TezImageLabelButton m_AddItem = null;
        [SerializeField]
        TezImageLabelButton m_RefreshDataBase = null;

        TezDatabaseGroup m_Group = null;
        TezDatabaseWindow m_Window = null;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            m_Save.onClick += onSave;
            m_AddItem.onClick += onAddItem;
            m_RefreshDataBase.onClick += onRefreshDataBase;
        }

        public void setGroup(TezDatabaseGroup group)
        {
            m_Group = group;
            m_Window = (TezDatabaseWindow)this.window;
        }

        private void onSave(PointerEventData.InputButton button)
        {
            TezDatabase.instance.clearZeroRefItem();
            TezJsonWriter writer = new TezJsonWriter(true);

            TezDatabase.instance.foreachInnateItem((TezItem item) =>
            {
                if(item != null)
                {
                    writer.beginObject(item.GUID);
                    item.serialization(writer);
                    writer.endObject(item.GUID);
                }
            });

            writer.save(TezcatFramework.rootPath + TezcatFramework.databaseFile);
        }

        private void onAddItem(PointerEventData.InputButton button)
        {
            if (m_Group.categoryType != null)
            {
                m_Window.createItemEditor(m_Group.categoryType);
            }
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

        protected override void clear()
        {
            m_AddItem = null;
            m_RefreshDataBase = null;

            base.clear();
        }
    }
}