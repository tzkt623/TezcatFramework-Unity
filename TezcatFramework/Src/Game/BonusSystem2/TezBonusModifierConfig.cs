namespace tezcat.Framework.Game
{
    public class TezBonusModifierConfig
    {
        //默认100倍缩放
        //也就是modifier=100时,实际加成是1倍

        public enum Ratio : int
        {
            R1 = 1,
            R10 = 10,
            R100 = 100,
            R1000 = 1000,
            R10000 = 10000,
        }

        static int mScale = 1;
        public static int scale => mScale;

        static float mReciprocal = 1;
        public static float reciprocal => mReciprocal;

        public static void init(Ratio ratio)
        {
            mScale = (int)ratio;
            mReciprocal = 1.0f / mScale;
        }
    }

    public enum TezBonusModifierCalculateRule : byte
    {
        /// <summary>
        /// 在基础值上加
        /// </summary>
        Base_SumAdd = 0,

        /// <summary>
        /// 在基础值上以百分比相加
        /// </summary>
        Base_PercentAdd,

        /// <summary>
        /// 在Base加成后值上加
        /// </summary>
        Total_SumAdd,

        /// <summary>
        /// 在Base加成后值上以百分比相加
        /// </summary>
        Total_PercentAdd,
    }
}