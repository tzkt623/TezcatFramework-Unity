namespace tezcat.Framework.Core
{
    public class TezValueModifierDefinition : TezModifierDefinition
    {
        public enum Assemble : byte
        {
            SumBase = 0,
            SumTotal,
            MultBase,
            MultTotal
        }

        public Assemble assemble { get; set; } = Assemble.SumBase;
        public ITezValueDescriptor target { get; set; } = null;

        public override void close(bool self_close = true)
        {
            base.close(self_close);

            this.target = null;
        }

        public override string ToString()
        {
            return string.Format("Assemble:{0}\nTarget:{1}\nPath:\n**********\n{2}\n**********"
                , this.assemble
                , target.name
                , definitionPath.ToString());
        }
    }
}