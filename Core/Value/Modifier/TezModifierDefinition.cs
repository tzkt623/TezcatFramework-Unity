namespace tezcat.Framework.Core
{
    public abstract class TezModifierDefinition : ITezCloseable
    {
        public enum Assemble : sbyte
        {
            Null = -1,
            SumBase,
            SumTotal,
            MultBase,
            MultTotal
        }
        public Assemble assemble { get; set; } = Assemble.Null;
        public ITezValueDescriptor target { get; set; } = null;

        public virtual void close()
        {
            this.target = null;
        }
    }
}