using UnityEngine;
using UnityEngine.UI;


namespace tezcat.UI
{
    public class TezImageButtonWithBG : TezImageButton
    {
        [SerializeField]
        public Image background { get; private set; } = null;

        protected override void initWidget()
        {
            base.initWidget();
        }
    }
}