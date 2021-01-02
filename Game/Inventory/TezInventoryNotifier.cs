using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public interface ITezInventoryNotifier : ITezCloseable
    {

    }


    public class TezInventoryNotifier : ITezInventoryNotifier
    {
        public void notifyItemAdd()
        {

        }

        public virtual void close()
        {

        }
    }
}
