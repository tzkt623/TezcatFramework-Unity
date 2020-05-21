using System;

namespace tezcat.Framework.AI
{
    public abstract class TezBTCompositeNode : TezBTNode
    {
        public abstract void addNode(TezBTNode node);
    }
}