﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public interface ITezSerializable
    {
        void serialize(TezSaveManager manager);
        void deserialize(TezSaveManager manager);
    }

    public interface ITezSerializableItem
    {
        string NID { get; }
        void serialize(TezWriter writer);
        void deserialize(TezReader reader);
    }
}