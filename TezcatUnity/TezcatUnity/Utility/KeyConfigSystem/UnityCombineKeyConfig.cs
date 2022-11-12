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
        bool mLockKey = false;
        protected UnityCombineKeyConfig(string name, int keyCount) : base(name, keyCount)
        {

        }

        public sealed override bool active()
        {
            if (mLockKey)
            {
                ///如果锁住了,检测是否放开了
                mLockKey = this.onActive();
            }
            else
            {
                ///如果没有锁住,检测是否激活
                mLockKey = this.onActive();
                return mLockKey;
            }

            return false;
        }

        protected abstract bool onActive();
    }

    public class UnityAdvanceKeyConfig2 : UnityCombineKeyConfig
    {
        UnityKeyWrapper mWrapper1;
        UnityKeyWrapper mWrapper2;

        public UnityAdvanceKeyConfig2(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2) : base(name, 2)
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

        protected override bool onActive()
        {
            return mWrapper1.active() && mWrapper2.active();
        }
    }

    public class UnityAdvanceKeyConfig3 : UnityCombineKeyConfig
    {
        UnityKeyWrapper mWrapper1;
        UnityKeyWrapper mWrapper2;
        UnityKeyWrapper mWrapper3;

        public UnityAdvanceKeyConfig3(string name, UnityKeyWrapper wrapper1, UnityKeyWrapper wrapper2, UnityKeyWrapper wrapper3) : base(name, 3)
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

        protected override bool onActive()
        {
            return mWrapper1.active() && mWrapper2.active() && mWrapper3.active();
        }
    }
}