using System.Collections.Generic;
using tezcat.String;
using UnityEngine;

namespace tezcat.DataBase
{
    public class TezTextureDatabase : ScriptableObject
    {
        [SerializeField]
        Sprite m_ErrorSprite = null;
        [SerializeField]
        List<Sprite> m_Sprites = new List<Sprite>();

        class Texture
        {
            public int ID { get; set; }
        }
        

        static List<Sprite> m_SpriteList = new List<Sprite>();
        static Dictionary<TezStaticString, Sprite> m_SpriteDic = new Dictionary<TezStaticString, Sprite>();

        public void init()
        {
            m_SpriteList.Add(m_ErrorSprite);
            foreach (var s in m_Sprites)
            {
                m_SpriteList.Add(s);
            }

            foreach (var s in m_Sprites)
            {
                m_SpriteDic.Add(s.name, s);
            }
        }
    }
}

