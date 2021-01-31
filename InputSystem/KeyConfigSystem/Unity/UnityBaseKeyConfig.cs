using tezcat.Framework.Database;

namespace tezcat.Framework.InputSystem
{
    /// <summary>
    /// 通用型按键配置
    /// 可以单个
    /// 可以组合
    /// 组合按键必须是press+press+...+down的配置模式
    /// 并且存在最后一个按下的按键必须为down按键的触发方式
    /// 不想用这样的方式请使用CombineKeyConfig
    /// </summary>
    public abstract class UnityBaseKeyConfig : TezKeyConfig
    {
        public UnityBaseKeyConfig(string name, int keyCount) : base(name, keyCount)
        {

        }
    }

    public class UnityBaseKeyConfig1 : UnityBaseKeyConfig
    {
        UnityKeyWrapper m_Wrapper;
        public UnityBaseKeyConfig1(string name, UnityKeyWrapper wrapper) : base(name, 1)
        {
            m_Wrapper = wrapper;

            this.setWrapper(0, m_Wrapper);
        }

        public override void close()
        {
            base.close();
            m_Wrapper = null;
        }

        public override bool active()
        {
            return m_Wrapper.active();
        }
    }

    public class UnityBaseKeyConfig2 : UnityBaseKeyConfig
    {
        UnityKeyWrapper m_Wrapper1;
        UnityKeyWrapper m_Wrapper2;

        public UnityBaseKeyConfig2(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2) : base(name, 2)
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

        public override bool active()
        {
            return m_Wrapper1.active() && m_Wrapper2.active();
        }
    }

    public class UnityBaseKeyConfig3 : UnityBaseKeyConfig
    {
        UnityKeyWrapper m_Wrapper1;
        UnityKeyWrapper m_Wrapper2;
        UnityKeyWrapper m_Wrapper3;

        public UnityBaseKeyConfig3(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2, UnityKeyWrapper wrapper3) : base(name, 3)
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

        public override bool active()
        {
            return m_Wrapper1.active() && m_Wrapper2.active() && m_Wrapper3.active();
        }
    }
}