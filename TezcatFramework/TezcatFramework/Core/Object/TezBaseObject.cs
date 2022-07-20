namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezBaseObject : ITezCloseable
    {
        public string CID
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public abstract void close();
    }


    public abstract class TezObject : TezBaseObject
    {
        public uint objectUID { get; private set; }

        public virtual void init()
        {
            this.objectUID = TezObjectUID.generateID();
        }

        public override void close()
        {

        }
    }
}