namespace tezcat.Framework.Shape
{
    public class TezRectangle : TezShape
    {
        public override Category category => Category.Rectangle;

        public int maxX
        {
            get { return originX + width - 1; }
        }

        public int maxY
        {
            get { return originY + height - 1; }
        }

        public int midX
        {
            get { return originX + width / 2; }
        }

        public int midY
        {
            get { return originY + height / 2; }
        }

        public int width = 0;
        public int height = 0;

        public override bool contains(TezShape shape)
        {
            switch (shape.category)
            {
                case Category.Point:
                    return shape.originX >= originX
                        && shape.originY >= originY
                        && shape.originX <= maxX
                        && shape.originY <= maxY;

                case Category.Circle:
                    var circle = (TezCircle)shape;
                    var max_x = circle.originX + circle.radius;
                    var max_y = circle.originY + circle.radius;
                    var min_x = circle.originX - circle.radius;
                    var min_y = circle.originY - circle.radius;

                    return min_x >= originX
                        && min_y >= originY
                        && max_x <= maxX
                        && max_y <= maxY;

                case Category.Rectangle:
                    var rect = (TezRectangle)shape;

                    return rect.originX >= originX
                        && rect.originY >= originY
                        && rect.maxX <= maxX
                        && rect.maxY <= maxY;
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
                    return shape.originX >= originX
                        && shape.originY >= originY
                        && shape.originX <= maxX
                        && shape.originY <= maxY;

                case Category.Circle:
                    return false;

                case Category.Rectangle:
                    var rect = (TezRectangle)shape;

                    return !(rect.originX > maxX
                        || rect.originY > maxY
                        || rect.maxX < originX
                        || rect.maxY < originY);

                default:
                    break;
            }

            return false;
        }
    }
}