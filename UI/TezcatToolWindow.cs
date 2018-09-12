using tezcat.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezcatToolWindow : TezToolWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezImageLabelButton m_DatabaseButton;
        [SerializeField]
        TezImageLabelButton m_LocalizationButton;
        [SerializeField]
        TezImageLabelButton m_OptionButton;
        [SerializeField]
        TezImageLabelButton m_CloseButton;

        protected override void preInit()
        {
            m_DatabaseButton.onClick += onDatabaseButtonClick;
            m_LocalizationButton.onClick += onLocalizationButtonClick;
            m_OptionButton.onClick += onOptionButtonClick;
            m_CloseButton.onClick += onCloseButtonClick;
        }

        protected override void initWidget()
        {
            TezcatFramework.checkNeedFile();
        }

        protected override void linkEvent() 
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        public override void clear()
        {
            m_DatabaseButton.onClick -= onDatabaseButtonClick;
            m_LocalizationButton.onClick -= onLocalizationButtonClick;
            m_OptionButton.onClick -= onOptionButtonClick;
            m_CloseButton.onClick -= onCloseButtonClick;

            base.clear();
        }

        protected override void onRefresh()
        {

        }

        private void onCloseButtonClick(PointerEventData.InputButton button)
        {
            this.close();
        }

        private void onOptionButtonClick(PointerEventData.InputButton button)
        {
            TezcatFramework.instance.createWindow<TezOptionWindow>("TezOptionWindow", TezLayer.last).open();
            this.close();
        }

        private void onLocalizationButtonClick(PointerEventData.InputButton button)
        {
            TezcatFramework.instance.createWindow<TezLocalizationWindow>("TezLocalizationWindow", TezLayer.last).open();
            this.close();
        }

        private void onDatabaseButtonClick(PointerEventData.InputButton button)
        {
            TezcatFramework.instance.createWindow<TezDatabaseWindow>("TezDatabaseWindow", TezLayer.last).open();
            this.close();
        }
    }
}