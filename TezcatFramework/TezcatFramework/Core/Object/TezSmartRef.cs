namespace tezcat.Framework.Core
{
    public interface ITezSmartRefObject : ITezCloseable
    {

    }

    public class TezSmartRef<T>
        : ITezCloseable
        where T : class, ITezSmartRefObject
    {
        class Ref
        {
            public T obj;
            public int countor;
        }

        Ref mRef = null;

        public TezSmartRef(T obj)
        {
            mRef = new Ref()
            {
                obj = obj,
                countor = 1
            };
        }

        public TezSmartRef(TezSmartRef<T> other)
        {
            mRef = other.mRef;
            mRef.countor++;
        }

        public T get()
        {
            return mRef.obj;
        }

        public bool tryGet(out T obj)
        {
            if (mRef.countor == 0)
            {
                obj = null;
                return false;
            }

            obj = mRef.obj;
            return true;
        }

        public void close()
        {
            mRef.countor--;
            if (mRef.countor == 0)
            {
                mRef.obj.close();
                mRef.obj = null;
            }

            mRef = null;
        }

        public static implicit operator TezSmartRef<T>(T value)
        {
            return new TezSmartRef<T>(value);
        }
    }
}
