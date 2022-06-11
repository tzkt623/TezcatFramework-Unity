namespace tezcat.Framework.Attribute
{
    public interface ITezAttributeHandler
    {
        void addAttributeDefObject(ITezAttributeDefObject def_object);
        void removeAttributeDefObject(ITezAttributeDefObject def_object);
    }

    public interface ITezAttributeDefObject
    {
        TezAttributeDef definition { get; }
    }

    public interface ITezAttributeBuilder
        : ITezAttributeDefObject
        , ITezAttributeHandler
    {

    }
}