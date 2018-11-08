namespace tezcat.Framework.Utility
{
    public class TezTuple<T1, T2>
    {
        public T1 v1 { get; set; }
        public T2 v2 { get; set; }
    }

    public class TezTuple<T1, T2, T3> : TezTuple<T1, T2>
    {
        public T3 v3 { get; set; }
    }

    public class TezTuple<T1, T2, T3, T4> : TezTuple<T1, T2, T3>
    {
        public T4 v4 { get; set; }
    }
}