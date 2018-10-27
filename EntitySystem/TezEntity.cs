using UnityEngine;
using System.Collections;

namespace tezcat.Framework.EntitySystem
{
    public struct TezEntity
    {
        public static readonly TezEntity Error = new TezEntity(-1);

        public readonly int ID;
        public TezEntity(int id)
        {
            this.ID = id;
        }
    }
}