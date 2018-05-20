using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezcatControlCenter : TezWidget
    {
        [Header("Menu")]
        [SerializeField]
        GameObject m_Menu = null;
        [SerializeField]
        TezImageLabelButton m_DatabaseButton;
        [SerializeField]
        TezImageLabelButton m_LocalizationButton;
        [SerializeField]
        TezImageLabelButton m_OptionButton;
        [SerializeField]
        TezImageLabelButton m_CloseButton;

        [Header("Function")]
        [SerializeField]
        TezDatabaseWindow m_DatabaseWindow = null;
        [SerializeField]
        TezLocalizationWindow m_LocalizationWindow = null;
        [SerializeField]
        TezOptionWindow m_OptionWindow = null;

        private void onCloseButtonClick(PointerEventData.InputButton button)
        {
            m_Menu.SetActive(false);
        }

        private void onOptionButtonClick(PointerEventData.InputButton button)
        {
            m_OptionWindow.open();
            m_Menu.SetActive(false);
        }

        private void onLocalizationButtonClick(PointerEventData.InputButton button)
        {
            m_LocalizationWindow.open();
            m_Menu.SetActive(false);
        }

        private void onDatabaseButtonClick(PointerEventData.InputButton button)
        {
            m_DatabaseWindow.open();
            m_Menu.SetActive(false);
        }

        public override void clear()
        {

        }

        protected override void onRefresh()
        {

        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.F12))
            {
                m_Menu.SetActive(true);
            }
        }

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
    }
}