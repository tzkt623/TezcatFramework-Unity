namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionPathNode : TezDefinitionChildrenNode
    {
        public override TezDefinitionNodeType nodeType => TezDefinitionNodeType.Path;
    }
}