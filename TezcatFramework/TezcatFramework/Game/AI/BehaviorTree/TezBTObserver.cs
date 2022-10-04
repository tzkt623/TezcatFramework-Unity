using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    public class TezBTObserver : ITezCloseable
    {
        public static readonly TezBTObserver empty = new TezBTObserver();

        public TezBehaviorTree tree { get; set; }

        public virtual void close()
        {
            this.tree = null;
        }


        public virtual void init()
        {

        }

        /// <summary>
        /// 检测是否需要重置整个树
        /// </summary>
        public virtual bool update(ITezBTContext context)
        {
            return false;
        }
    }
}