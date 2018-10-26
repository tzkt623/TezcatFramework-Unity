using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.EntitySystem
{
    public class TezEntityFactory<TMask>
        : ITezService
        where TMask : ITezMask
    {
        

        public TezEntity createEntity()
        {

            return default(TezEntity);
        }



        void ITezCloseable.close()
        {

        }
    }
}

