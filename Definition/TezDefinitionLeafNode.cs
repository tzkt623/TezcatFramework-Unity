namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionLeafNode : ITezDefinitionNode
    {
        public TezDefinitionNodeType nodeType => TezDefinitionNodeType.Leaf;

        public abstract void onRegisterObject(ITezDefinitionPathObject path_object);
        public abstract void onUnregisterObject(ITezDefinitionPathObject path_object);

        public abstract void close();
    }
}