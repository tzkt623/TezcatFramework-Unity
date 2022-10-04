namespace tezcat.Unity.Core
{
    /// <summary>
    /// 刷新阶段
    /// </summary>
    public enum TezRefreshPhase : byte
    {
        /// <summary>
        /// 准备阶段
        /// 不要用此参数来刷新
        /// </summary>
        Ready = 0,
        Refresh
    }

    /// <summary>
    /// 刷新者
    /// </summary>
    public interface ITezRefreshHandler
    {
        void needRefresh();
        //        ITezRefresher next { get; set; }
        void refresh();
    }

    public interface ITezDelayInitHandler
    {
        void delayInit();
    }

    /// <summary>
    /// 刷新标志
    /// </summary>
    public class TezRefreshMask
    {
        public const byte Inited = 1;
        public const byte Closed = 1 << 1;
    }
}