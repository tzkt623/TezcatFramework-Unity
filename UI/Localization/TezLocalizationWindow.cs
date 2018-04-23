using UnityEngine;
namespace tezcat.UI
{
    public class TezLocalizationWindow : TezWindow
    {
        [SerializeField]
        GameObject m_RootMenu = null;

        TezLocalizationMenu m_Menu = null;
        TezLocalizationNameList m_NameList = null;
        TezLocalizationDescriptionList m_DescriptionList = null;

        protected override void Awake()
        {
            base.Awake();

            m_Menu = this.GetComponentInChildren<TezLocalizationMenu>();
            m_NameList = this.GetComponentInChildren<TezLocalizationNameList>();
            m_DescriptionList = this.GetComponentInChildren<TezLocalizationDescriptionList>();

            m_Menu.nameList = m_NameList;
            m_Menu.descriptionList = m_DescriptionList;
        }

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