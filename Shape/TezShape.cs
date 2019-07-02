namespace tezcat.Framework.Shape
{
    public abstract class TezShape
    {
        public enum Category
        {
            Point,
            Rectangle,
            Circle
        }

        public int originX = 0;
        public int originY = 0;

        public abstract Category category { get; }

        public abstract bool contains(TezShape shape);
        public abstract bool intersects(TezShape shape);
    }
}