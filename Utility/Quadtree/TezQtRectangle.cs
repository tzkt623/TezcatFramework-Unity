using tezcat.Framework.Shape;

namespace tezcat.Framework.Utility
{
    public class TezQtRectangle : TezQtEntry
    {
        public override TezShape shape => rectangle;
        public TezRectangle rectangle { get; private set; } = new TezRectangle();

        public TezQtRectangle()
        {

        }

        public TezQtRectangle(int ox, int oy, int width, int height)
        {
            this.rectangle.originX = ox;
            this.rectangle.originY = oy;
            this.rectangle.width = width;
            this.rectangle.height = height;
        }

        public override void close()
        {
            base.close();
            this.rectangle = null;
        }
    }
}