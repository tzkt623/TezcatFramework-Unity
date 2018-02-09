using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{
    /// <summary>
    /// 包装器
    /// </summary>
    public abstract class TezObjectWrapper
    {
        public virtual string name
        {
            get { return null; }
        }

        public virtual string description
        {
            get { return null; }
        }

        public virtual Sprite getSprite(int type)
        {
            //             var temp = TezResourceSystem.instance.getItem(m_ResUID);
            //             return TezTextureManager.instance.getSprite(temp.icon[type]);
            return ResourceSystem.instance.errorIcon;
        }

        public Texture2D getTexture2D(int type)
        {
            return null;
        }

        public virtual bool equalTo(TezObjectWrapper other)
        {
            return this == other;
        }

        public virtual void clear()
        {

        }
    }
}