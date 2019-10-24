using tezcat.Framework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public class TezcatToolWindow : TezToolWindow
    {
        [Header("Menu")]
        [SerializeField]
        TezLabelButtonWithBG m_DatabaseButton;
        [SerializeField]
        TezLabelButtonWithBG m_LocalizationButton;
        [SerializeField]
        TezLabelButtonWithBG m_OptionButton;
        [SerializeField]
        TezLabelButtonWithBG m_CloseButton;

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

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected override void onClose()
        {
            m_DatabaseButton.onClick -= onDatabaseButtonClick;
            m_LocalizationButton.onClick -= onLocalizationButtonClick;
            m_OptionButton.onClick -= onOptionButtonClick;
            m_CloseButton.onClick -= onCloseButtonClick;

            base.onClose();
        }

        private void onCloseButtonClick(TezButton button, PointerEventData eventData)
        {
            this.close();
        }

        private void onOptionButtonClick(TezButton button, PointerEventData eventData)
        {
            TezService.get<TezcatFramework>().createWindow<TezOptionWindow>("TezOptionWindow", TezLayer.last).open();
            this.close();
        }

        private void onLocalizationButtonClick(TezButton button, PointerEventData eventData)
        {
            TezService.get<TezcatFramework>().createWindow<TezLocalizationWindow>("TezLocalizationWindow", TezLayer.last).open();
            this.close();
        }

        private void onDatabaseButtonClick(TezButton button, PointerEventData eventData)
        {
            TezService.get<TezcatFramework>().createWindow<TezDatabaseWindow>("TezDatabaseWindow", TezLayer.last).open();
            this.close();
        }
    }
}