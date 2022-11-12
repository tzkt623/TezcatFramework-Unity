using tezcat.Framework.Utility;

namespace tezcat.Unity.Utility
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
        UnityKeyWrapper mWrapper;
        public UnityBaseKeyConfig1(string name, UnityKeyWrapper wrapper) : base(name, 1)
        {
            mWrapper = wrapper;

            this.setWrapper(0, mWrapper);
        }

        public override void close()
        {
            base.close();
            mWrapper = null;
        }

        public override bool active()
        {
            return mWrapper.active();
        }
    }

    public class UnityBaseKeyConfig2 : UnityBaseKeyConfig
    {
        UnityKeyWrapper mWrapper1;
        UnityKeyWrapper mWrapper2;

        public UnityBaseKeyConfig2(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2) : base(name, 2)
        {
            mWrapper1 = wrapper1;
            mWrapper2 = wrapper2;

            this.setWrapper(0, mWrapper1);
            this.setWrapper(1, mWrapper2);
        }

        public override void close()
        {
            base.close();
            mWrapper1 = null;
            mWrapper2 = null;
        }

        public override bool active()
        {
            return mWrapper1.active() && mWrapper2.active();
        }
    }

    public class UnityBaseKeyConfig3 : UnityBaseKeyConfig
    {
        UnityKeyWrapper mWrapper1;
        UnityKeyWrapper mWrapper2;
        UnityKeyWrapper mWrapper3;

        public UnityBaseKeyConfig3(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2, UnityKeyWrapper wrapper3) : base(name, 3)
        {
            mWrapper1 = wrapper1;
            mWrapper2 = wrapper2;
            mWrapper3 = wrapper3;

            this.setWrapper(0, mWrapper1);
            this.setWrapper(1, mWrapper2);
            this.setWrapper(2, mWrapper3);
        }

        public override void close()
        {
            base.close();
            mWrapper1 = null;
            mWrapper2 = null;
            mWrapper3 = null;
        }

        public override bool active()
        {
            return mWrapper1.active() && mWrapper2.active() && mWrapper3.active();
        }
    }
}