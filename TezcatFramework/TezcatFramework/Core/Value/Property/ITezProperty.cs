using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezProperty : ITezValueWrapper
    {
        /// <summary>
        /// 更新事件
        /// </summary>
        event TezEventExtension.Action<ITezProperty> onValueChanged;

        void addModifier(ITezValueModifier modifier);
        bool removeModifier(ITezValueModifier modifier);

        /// <summary>
        /// 调用此方法时
        /// 只有数据需要重新计算时,才会重新计算
        /// 但一定会发出更新事件
        /// </summary>
        void update();

        string baseValueToString();
    }

    public interface ITezProperty<T> : ITezProperty
    {
        T baseValue { get; set; }
        T value { get; }
    }
}