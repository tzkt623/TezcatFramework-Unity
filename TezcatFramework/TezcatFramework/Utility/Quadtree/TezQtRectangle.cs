using tezcat.Framework.Shape;

namespace tezcat.Framework.Utility
{
    public class TezQtRectangle : TezQtEntry
    {
        TezRectangle mRectangle = new TezRectangle();
        public override TezShape shape => rectangle;
        public TezRectangle rectangle => mRectangle;

        public TezQtRectangle()
        {

        }

        public TezQtRectangle(int ox, int oy, int width, int height)
        {
            mRectangle.originX = ox;
            mRectangle.originY = oy;
            mRectangle.width = width;
            mRectangle.height = height;
        }

        public override void close()
        {
            base.close();
            mRectangle = null;
        }
    }
}