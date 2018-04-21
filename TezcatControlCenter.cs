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

        protected override void Awake()
        {
            base.Awake();

            m_DatabaseButton.onClick += onDatabaseButtonClick;
            m_LocalizationButton.onClick += onLocalizationButtonClick;
            m_OptionButton.onClick += onOptionButtonClick;
            m_CloseButton.onClick += onCloseButtonClick;
        }

        protected override void Start()
        {
            base.Start();

            this.checkNeedFile();
        }

        private void checkNeedFile()
        {
            if (!TezFileTool.directoryExist(TezcatFramework.rootPath))
            {
                var info = TezFileTool.createDirectory(TezcatFramework.rootPath);
            }

            this.checkFile(TezcatFramework.localizationPath);
            this.checkFile(TezcatFramework.databasePath);
            this.checkFile(TezcatFramework.savePath);
        }

        private void checkFile(string path)
        {
            if (!TezFileTool.fileExist(path))
            {
                TezFileTool.createFile(path);
            }
        }

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

        protected override void clear()
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
    }
}