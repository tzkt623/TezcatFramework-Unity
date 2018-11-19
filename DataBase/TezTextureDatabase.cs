using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Framework.DataBase
{
    public class TezTextureDatabase : ITezService
    {
        Dictionary<string, TezSprite> m_SpriteDic = new Dictionary<string, TezSprite>();
        TezSprite m_Missing = null;

        public void add(Sprite[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                this.add(array[i]);
            }
        }

        public void add(Sprite sprite)
        {
            var ts = new TezSprite(sprite);
            if(m_SpriteDic.ContainsKey(ts.name))
            {
                throw new Exception(string.Format("There is the same name[{0}] sprite in DB", ts.name));
            }

            if(m_Missing == null && sprite.name == "MissingIcon")
            {
                m_Missing = new TezSprite(sprite);
            }
            else
            {
                m_SpriteDic.Add(ts.name, ts);
            }
        }

        public TezSprite get(string name)
        {
            TezSprite sprite;
            if(m_SpriteDic.TryGetValue(name, out sprite))
            {
                return sprite;
            }

            return m_Missing;
        }

        public void close()
        {
            foreach (var pair in m_SpriteDic)
            {
                pair.Value.close();
            }

            m_SpriteDic.Clear();
            m_SpriteDic = null;
        }
    }
}

