namespace tezcat.Framework.Shape
{
    public class TezPoint : TezShape
    {
        public override Category category => Category.Point;

        public override bool contains(TezShape shape)
        {
            switch (shape.category)
            {
                case Category.Point:
                    return originX == shape.originX && originY == shape.originY;
                case Category.Rectangle:
                    break;
                case Category.Circle:
                    break;
                default:
                    break;
            }

            return false;
        }

        public override bool intersects(TezShape shape)
        {
            switch (shape.category)
            {
                case Category.Point:
                    return originX == shape.originX && originY == shape.originY;
                case Category.Rectangle:
                    var rect = (TezRectangle)shape;
                    return originX >= rect.originX
                        && originY >= rect.originY
                        && originX <= rect.maxX
                        && originY <= rect.maxY;
                case Category.Circle:
                    break;
                default:
                    break;
            }

            return false;
        }
    }
}