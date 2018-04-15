using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{

    public class TezDatabaseMenu : TezArea
    {
        [SerializeField]
        TezImageLabelButton m_AddGroup = null;
        [SerializeField]
        TezImageLabelButton m_AddType = null;
        [SerializeField]
        TezImageLabelButton m_AddItem = null;
        [SerializeField]
        TezImageLabelButton m_RefreshDataBase = null;

        protected override void Start()
        {
            base.Start();

            m_AddGroup.onClick += onAddGroup; ;
            m_RefreshDataBase.onClick += onRefreshDataBase;
        }

        private void onAddGroup(PointerEventData.InputButton button)
        {

        }

        private void onRefreshDataBase(PointerEventData.InputButton button)
        {

        }

        public override void clear()
        {

        }

        protected override void onRefresh()
        {

        }
    }
}