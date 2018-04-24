using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezWindow
    {
        [SerializeField]
        GameObject m_RootMenu = null;

        [Header("Area")]
        [SerializeField]
        TezDatabaseMenu m_Menu = null;
        [SerializeField]
        TezDatabaseGroup m_Group = null;
        [SerializeField]
        TezDatabaseItemContainer m_Container = null;


        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            m_Menu.setGroup(m_Group);
            m_Container.setGroup(m_Group);
            m_Group.setContainer(m_Container);
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