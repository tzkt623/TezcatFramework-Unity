using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat
{
    public class TezIconLabel
        : TezUIObjectMB
    {
        [SerializeField]
        Image m_Icon = null;
        [SerializeField]
        Text m_Label = null;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        public void setIcon(Sprite icon)
        {
            m_Icon.sprite = icon;
        }

        public void setLabel(string text)
        {
            m_Label.text = text;
        }

        public override void clear()
        {

        }
    }
}

