using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezTuple
    {

    }

    public class TezTuple<T1> : ITezTuple
    {
        public T1 v1 { get; set; }

        public TezTuple(T1 t1)
        {
            this.v1 = t1;
        }
    }

    public class TezTuple<T1, T2> : TezTuple<T1>
    {
        public T2 v2 { get; set; }

        public TezTuple(T1 t1, T2 t2) : base(t1)
        {
            this.v2 = t2;
        }
    }

    public class TezTuple<T1, T2, T3> : TezTuple<T1, T2>
    {
        public T3 v3 { get; set; }

        public TezTuple(T1 t1, T2 t2, T3 t3) : base(t1, t2)
        {
            this.v3 = t3;
        }
    }

    public class TezTuple<T1, T2, T3, T4> : TezTuple<T1, T2, T3>
    {
        public T4 v4 { get; set; }

        public TezTuple(T1 t1, T2 t2, T3 t3, T4 t4) : base(t1, t2, t3)
        {
            this.v4 = t4;
        }
    }

    public class TezTuple<T1, T2, T3, T4, T5> : TezTuple<T1, T2, T3, T4>
    {
        public T5 v5 { get; set; }

        public TezTuple(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) : base(t1, t2, t3, t4)
        {
            this.v5 = t5;
        }
    }
}