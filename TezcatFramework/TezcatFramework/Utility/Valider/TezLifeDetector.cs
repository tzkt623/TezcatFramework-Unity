namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 对象生命周期探测器
    /// 
    /// <para>
    /// 思路来源于智能指针
    /// 用于验证UserData的有效性
    /// </para>
    /// 
    /// <para>
    /// 比如你发射了一颗导弹
    /// 导弹飞行途中目标已经爆了
    /// 这时你需要让导弹寻找下一个目标
    /// 或者给定一个虚空目标
    /// 或者原地爆炸
    /// 这个验证器便可以知道目标的状态
    /// 以判断是否需要操作
    /// </para>
    /// 
    /// </summary>
    public class TezLifeDetector<UserData> where UserData : class
    {
        class FlagData
        {
            int mRef = 0;
            public bool valid = false;
            public UserData userData = null;

            public FlagData() { }

            public void retain()
            {
                mRef++;
            }

            public void release()
            {
                mRef--;
                if (mRef == 0)
                {
                    this.userData = null;
                }
            }
        }

        FlagData mFlagData = null;

        /// <summary>
        /// 
        /// </summary>
        public UserData owner => mFlagData.userData;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool isValied => mFlagData.valid;

        /// <summary>
        /// 创建一个新的Detector
        /// 以及新的共享数据
        /// </summary>
        public TezLifeDetector(UserData userData)
        {
            mFlagData = new FlagData()
            {
                userData = userData,
                valid = userData == null ? false : true
            };
            mFlagData.retain();
        }

        /// <summary>
        /// 从另一个Detector中获得共享数据
        /// 用于监视
        /// 不会新创建贡献数据
        /// </summary>
        public TezLifeDetector(TezLifeDetector<UserData> other)
        {
            this.hold(other);
        }

        /// <summary>
        /// 持有一个新的Detector
        /// </summary>
        public void hold(TezLifeDetector<UserData> other)
        {
            mFlagData?.release();
            mFlagData = other.mFlagData;
            mFlagData.retain();
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData(out UserData userData)
        {
            if (mFlagData.valid)
            {
                userData = mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        public bool tryGetUserData<T>(out T userData) where T : class, UserData
        {
            if (mFlagData.valid)
            {
                userData = (T)mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 将内部共享数据
        /// 即UserData设置为失效状态
        /// 其他共享对象就无法使用其进行计算
        /// </summary>
        public void setInvalid()
        {
            mFlagData.valid = false;
        }

        public void setValid()
        {
            mFlagData.valid = true;
        }

        public void close()
        {
            mFlagData.release();
            mFlagData = null;
        }
    }
}
