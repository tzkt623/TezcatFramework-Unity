namespace tezcat.Core
{
    public abstract class TezBaseDataSlot
    {
        public enum Type
        {
            Single,
            Multi
        }

        public abstract Type slotType { get; }
        public abstract void clear();
    }

    public abstract class TezSingleDataSlot : TezBaseDataSlot
    {
        public sealed override Type slotType
        {
            get { return Type.Single; }
        }

        public abstract ITezData getTezData();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TezSingleDataSlot<T> : TezSingleDataSlot where T : class, ITezData
    {
        public T myData { get; protected set; } = null;

        public sealed override ITezData getTezData()
        {
            return myData;
        }

        public override void clear()
        {
            this.myData = null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class TezMultiDataSlot : TezBaseDataSlot
    {
        public sealed override Type slotType
        {
            get { return Type.Multi; }
        }

        public abstract ITezData[] getTezDatas();
    }

    public abstract class TezMultiDataSlot<S1, S2>
        : TezMultiDataSlot
        where S1 : class, ITezData
        where S2 : class, ITezData
    {
        public S1 data1 { get; protected set; } = null;
        public S2 data2 { get; protected set; } = null;

        public override ITezData[] getTezDatas()
        {
            return new ITezData[] { data1, data2 };
        }

        public override void clear()
        {
            data1 = null;
            data2 = null;
        }
    }

    public abstract class TezMultiDataSlot<S1, S2, S3>
        : TezMultiDataSlot
        where S1 : class, ITezData
        where S2 : class, ITezData
        where S3 : class, ITezData
    {
        public S1 data1 { get; protected set; } = null;
        public S2 data2 { get; protected set; } = null;
        public S3 data3 { get; protected set; } = null;

        public override ITezData[] getTezDatas()
        {
            return new ITezData[] { data1, data2, data3 };
        }

        public override void clear()
        {
            data1 = null;
            data2 = null;
            data3 = null;
        }
    }
}