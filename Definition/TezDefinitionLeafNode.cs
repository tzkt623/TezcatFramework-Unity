namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionLeafNode : ITezDefinitionNode
    {
        public TezDefinitionNodeType nodeType => TezDefinitionNodeType.Leaf;

        public abstract void onRegisterObject(ITezDefinitionPathWithObject path_with_object);
        public abstract void onUnregisterObject(ITezDefinitionPathWithObject path_with_object);

        public abstract void close();
    }
}