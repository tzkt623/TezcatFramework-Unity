using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezOptionWindow : TezWindow
    {
        [Header("RootMenu")]
        [SerializeField]
        GameObject m_RootMenu = null;

        [SerializeField]
        InputField m_RootPath = null;

        [SerializeField]
        InputField m_LocalizationName = null;
        [SerializeField]
        InputField m_DatabaseName = null;
        [SerializeField]
        InputField m_SaveName = null;
        [SerializeField]
        TezImageLabelButton m_SaveAndClose = null;

        protected override void Awake()
        {
            base.Awake();
            m_RootPath.readOnly = true;
            m_LocalizationName.readOnly = true;
            m_DatabaseName.readOnly = true;
            m_SaveName.readOnly = true;

            m_SaveAndClose.onClick += onSaveAndCloseClick;
        }

        private void onSaveAndCloseClick(PointerEventData.InputButton button)
        {
            this.hide();
            m_RootMenu.SetActive(true);
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        private void checkFile(string path)
        {
            if (!TezFileTool.fileExist(path))
            {
                TezFileTool.createFile(path);
            }
        }

        protected override void onRefresh()
        {
            base.onRefresh();

            m_RootPath.text = TezcatFramework.rootPath;

            m_LocalizationName.text = TezcatFramework.localizationFile;
            m_DatabaseName.text = TezcatFramework.databaseFile;
            m_SaveName.text = TezcatFramework.saveFile;
        }
    }
}
