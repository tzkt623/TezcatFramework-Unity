using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 带状态的验证器
    /// 
    /// 与普通验证器相同
    /// 只是可以携带32种状态(uint)进行验证
    /// 
    /// </summary>
    public class TezStateMonitor<UserData>
        : ITezCloseable
        where UserData : class
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


        void ITezCloseable.deleteThis()
        {
            mFlagData.release();
            mFlagData = null;
        }
    }

    public interface ITezStateMonitor64 : ITezCloseable
    {
        bool noneOf(ulong mask);

        bool anyOf(ulong mask);

        bool allOf(ulong mask);

        void addStateMask(ulong mask);

        void removeStateMask(ulong mask);

        void clearStateMask();

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData_AnyOf(ulong mask, out object userData);

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData_AllOf(ulong mask, out object userData);

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData_NoneOf(ulong mask, out object userData);
    }

    /// <summary>
    /// 带状态的验证器
    /// 
    /// 与普通验证器相同
    /// 只是可以携带32种状态(ulong)进行验证
    /// 
    /// </summary>
    public class TezStateMonitor64<UserData>
        : ITezStateMonitor64
        where UserData : class
    {
        class Data
        {
            int m_Ref = 0;
            ulong m_StateMask = 0;
            public UserData userData;

            public Data() { }

            public void add(ulong value)
            {
                m_StateMask |= value;
            }

            public void remove(ulong value)
            {
                m_StateMask &= ~value;
            }

            public bool allOf(ulong value)
            {
                return (m_StateMask & value) == value;
            }

            public bool anyOf(ulong value)
            {
                return (m_StateMask & value) > 0;
            }

            public bool noneOf(ulong value)
            {
                return (m_StateMask & value) == 0;
            }

            public void retain()
            {
                m_Ref++;
            }

            public void release()
            {
                m_Ref--;
                if (m_Ref == 0)
                {
                    this.userData = null;
                }
            }

            public void reset()
            {
                m_StateMask = 0;
            }
        }

        Data m_Valider = null;

        /// <summary>
        /// 创建一个新的Valider
        /// 以及新的共享数据
        /// </summary>
        public TezStateMonitor64()
        {
            m_Valider = new Data();
            m_Valider.retain();
        }

        /// <summary>
        /// 从另一个Valider中获得共享数据
        /// 用于监视
        /// 不会新创建贡献数据
        /// </summary>
        public TezStateMonitor64(TezStateMonitor64<UserData> entry)
        {
            this.hold(entry);
        }

        /// <summary>
        /// 持有一个新的entry
        /// </summary>
        public void hold(TezStateMonitor64<UserData> entry)
        {
            m_Valider?.release();
            m_Valider = entry.m_Valider;
            m_Valider.retain();
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_AnyOf(ulong mask, out UserData userData)
        {
            if (m_Valider.anyOf(mask))
            {
                userData = m_Valider.userData;
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
        public bool tryGetUserData_AnyOf<T>(ulong mask, out T userData) where T : class, UserData
        {
            if (m_Valider.anyOf(mask))
            {
                userData = (T)m_Valider.userData;
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
        public bool tryGetUserData_AllOf(ulong mask, out UserData userData)
        {
            if (m_Valider.allOf(mask))
            {
                userData = m_Valider.userData;
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
        public bool tryGetUserData_AllOf<T>(ulong mask, out T userData) where T : class, UserData
        {
            if (m_Valider.allOf(mask))
            {
                userData = (T)m_Valider.userData;
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
        public bool tryGetUserData_NoneOf(ulong mask, out UserData userData)
        {
            if (m_Valider.noneOf(mask))
            {
                userData = m_Valider.userData;
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
        public bool tryGetUserData_NoneOf<T>(ulong mask, out T userData) where T : class, UserData
        {
            if (m_Valider.noneOf(mask))
            {
                userData = (T)m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        public bool allOf(ulong mask)
        {
            return m_Valider.allOf(mask);
        }

        public bool noneOf(ulong mask)
        {
            return m_Valider.noneOf(mask);
        }

        public bool anyOf(ulong mask)
        {
            return m_Valider.anyOf(mask);
        }

        /// <summary>
        /// 添加一种状态
        /// </summary>
        public void addStateMask(ulong mask)
        {
            m_Valider.add(mask);
        }

        /// <summary>
        /// 移除一种状态
        /// </summary>
        public void removeStateMask(ulong mask)
        {
            m_Valider.remove(mask);
        }

        /// <summary>
        /// 清空所有状态
        /// </summary>
        public void clearStateMask()
        {
            m_Valider.reset();
        }


        void ITezCloseable.deleteThis()
        {
            m_Valider.release();
            m_Valider = null;
        }

        bool ITezStateMonitor64.tryGetUserData_AllOf(ulong mask, out object userData)
        {
            if (m_Valider.allOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        bool ITezStateMonitor64.tryGetUserData_AnyOf(ulong mask, out object userData)
        {
            if (m_Valider.anyOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        bool ITezStateMonitor64.tryGetUserData_NoneOf(ulong mask, out object userData)
        {
            if (m_Valider.noneOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }
    }
}
