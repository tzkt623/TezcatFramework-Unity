using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public abstract class TezBaseModifierCache : ITezCloseable
    {
        //        public bool dirty { get; set; } = false;
        List<ITezModifier> mModifiers = new List<ITezModifier>();

        public void addModifier(ITezModifier modifier)
        {
            this.onModifierAdded(modifier);
            mModifiers.Add(modifier);
        }

        public bool removeModifier(ITezModifier modifier)
        {
            if (mModifiers.Remove(modifier))
            {
                this.onModifierRemoved(modifier);
                return true;
            }

            return false;
        }

        public bool removeAllModifiersFrom(object source)
        {
            int count = mModifiers.Count;
            for (int i = mModifiers.Count - 1; i >= 0; i--)
            {
                if (mModifiers[i].source == source)
                {
                    this.onModifierRemoved(mModifiers[i]);
                    mModifiers.RemoveAt(i);
                }
            }

            return count != mModifiers.Count;
        }

        /// <summary>
        /// 清理cache到初始状态
        /// </summary>
        public virtual void clearModifiers()
        {
            foreach (var modifier in mModifiers)
            {
                this.onModifierClear(modifier);
            }
            mModifiers.Clear();
        }

        /// <summary>
        /// Modifier一定都来源于其他系统的加成
        /// 所以Cache只负责记录
        /// 不负责清理
        /// </summary>
        public virtual void close()
        {
            foreach (var modifier in mModifiers)
            {
                this.onModifierClear(modifier);
            }
            mModifiers.Clear();
            mModifiers = null;
        }

        protected abstract void onModifierAdded(ITezModifier modifier);
        protected abstract void onModifierRemoved(ITezModifier modifier);
        protected abstract void onModifierClear(ITezModifier modifier);
    }

    public abstract class TezBaseFunctionModifierCache : TezBaseModifierCache
    {

    }

    public abstract class TezBaseValueModifierCache<T> : TezBaseModifierCache
    {
        protected sealed override void onModifierAdded(ITezModifier modifier)
        {
            ITezValueModifier vm = (ITezValueModifier)modifier;
            this.onModifierAdded(vm);
            vm.onChanged += this.changeModifier;
        }

        protected sealed override void onModifierRemoved(ITezModifier modifier)
        {
            ITezValueModifier vm = (ITezValueModifier)modifier;
            this.onModifierRemoved(vm);
            vm.onChanged -= this.changeModifier;
        }

        protected override void onModifierClear(ITezModifier modifier)
        {
            ITezValueModifier vm = (ITezValueModifier)modifier;
            vm.onChanged -= this.changeModifier;
        }

        private void changeModifier(ITezValueModifier modifier, float oldValue)
        {
            this.onModifierChanged(modifier, oldValue);
        }

        protected abstract void onModifierAdded(ITezValueModifier valueModifier);
        protected abstract void onModifierRemoved(ITezValueModifier valueModifier);
        protected abstract void onModifierChanged(ITezValueModifier valueModifier, float oldValue);

        public abstract T calculate(ITezProperty<T> property);
    }
}