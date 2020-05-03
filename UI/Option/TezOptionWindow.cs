using tezcat.Framework.Core;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
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
        TezButton m_SaveAndClose = null;

        protected override void preInit()
        {
            base.preInit();
            m_RootPath.readOnly = true;
            m_LocalizationName.readOnly = true;
            m_DatabaseName.readOnly = true;
            m_SaveName.readOnly = true;

            m_SaveAndClose.onClick += onSaveAndCloseClick;
        }

        private void onSaveAndCloseClick(TezButton button, PointerEventData eventData)
        {
            this.hide();
        }

        private void checkFile(string path)
        {
            if (!TezFilePath.fileExist(path))
            {
                TezFilePath.createFile(path);
            }
        }

        private void refreshData()
        {
            m_RootPath.text = TezcatFramework.dataPath;

//             m_LocalizationName.text = TezcatFramework.localizationFile;
//             m_DatabaseName.text = TezcatFramework.databaseFile;
//             m_SaveName.text = TezcatFramework.saveFile;
        }
    }
}
