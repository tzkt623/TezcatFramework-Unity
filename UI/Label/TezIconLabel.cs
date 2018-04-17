using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezIconLabel
        : TezUIObjectMB
    {
        [SerializeField]
        Image m_Icon = null;
        [SerializeField]
        Text m_Label = null;

        public Sprite icon
        {
            get { return m_Icon.sprite; }
            set { m_Icon.sprite = value; }
        }

        public string text
        {
            get { return m_Label.text; }
            set { m_Label.text = value; }
        }

        public override void clear()
        {

        }
    }
}

