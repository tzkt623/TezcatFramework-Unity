using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public class TezHexBlock : ITezCloseable
    {
        public TezHexCubeCoordinate coordinate { get; set; }

        public virtual void close(bool self_close = true)
        {

        }
    }
}
