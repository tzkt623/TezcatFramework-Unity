using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezScrollCell : UIBehaviour
    {
        public Vector2 size { get; private set; } = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
            size = ((RectTransform)this.transform).rect.size;
        }

        protected override void Start()
        {
            base.Start();
        }
    }


}

