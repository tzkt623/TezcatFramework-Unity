using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezPopupContent : UIBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            this.name = "Popup";
        }
    }
}
