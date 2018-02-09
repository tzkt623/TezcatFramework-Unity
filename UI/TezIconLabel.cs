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

        protected override void onAwake()
        {

        }

        protected override void onDelete()
        {

        }

        protected override void onStart()
        {

        }

        public void setIcon(Sprite icon)
        {
            m_Icon.sprite = icon;
        }

        public void setLabel(string text)
        {
            m_Label.text = text;
        }

        protected override void onDisable()
        {

        }

        protected override void onEnable()
        {

        }
    }
}

