using System.Collections.Generic;
using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Framework.Database
{
    public class TezTextureDatabase : ITezService
    {
        Dictionary<string, TezSprite> m_SpriteDic = new Dictionary<string, TezSprite>();

        TezSprite m_Missing = new TezSprite("MissingIcon");

        public void add(Sprite[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                this.add(array[i]);
            }
        }

        public void add(Sprite sprite)
        {
            string name = sprite.name.Substring(0, sprite.name.IndexOf('('));

            var ext_index = name.LastIndexOf('_');
            var ext = 'N';
            if (ext_index >= 0)
            {
                ext = name[name.Length - 1];
                name = name.Remove(name.Length - 2);
            }

            if (name == "MissingIcon")
            {
                switch (ext)
                {
                    case 'S':
                        m_Missing.set(sprite, TezSprite.Size.Samll);
                        Debug.Log(name);
                        break;
                    case 'L':
                        m_Missing.set(sprite, TezSprite.Size.Large);
                        break;
                    default:
                        m_Missing.set(sprite, TezSprite.Size.Normal);
                        break;
                }
            }
            else
            {
                TezSprite container = null;
                if (!m_SpriteDic.TryGetValue(name, out container))
                {
                    container = new TezSprite(name);
                    m_SpriteDic.Add(name, container);
                }

                switch (ext)
                {
                    case 'S':
                        container.set(sprite, TezSprite.Size.Samll);
                        break;
                    case 'L':
                        container.set(sprite, TezSprite.Size.Large);
                        break;
                    default:
                        container.set(sprite, TezSprite.Size.Normal);
                        break;
                }
            }
        }

        public TezSprite get(string name)
        {
            TezSprite sprite;
            if (m_SpriteDic.TryGetValue(name, out sprite))
            {
                return sprite;
            }

            return m_Missing;
        }

        public void close(bool self_close = true)
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

