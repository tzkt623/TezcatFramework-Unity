using UnityEngine;

namespace tezcat.Framework.DataBase
{
    public class TezSprite : TezDataBaseAssetItem
    {
        public static TezSprite missing { get; } = new TezSprite();

        Sprite m_Sprite = null;

        public string name
        {
            get { return m_Sprite.name; }
        }

        public TezSprite(Sprite sprite = null)
        {
            m_Sprite = sprite;
        }

        public override void close()
        {
            m_Sprite = null;
        }

        public static implicit operator Sprite(TezSprite sprite)
        {
            return sprite.m_Sprite;
        }
    }
}