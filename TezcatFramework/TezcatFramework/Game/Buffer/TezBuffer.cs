using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezBufferType
    {
        Attribute,
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
        public virtual void close()
        {
            this.source = null;
        }
    }

    public abstract class TezAttributeBuffer : TezBuffer
    {
        public sealed override TezBufferType bufferType => TezBufferType.Attribute;
        public abstract void onAdd(ITezLitProperty current_property);
        public abstract void onRemove(ITezLitProperty current_property);
    }

    public abstract class TezFunctionBuffer : TezBuffer
    {
        public sealed override TezBufferType bufferType => TezBufferType.Function;
    }
}