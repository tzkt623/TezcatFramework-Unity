using System.Collections;

namespace tezcat.Unity.Utility
{
    /// <summary>
    /// 改进型组合按键配置
    /// 解决了press+press+...+down型最后一个按键顺序的问题
    /// 可以随意按键
    /// 但是会延迟一帧触发下一次按键效果
    /// </summary>
    public abstract class UnityCombineKeyConfig : UnityBaseKeyConfig
    {
        bool m_LockKey = false;
        protected UnityCombineKeyConfig(string name, int keyCount) : base(name, keyCount)
        {

        }

        public sealed override bool active()
        {
            if (m_LockKey)
            {
                ///如果锁住了,检测是否放开了
                m_LockKey = this.onActive();
            }
            else
            {
                ///如果没有锁住,检测是否激活
                m_LockKey = this.onActive();
                return m_LockKey;
            }

            return false;
        }

        protected abstract bool onActive();
    }

    public class UnityAdvanceKeyConfig2 : UnityCombineKeyConfig
    {
        UnityKeyWrapper m_Wrapper1;
        UnityKeyWrapper m_Wrapper2;

        public UnityAdvanceKeyConfig2(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2) : base(name, 2)
        {
            m_Wrapper1 = wrapper1;
            m_Wrapper2 = wrapper2;

            this.setWrapper(0, m_Wrapper1);
            this.setWrapper(1, m_Wrapper2);
        }

        public override void close()
        {
            base.close();
            m_Wrapper1 = null;
            m_Wrapper2 = null;
        }

        protected override bool onActive()
        {
            return m_Wrapper1.active() && m_Wrapper2.active();
        }
    }

    public class UnityAdvanceKeyConfig3 : UnityCombineKeyConfig
    {
        UnityKeyWrapper m_Wrapper1;
        UnityKeyWrapper m_Wrapper2;
        UnityKeyWrapper m_Wrapper3;

        public UnityAdvanceKeyConfig3(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2, UnityKeyWrapper wrapper3) : base(name, 3)
        {
            m_Wrapper1 = wrapper1;
            m_Wrapper2 = wrapper2;
            m_Wrapper3 = wrapper3;

            this.setWrapper(0, m_Wrapper1);
            this.setWrapper(1, m_Wrapper2);
            this.setWrapper(2, m_Wrapper3);
        }

        public override void close()
        {
            base.close();
            m_Wrapper1 = null;
            m_Wrapper2 = null;
            m_Wrapper3 = null;
        }

        protected override bool onActive()
        {
            return m_Wrapper1.active() && m_Wrapper2.active() && m_Wrapper3.active();
        }
    }
}