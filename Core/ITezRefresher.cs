namespace tezcat.Framework.Core
{
    public enum TezRefreshPhase : byte
    {
        /// <summary>
        /// 准备阶段
        /// 不要用此参数来刷新
        /// </summary>
        Ready = 0,

        Refresh
    }

    public interface ITezRefresher
    {
        TezRefreshPhase refreshPhase { set; }
        //        ITezRefresher next { get; set; }
        void refresh();
    }

    public class TezRefreshMask
    {
        public const byte Inited = 1;
        public const byte Closed = 1 << 1;
    }
}