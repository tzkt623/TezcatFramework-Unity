using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    public enum TezBTResult : byte
    {
        Empty = 0,
        Fail,
        Success,
        Running,
    }

    public abstract class TezBTNode<Data>
        : ITezCloseable
        where Data : ITezBTData
    {
        public string name { get; set; }

        public abstract void close();

        public abstract TezBTResult execute(Data data);

        public static bool operator true(TezBTNode<Data> obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezBTNode<Data> obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezBTNode<Data> obj)
        {
            return object.ReferenceEquals(obj, null);
        }
    }

    public abstract class TezBTCompositeNode<Data>
        : TezBTNode<Data>
        where Data : ITezBTData
    {
        public abstract void addNode(TezBTNode<Data> node);
    }
}