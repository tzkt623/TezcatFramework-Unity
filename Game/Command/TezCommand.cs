namespace tezcat.Game
{
    public abstract class TezCommand
    {
        public int ID { get; set; }
        public TezCommandSequence sequence { get; set; }

        public virtual void onEnter()
        {

        }

        public virtual void onExit()
        {
            sequence.commandExit(this);
        }
    }
}
