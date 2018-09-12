using System.Collections.Generic;
using tezcat.Core;
using UnityEngine;


namespace tezcat.DataBase
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
        : ITezPrototype<TezSprite>
        , ITezCloseable
    {
        public static TezSprite empty { get; private set; }

        public string prototypeName
        {
            get { return sprite.name; }
        }


        int m_ID = 0;

        public Sprite sprite { get; private set; }

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
            m_ID = TezTextureManager.getSpriteID(name);
        }

        public static implicit operator TezSprite(string name)
        {
            return new TezSprite(name);
        }

        public Sprite convertToSprite()
        {
            return TezTextureManager.getSprite(m_ID);
        }

        public TezSprite clone()
        {
            return this;
        }

        public void serialize(TezWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void deserialize(TezReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void close()
        {
            this.sprite = null;
        }
    }

    public class TezTextureManager
    {
        static Dictionary<string, int> m_SpriteDic = new Dictionary<string, int>();
        static List<Sprite> m_SpriteList = new List<Sprite>();

        public static void setErrorSprite(Sprite sprite)
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

        public static void add(Sprite sprite)
        {
            int id = m_SpriteList.Count;
            m_SpriteList.Add(sprite);
            m_SpriteDic.Add(sprite.name, id);
        }

        public static int getSpriteID(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return 0;
            }

            int id = 0;
            m_SpriteDic.TryGetValue(name, out id);
            return id;
        }

        public static Sprite getSprite(int id)
        {
            return m_SpriteList[id];
        }

        public static Sprite getSprite(string name)
        {
            int id = -1;
            if(m_SpriteDic.TryGetValue(name, out id))
            {
                return m_SpriteList[id];
            }

            return m_SpriteList[0];
        }
    }
}