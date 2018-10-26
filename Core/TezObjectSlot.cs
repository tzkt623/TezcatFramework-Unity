namespace tezcat.Framework.Core
{
    public abstract class TezObjSlot : TezSlot
    {
        public TezObject myObject { get; set; } = null;

        public override void close()
        {
            this.myObject = null;
        }
    }
}