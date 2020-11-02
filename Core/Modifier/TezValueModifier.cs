using tezcat.Framework.Definition;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezValueModifier
        : TezValueWrapper<float>
        , ITezValueModifier
    {
        /// <summary>
        /// 加成来源
        /// </summary>
        public object source { get; set; }

        /// <summary>
        /// 加成路径定义
        /// </summary>
        public TezDefinition definition { get; set; }

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
        /// <para>TezModifier --> Self</para> 
        /// <para>float --> old value</para>
        /// </summary>
        public event TezEventExtension.Action<ITezValueModifier, float> onValueChanged;

        protected float m_Value;
        public override float value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (m_Value != value)
                {
                    var old = m_Value;
                    m_Value = value;
                    onValueChanged?.Invoke(this, old);
                }
            }
        }

        protected TezValueModifier(ITezValueDescriptor name) : base(name)
        {

        }

        protected void notifyValueChanged(ITezValueModifier modifier, float old_value)
        {
            onValueChanged?.Invoke(modifier, old_value);
        }

        public override string ToString()
        {
            return string.Format("[{0}]\nSrc : {1}\nDef:\n======\n{2}\n======"
                , this.name
                , this.source.GetType().Name
                , definition.ToString());
        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);
            this.definition.close(self_close);
            this.modifierConfig.close(self_close);

            this.modifierConfig = null;
            this.source = null;
            this.definition = null;
            onValueChanged = null;
        }
    }
}