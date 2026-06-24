namespace tezcat.Framework.Shape
{
    public class TezCircle : TezShape
    {
        public override Category category => Category.Circle;

        public int radius;

        public override bool contains(TezShape circle)
        {
            return false;
        }

        public override bool intersects(TezShape shape)
        {
            return false;
        }
    }
}