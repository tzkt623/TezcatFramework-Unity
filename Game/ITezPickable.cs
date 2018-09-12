namespace tezcat.Game
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