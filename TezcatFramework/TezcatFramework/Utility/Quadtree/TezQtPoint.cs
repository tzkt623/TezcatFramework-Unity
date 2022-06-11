using tezcat.Framework.Shape;

namespace tezcat.Framework.Utility
{
    public class TezQtPoint : TezQtEntry
    {
        public override TezShape shape => point;
        public TezPoint point { get; private set; } = new TezPoint();

        public TezQtPoint()
        {

        }

        public TezQtPoint(int x, int y)
        {
            this.point.originX = x;
            this.point.originY = y;
        }

        public override void close()
        {
            base.close();
            this.point = null;
        }
    }
}