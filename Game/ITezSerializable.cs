using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public interface ITezSerializable
    {
        void serialization(TezJsonWriter writer);
        void deserialization(TezJsonReader reader);
    }
}