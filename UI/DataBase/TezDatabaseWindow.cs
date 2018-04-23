using UnityEngine;
using tezcat.DataBase;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezWindow
    {
        [SerializeField]
        GameObject m_RootMenu = null;

        [SerializeField]
        TezDatabaseMenu m_Menu = null;
        [SerializeField]
        TezDatabaseGroup m_Group = null;
        [SerializeField]
        TezDatabaseItemContainer m_Container = null;


        public TezDatabase.GroupType selectGroup { get; set; }
        public TezDatabase.CategoryType selectCategory { get; set; }



        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        protected override void onRefresh()
        {
            base.onRefresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_RootMenu.SetActive(true);
        }
    }
}