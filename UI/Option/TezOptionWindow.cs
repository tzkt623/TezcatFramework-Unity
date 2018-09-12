using tezcat.Core;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezOptionWindow : TezToolWindow
    {
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

        protected override void preInit()
        {
            base.preInit();
            m_RootPath.readOnly = true;
            m_LocalizationName.readOnly = true;
            m_DatabaseName.readOnly = true;
            m_SaveName.readOnly = true;

            m_SaveAndClose.onClick += onSaveAndCloseClick;
        }

        private void onSaveAndCloseClick(PointerEventData.InputButton button)
        {
            this.hide();
        }

        private void checkFile(string path)
        {
            if (!TezPath.fileExist(path))
            {
                TezPath.createFile(path);
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
