namespace tezcat.Framework.Core
{
    /// <summary>
    /// 代理型Modifier
    /// 
    /// <para>
    /// 其属性值来源于另一个Property
    /// 其Value值不能被直接Set
    /// </para>
    /// 
    /// <para>
    /// 他的生命周期不同于普通ValueModifier
    /// ValueModifier生命周期属于其管理者
    /// AgentValueModifier生命周期属于自己
    /// 因为他的来源Property自身拥有管理者
    /// </para>
    /// 
    /// <para>
    /// 例子
    /// A单位使用自身1号属性对B单位增加了一个持续Buff
    /// </para>
    /// 
    /// <para>
    /// 情况1:
    /// 这时A单位死亡了,就必须通知B单位Buff没了
    /// 这时必须通过一个中介,也就是buff对象来通知B单位buff会消失并且移除此代理属性
    /// </para>
    /// 
    /// <para>
    /// 情况2:
    /// 这时A单位受到一个加成使1号属性变强了,那么对应的B身上的buff也应该变强
    /// 这时有两种处理方法
    /// 1.通知B身上的buff要变强
    /// 2.不通知B身上的buff,需要重新释放一次才能更新
    /// </para>
    /// 
    /// </summary>
    public class TezAgentValueModifier : TezValueModifier
    {
        ITezProperty mProperty = null;

        public override float value
        {
            get
            {
                return mValue;
            }
            set
            {
                ///引用型Modifier不能被直接Set
                ///throw new MethodAccessException(string.Format("{0} : 引用型Modifier不能被直接Set", this.GetType().Name));
            }
        }

        public TezAgentValueModifier(ITezProperty property) : base()
        {
            mProperty = property;
            mProperty.evtValueChanged += onMainValueChanged;
            this.resetValue();
        }

        public void resetValue()
        {
            switch (mProperty.valueType)
            {
                case TezValueType.Int:
                    mValue = ((ITezProperty<int>)mProperty).value;
                    break;
                case TezValueType.Float:
                    mValue = ((ITezProperty<float>)mProperty).value;
                    break;
            }
        }

        private void onMainValueChanged(ITezProperty property)
        {
            var old = mValue;
            this.resetValue();
            this.notifyChanged(old);
        }

        protected override void onClose()
        {
            base.onClose();
            mProperty.evtValueChanged -= onMainValueChanged;
            mProperty = null;
        }
    }
}