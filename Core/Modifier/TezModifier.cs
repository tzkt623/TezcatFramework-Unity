namespace tezcat.Framework.Core
{
    public interface ITezModifier
        : ITezCloseable
    {
        int operationID { get; }
        object source { get; }
        ITezModifierOperation operation { get; }
        ITezValueDescriptor descriptor { get; }      
    }

    public abstract class TezModifier : ITezModifier
    {
        public int operationID
        {
            get { return this.operation.toID; }
        }

        public object source { get; }
        public ITezModifierOperation operation { get; }
        public ITezValueDescriptor descriptor { get; }

        protected TezModifier(ITezValueDescriptor descriptor, ITezModifierOperation operation)
        {
            this.descriptor = descriptor;
            this.operation = operation;
        }

        public abstract void close();
    }
}