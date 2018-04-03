using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezUILineRender : Graphic
    {
        [SerializeField]
        Vector2[] m_Positions;

        protected override void OnPopulateMesh(VertexHelper vh)
        {

        }
    }
}