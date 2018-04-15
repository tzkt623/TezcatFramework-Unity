namespace tezcat.Utility
{
    public interface ITezRefresher
    {
        bool dirty { get; set; }
        void onRefresh();
    }
}

