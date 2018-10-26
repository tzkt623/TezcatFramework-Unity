using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Framework.DataBase
{
    public class TezTextureDatabase : ITezService
    {
        static List<TezSprite> m_SpriteList = new List<TezSprite>();
        static Dictionary<string, TezSprite> m_SpriteDic = new Dictionary<string, TezSprite>();

        public void add(Sprite sprite)
        {
            var ts = new TezSprite(sprite);
            if(m_SpriteDic.ContainsKey(ts.name))
            {
                throw new Exception(string.Format("There is the same name[{0}] sprite in DB", ts.name));
            }

            m_SpriteList.Add(ts);
            m_SpriteDic.Add(ts.name, ts);
        }

        public void close()
        {
            foreach (var sprite in m_SpriteList)
            {
                sprite.close();
            }

            m_SpriteList.Clear();
            m_SpriteDic.Clear();

            m_SpriteList = null;
            m_SpriteDic = null;
        }
    }
}

