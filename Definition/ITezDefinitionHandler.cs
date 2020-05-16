namespace tezcat.Framework.Definition
{
    public interface ITezDefinitionHandler
    {
        void addDefinitionObject(ITezDefinitionObject def_object);
        void removeDefinitionObject(ITezDefinitionObject def_object);
    }

    public interface ITezDefinitionObject
    {
        TezDefinition definition { get; }
    }

    public interface ITezDefinitionObjectAndHandler
        : ITezDefinitionObject
        , ITezDefinitionHandler
    {

    }
}