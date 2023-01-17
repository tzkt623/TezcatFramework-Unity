namespace tezcat.Framework.Core
{
    public interface ITezFlagable : ITezCloseable
    {
        TezFlagable.Flag flag { get; }
    }

    public abstract class TezFlagable : ITezFlagable
    {
        public class Flag
        {
            public bool isClosed = false;
        }

        Flag mFlag = new Flag();
        Flag ITezFlagable.flag => mFlag;

        public virtual void close()
        {
            mFlag.isClosed = true;
            mFlag = null;
        }
    }

    public class TezFlagableRef<T>
        : ITezCloseable
        where T : class, ITezFlagable
    {
        T mObject = null;
        TezFlagable.Flag mRef = null;

        public TezFlagableRef(T obj)
        {
            mRef = obj.flag;
            mObject = obj;
        }

        public T get()
        {
            return mObject;
        }

        public bool tryGet(out T obj)
        {
            if (mRef.isClosed)
            {
                obj = null;
                return false;
            }

            obj = mObject;
            return true;
        }

        public void close()
        {
            mRef = null;
            mObject = null;
        }
    }
}
