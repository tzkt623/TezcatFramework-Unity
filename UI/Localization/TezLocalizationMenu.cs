using UnityEngine;
using UnityEngine.EventSystems;

using tezcat.Utility;
using tezcat.Serialization;
namespace tezcat.UI
{
    public class TezLocalizationMenu : TezArea
    {
        [SerializeField]
        TezImageLabelButton m_Refresh = null;
        [SerializeField]
        TezImageLabelButton m_Save = null;

        public TezLocalizationNameList nameList { get; set; }
        public TezLocalizationDescriptionList descriptionList { get; set; }

        protected override void preInit()
        {
            base.preInit();
            m_Refresh.onClick += onRefreshClick;
            m_Save.onClick += onSaveClick;
        }

        private void onSaveClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                TezJsonWriter writer = new TezJsonWriter();
                TezTranslator.serialization(writer);
                writer.save(TezcatGameEngine.localizationPath);
            }
        }

        private void onRefreshClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.nameList.dirty = true;
                this.descriptionList.dirty = true;
            }
        }

        protected override void onRefresh()
        {

        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }
    }
}