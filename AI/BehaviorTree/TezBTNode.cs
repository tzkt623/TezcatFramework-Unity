using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    public abstract class TezBTNode<Data>
        : ITezCloseable
        where Data : ITezBTData
    {
        public string name { get; set; }
        public abstract TezBTNodeType nodeType { get; }

        public abstract TezBTResult execute(Data data);
        public abstract void close();

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