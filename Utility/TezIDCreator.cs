namespace tezcat.Framework.Utility
{
    /// <summary>
    /// ID生成器
    /// 生成静态整数ID
    /// 用类型T来区别类型
    /// </summary>
    public static class TezIDCreator<T>
    {
        static int m_ID = -1;

        public static int count
        {
            get { return m_ID; }
        }

        public static int create()
        {
            return m_ID++;
        }

        public static void reset()
        {
            m_ID = -1;
        }
    }
}