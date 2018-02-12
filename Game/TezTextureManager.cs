using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{
    public enum TezIconType
    {
        Normal = 0,
        Samll,
        Middle,
        Large
    }

    public class TezIconPack
    {
        TezSprite[] m_Icon = new TezSprite[4];

        public void setIcon(TezSprite sprite, TezIconType type)
        {
            m_Icon[(int)type] = sprite;
        }

        public TezSprite getIcon(TezIconType type)
        {
            return m_Icon[(int)type];
        }
    }

    public class TezSprite
    {
        public static TezSprite empty { get; private set; }

        int m_ID = 0;

        static TezSprite()
        {
            empty = new TezSprite();
        }

        public TezSprite()
        {
            m_ID = 0;
        }

        public TezSprite(string name)
        {
            m_ID = TezTextureManager.instance.getSpriteID(name);
        }

        public static implicit operator TezSprite(string name)
        {
            return new TezSprite(name);
        }

        public Sprite convertToSprite()
        {
            return TezTextureManager.instance.getSprite(m_ID);
        }
    }

    public class TezTextureManager : TezSingleton<TezTextureManager>
    {
        Dictionary<string, int> m_SpriteDic = new Dictionary<string, int>();

        List<Sprite> m_SpriteList = new List<Sprite>();

        public void setErrorSprite(Sprite sprite)
        {
            if (m_SpriteList.Count == 0)
            {
                m_SpriteList.Add(sprite);
            }
            else
            {
                m_SpriteList[0] = sprite;
            }

            m_SpriteDic.Add(sprite.name, 0);
        }

        public void add(Sprite sprite)
        {
            int id = m_SpriteList.Count;
            m_SpriteList.Add(sprite);
            m_SpriteDic.Add(sprite.name, id);
        }

        public int getSpriteID(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return 0;
            }

            int id = 0;
            m_SpriteDic.TryGetValue(name, out id);
            return id;
        }

        public Sprite getSprite(int id)
        {
            return m_SpriteList[id];
        }
    }
}