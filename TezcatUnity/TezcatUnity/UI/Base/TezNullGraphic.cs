﻿using UnityEngine.UI;

namespace tezcat.Unity.UI
{
    public class TezNullGraphic : UnityEngine.UI.Graphic
    {
        public override void SetMaterialDirty()
        {
            
        }

        public override void SetVerticesDirty()
        {

        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}