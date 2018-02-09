using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public interface ITezSerializable
    {
        TezJsonWriter serialization();
        void deserialization(TezJsonReader reader);
    }
}