using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezBufferType
    {
        Property,
        Function,
        Custom
    }

    public interface ITezBuffer : ITezCloseable
    {
        object source { get; set; }
        TezBufferType bufferType { get; }
    }

    public abstract class TezBuffer : ITezBuffer
    {
        public object source { get; set; }
        public abstract TezBufferType bufferType { get; }

        void ITezCloseable.deleteThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            this.source = null;
        }
    }

    public abstract class TezPropertyBuffer : TezBuffer
    {
        public sealed override TezBufferType bufferType => TezBufferType.Property;
        public abstract void onAdd(ITezValue current_property);
        public abstract void onRemove(ITezValue current_property);
    }

    public abstract class TezFunctionBuffer : TezBuffer
    {
        public sealed override TezBufferType bufferType => TezBufferType.Function;
    }
}