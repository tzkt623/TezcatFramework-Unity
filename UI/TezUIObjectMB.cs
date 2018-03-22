using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezUIObjectMB : UIBehaviour
    {
        public virtual bool interactable { get; set; } = true;

        public abstract void clear();
    }
}