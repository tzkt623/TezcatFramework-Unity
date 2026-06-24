using tezcat.Framework.BonusSystem;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezValueModifier
        : TezValueWrapper<float>
        , ITezValueModifier
        , ITezBonusCarrier
    {
        public event TezEventExtension.Action<ITezValueModifier, float> onChanged;

        public override ITezValueDescriptor descriptor
        {
            get { return this.modifierConfig.target; }
            set { }
        }

        /// <summary>
        /// 加成来源
        /// </summary>
        public object source { get; set; }

        /// <summary>
        /// 加成路径
        /// </summary>
        public TezBonusPath bonusPath { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public TezValueModifierConfig modifierConfig { get; set; }

        /// <summary>
        /// 加成模式
        /// 值模式
        /// 或者
        /// 功能模式
        /// </summary>
        public TezModifierType modifierType { get; } = TezModifierType.Value;

        /// <summary>
        /// 
        /// </summary>
        protected float mValue;

        /// <summary>
        /// 加成数据
        /// </summary>
        public override float value
        {
            get
            {
                return mValue;
            }
            set
            {
                var old = mValue;
                mValue = value;
                onChanged?.Invoke(this, old);
            }
        }

        protected TezValueModifier() : base(null)
        {

        }

        protected void notifyChanged(float oldValue)
        {
            onChanged?.Invoke(this, oldValue);
        }


        public override string ToString()
        {
            return string.Format("[{0}]\nSrc : {1}\nDef:\n======\n{2}\n======"
                , this.name
                , this.source.GetType().Name
                , bonusPath.ToString());
        }

        protected override void onClose()
        {
            base.onClose();
            this.modifierConfig.close();

            this.modifierConfig = null;
            this.source = null;
            this.bonusPath = null;
            this.onChanged = null;
        }
    }
}