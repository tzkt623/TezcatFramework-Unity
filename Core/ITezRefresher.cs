namespace tezcat.Framework.Core
{
    public enum TezRefreshPhase : byte
    {
        P_Empty = 0,
        P_OnInit = 1,
        P_OnEnable = 1 << 1,
        P_Custom1 = 1 << 2,
        P_Custom2 = 1 << 3,
        P_Custom3 = 1 << 4,
        P_Custom4 = 1 << 5,
        P_Custom5 = 1 << 6
    }

    public interface ITezRefresher
    {
        TezRefreshPhase refreshPhase { set; }
        ITezRefresher next { get; set; }
        void refresh();
    }
}