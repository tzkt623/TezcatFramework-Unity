using tezcat.Core;

namespace tezcat.UI
{
    public class TezLocalizationWindow : TezWindow
    {
        TezLocalizationMenu m_Menu = null;
        TezLocalizationNameList m_NameList = null;
        TezLocalizationDescriptionList m_DescriptionList = null;

        protected override void preInit()
        {
            base.preInit();

            m_Menu = this.GetComponentInChildren<TezLocalizationMenu>();
            m_NameList = this.GetComponentInChildren<TezLocalizationNameList>();
            m_DescriptionList = this.GetComponentInChildren<TezLocalizationDescriptionList>();

            m_Menu.nameList = m_NameList;
            m_Menu.descriptionList = m_DescriptionList;
        }

        protected override void onRefresh()
        {
            base.onRefresh();
        }

        public override void clear()
        {
            base.clear();
            TezcatFramework.instance.createWindow<TezcatToolWindow>("TezcatToolWindow", TezLayer.last).open();
        }
    }
}