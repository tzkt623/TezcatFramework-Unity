using tezcat.Framework.Shape;

namespace tezcat.Framework.Utility
{
    public class TezQtPoint : TezQtEntry
    {
        TezPoint mPoint = new TezPoint();
        public override TezShape shape => point;
        public TezPoint point => mPoint;

        public TezQtPoint()
        {

        }

        public TezQtPoint(int x, int y)
        {
            mPoint.originX = x;
            mPoint.originY = y;
        }

        public override void close()
        {
            base.close();
            mPoint = null;
        }
    }
}