namespace tezcat.Framework.Core
{
    public interface ITezLifeMonitor
    {
        TezLifeMonitor lifeMonitor { get; }
    }

    public class TezLifeMonitor : ITezCloseable
    {
        public class Flag
        {
            public object managedObject;
        }

        Flag mFlag = new Flag();
        public Flag flag => mFlag;

        public void setManagedObject(object obj)
        {
            mFlag.managedObject = obj;
        }

        public virtual void close()
        {
            mFlag.managedObject = null;
            mFlag = null;
        }
    }

    public class TezLifeMonitorSlot : ITezCloseable
    {
        protected TezLifeMonitor.Flag mFlag = null;

        public TezLifeMonitorSlot(ITezLifeMonitor iLifeMonitor)
        {
            mFlag = iLifeMonitor.lifeMonitor.flag;
        }

        public bool tryUse<T>(out T result) where T : class
        {
            if (mFlag.managedObject != null)
            {
                result = (T)mFlag.managedObject;
                return true;
            }

            result = null;
            return false;
        }

        public void close()
        {
            mFlag = null;
        }
    }
}
