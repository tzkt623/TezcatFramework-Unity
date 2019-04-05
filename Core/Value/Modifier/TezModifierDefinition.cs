namespace tezcat.Framework.Core
{
    public abstract class TezModifierDefinition
    {
        public enum Assemble : sbyte
        {
            Null = -1,
            Sum_Base,
            Sum_Total,
            Mult_Base,
            Mult_Total
        }
        public Assemble assemble { get; set; } = Assemble.Null;
        public ITezValueDescriptor target { get; set; } = null;
    }
}