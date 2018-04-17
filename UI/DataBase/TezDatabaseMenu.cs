﻿using tezcat.DataBase;
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

        protected override void Start()
        {
            base.Start();

            m_Save.onClick += onSave;
            m_AddItem.onClick += onAddItem;
            m_RefreshDataBase.onClick += onRefreshDataBase;

            m_Group = this.window.getArea<TezDatabaseGroup>();
        }

        private void onSave(PointerEventData.InputButton button)
        {
            //TezDatabase.instance.save();
        }

        private void onAddItem(PointerEventData.InputButton button)
        {

        }

        private void onRefreshDataBase(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                m_Group.refreshDataBase();
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