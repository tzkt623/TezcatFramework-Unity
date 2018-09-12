namespace tezcat.Game
{
    public class TezDungeonBlock
    {
        public enum State
        {
            CanPass,
            CanNotPass
        }

        public int x { get; private set; }
        public int y { get; private set; }

        public bool isInited { get; set; } = false;
        public State state { get; set; } = State.CanNotPass;

        public TezDungeonBlock(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void init()
        {

        }
    }
}