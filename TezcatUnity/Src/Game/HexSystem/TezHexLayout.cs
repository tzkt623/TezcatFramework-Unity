namespace tezcat.Framework.Game
{
    public struct TezHexLayout
    {
        public TezHexOrientation orientation;
        public TezHexVector2 size;
        public TezHexVector2 origin;

        public TezHexLayout(TezHexOrientation orientation, TezHexVector2 size, TezHexVector2 origin)
        {
            this.orientation = orientation;
            this.size = size;
            this.origin = origin;
        }
    }
}