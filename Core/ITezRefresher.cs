namespace tezcat.Framework.Core
{
    public enum TezRefreshPhase : byte
    {
        /// <summary>
        /// 空阶段
        /// 不要用此参数来刷新
        /// </summary>
        P_Empty = 0,

        /// <summary>
        /// 控件初始化时刷新
        /// </summary>
        P_OnInit = 1,

        /// <summary>
        /// 控件被激活时刷新
        /// </summary>
        P_OnEnable = 1 << 1,

        /// <summary>
        /// 自定义刷新阶段1
        /// </summary>
        P_Custom1 = 1 << 2,

        /// <summary>
        /// 自定义刷新阶段2
        /// </summary>
        P_Custom2 = 1 << 3,

        /// <summary>
        /// 自定义刷新阶段3
        /// </summary>
        P_Custom3 = 1 << 4,

        /// <summary>
        /// 自定义刷新阶段4
        /// </summary>
        P_Custom4 = 1 << 5,

        /// <summary>
        /// 自定义刷新阶段5
        /// </summary>
        P_Custom5 = 1 << 6
    }

    public interface ITezRefresher
    {
        TezRefreshPhase refreshPhase { set; }
        ITezRefresher next { get; set; }
        void refresh();
    }
}