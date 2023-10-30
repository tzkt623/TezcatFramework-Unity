namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 带状态的验证器
    /// 
    /// 与普通验证器相同
    /// 只是可以携带32种状态(uint)进行验证
    /// 
    /// </summary>
    public class TezStateMonitor<UserData> where UserData : class
    {
        class FlagData
        {
            int mRef = 0;
            uint mStateMask = 0;
            public UserData userData;

            public FlagData() { }

            public void add(uint value)
            {
                mStateMask |= value;
            }

            public void remove(uint value)
            {
                mStateMask &= ~value;
            }

            public bool allOf(uint value)
            {
                return (mStateMask & value) == value;
            }

            public bool anyOf(uint value)
            {
                return (mStateMask & value) > 0;
            }

            public bool noneOf(uint value)
            {
                return (mStateMask & value) == 0;
            }

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

            public void reset()
            {
                mStateMask = 0;
            }
        }

        FlagData mFlagData = null;

        /// <summary>
        /// 创建一个新的Valider
        /// 以及新的共享数据
        /// </summary>
        public TezStateMonitor(UserData userData)
        {
            mFlagData = new FlagData()
            {
                userData = userData
            };
            mFlagData.retain();
        }

        /// <summary>
        /// 从另一个Valider中获得共享数据
        /// 用于监视
        /// 不会新创建贡献数据
        /// </summary>
        public TezStateMonitor(TezStateMonitor<UserData> other)
        {
            this.hold(other);
        }

        /// <summary>
        /// 持有一个新的entry
        /// </summary>
        public void hold(TezStateMonitor<UserData> other)
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
        public bool tryGetUserData_AnyOf(uint mask, out UserData userData)
        {
            if (mFlagData.anyOf(mask))
            {
                userData = mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_AnyOf<T>(uint mask, out T userData) where T : class, UserData
        {
            if (mFlagData.anyOf(mask))
            {
                userData = (T)mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_AllOf(uint mask, out UserData userData)
        {
            if (mFlagData.allOf(mask))
            {
                userData = mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_AllOf<T>(uint mask, out T userData) where T : class, UserData
        {
            if (mFlagData.allOf(mask))
            {
                userData = (T)mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_NoneOf(uint mask, out UserData userData)
        {
            if (mFlagData.noneOf(mask))
            {
                userData = mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_NoneOf<T>(uint mask, out T userData) where T : class, UserData
        {
            if (mFlagData.noneOf(mask))
            {
                userData = (T)mFlagData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        public bool allOf(uint mask)
        {
            return mFlagData.allOf(mask);
        }

        public bool noneOf(uint mask)
        {
            return mFlagData.noneOf(mask);
        }

        public bool anyOf(uint mask)
        {
            return mFlagData.anyOf(mask);
        }

        /// <summary>
        /// 添加一种状态
        /// </summary>
        public void addStateMask(uint mask)
        {
            mFlagData.add(mask);
        }

        /// <summary>
        /// 移除一种状态
        /// </summary>
        public void removeStateMask(uint mask)
        {
            mFlagData.remove(mask);
        }

        /// <summary>
        /// 清空所有状态
        /// </summary>
        public void clearStateMask()
        {
            mFlagData.reset();
        }


        public void close()
        {
            mFlagData.release();
            mFlagData = null;
        }
    }
}
