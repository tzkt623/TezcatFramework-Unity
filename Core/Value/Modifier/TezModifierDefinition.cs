using tezcat.Framework.Definition;

namespace tezcat.Framework.Core
{
    public class TezModifierDefinition : ITezCloseable
    {
        public TezDefinitionPath definitionPath { get; set; } = null;

        public virtual void close()
        {
            this.definitionPath.close();
            this.definitionPath = null;
        }

        public override string ToString()
        {
            return string.Format("Target:{0}\nPath:\n**********\n", definitionPath.ToString());
        }
    }
}