namespace tezcat.Framework.Game
{
    public enum PickState
    {
        RayCast,
        MouseLeft,
        MouseRight,
    }

    public interface ITezPickable 
    {
        void onPicked(PickState pick_state);
    }
}