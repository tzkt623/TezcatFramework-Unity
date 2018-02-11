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

        public virtual TezSprite getSprite(TezIconType type)
        {
            return TezSprite.empty;
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