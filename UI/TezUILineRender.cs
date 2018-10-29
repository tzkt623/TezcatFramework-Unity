using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezUILineRender : UnityEngine.UI.Graphic
    {
        [SerializeField]
        Vector2[] m_Positions;

        protected override void OnPopulateMesh(VertexHelper vh)
        {

        }
    }
}