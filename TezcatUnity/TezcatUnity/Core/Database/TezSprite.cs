using System;
using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Unity.Database
{
    public class TezSprite : ITezCloseable
    {
        public enum Size
        {
            Normal = 0,
            Samll,
            Large
        }

        Sprite[] m_Sprite = new Sprite[Enum.GetValues(typeof(Size)).Length];

        public string name
        {
            get;
        }

        public TezSprite(string name)
        {
            this.name = name;
        }

        void ITezCloseable.closeThis()
        {
            m_Sprite = null;
        }

        public void set(Sprite sprite, Size size)
        {
            m_Sprite[(int)size] = sprite;
        }

        public Sprite get(Size size)
        {
            return m_Sprite[(int)size];
        }
    }
}