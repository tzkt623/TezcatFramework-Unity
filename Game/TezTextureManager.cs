using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{
    public class TezTextureManager : TezSingleton<TezTextureManager>
    {
        Dictionary<string, int> m_SpriteDic = new Dictionary<string, int>();
        List<Sprite> m_SpriteList = new List<Sprite>();

        Dictionary<string, int> m_Tex2DDic = new Dictionary<string, int>();
        List<Texture2D> m_Tex2DList = new List<Texture2D>();

        public void load(Sprite error_sprite, List<Sprite> list)
        {
            m_SpriteList.Add(error_sprite);
            m_SpriteList.AddRange(list);
            for (int i = 0; i < m_SpriteList.Count; i++)
            {
                m_SpriteDic.Add(m_SpriteList[i].name, i);
            }

            m_SpriteDic.Clear();
        }

        public void load(Texture2D error_tex2d, List<Texture2D> list)
        {
            m_Tex2DList.Add(error_tex2d);
            m_Tex2DList.AddRange(list);
            for (int i = 0; i < m_Tex2DList.Count; i++)
            {
                m_Tex2DDic.Add(m_Tex2DList[i].name, i);
            }
        }

        public Sprite getSprite(int id)
        {
            return m_SpriteList[id];
        }

        public int getSpriteID(string name)
        {
            int i = 0;
            if(m_SpriteDic.TryGetValue(name, out i))
            {
                return i;
            }

            return i;
        }

        public Texture2D getTextrue2D(int id)
        {
            return m_Tex2DList[id];
        }
    }
}