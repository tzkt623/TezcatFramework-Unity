using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusModifierContainer : ITezCloseable
    {
        bool isDirty { get; }
        void setDirty();

        void addModifier(ITezBonusModifier modifier);
        bool removeModifier(ITezBonusModifier modifier);
        void removeModifierByModifierClosed(ITezBonusModifier modifier);

        int calculate(ref int baseValue);
        float calculate(ref float baseValue);

        void clear();
    }


    public interface ITezBonusModifierCalculator : ITezCloseable
    {
        bool tryAdd(ITezBonusModifierContainer master, ITezBonusModifier modifier);
        bool tryRemove(ITezBonusModifierContainer master, ITezBonusModifier modifier);
        int calculate(ref int baseValue);
        float calculate(ref float baseValue);
        void clear(ITezBonusModifierContainer container);
        void removeModifierByModifierClosed(ITezBonusModifier removeModifier);
    }

    /*
    public class TezBonusModifierCalculator_Value : ITezBonusModifierCalculator
    {
        float mBase_SumAdd = 0;
        float mBase_PercentAdd = 0;
        float mTotal_SumAdd = 0;
        float mTotal_PercentAdd = 0;

        bool ITezBonusModifierCalculator.tryAdd(ITezBonusModifierContainer master, ITezBonusModifier modifier)
        {
            switch (modifier.calculateRule)
            {
                case TezBonusModifierCalculateRule.Base_SumAdd:
                    mBase_SumAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentAdd:
                    mBase_PercentAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_SumAdd:
                    mTotal_SumAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_PercentAdd:
                    mTotal_PercentAdd += modifier.value;
                    break;
                default:
                    break;
            }

            return true;
        }

        bool ITezBonusModifierCalculator.tryRemove(ITezBonusModifierContainer master, ITezBonusModifier modifier)
        {
            switch (modifier.calculateRule)
            {
                case TezBonusModifierCalculateRule.Base_SumAdd:
                    mBase_SumAdd -= modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentAdd:
                    mBase_PercentAdd -= modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_SumAdd:
                    mTotal_SumAdd -= modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_PercentAdd:
                    mTotal_PercentAdd -= modifier.value;
                    break;
                default:
                    break;
            }

            return true;
        }

        void ITezBonusModifierCalculator.removeModifierByModifierClosed(ITezBonusModifier removeModifier)
        {
            switch (removeModifier.calculateRule)
            {
                case TezBonusModifierCalculateRule.Base_SumAdd:
                    mBase_SumAdd -= removeModifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentAdd:
                    mBase_PercentAdd -= removeModifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_SumAdd:
                    mTotal_SumAdd -= removeModifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_PercentAdd:
                    mTotal_PercentAdd -= removeModifier.value;
                    break;
                default:
                    break;
            }
        }

        int ITezBonusModifierCalculator.calculate(ref int baseValue)
        {
            var base_temp = mBase_SumAdd + baseValue * (1.0f + mBase_PercentAdd);
            return (int)(mTotal_SumAdd + base_temp * (1.0f + mTotal_PercentAdd));
        }

        float ITezBonusModifierCalculator.calculate(ref float baseValue)
        {
            var base_temp = mBase_SumAdd + baseValue * (1.0f + mBase_PercentAdd);
            return mTotal_SumAdd + base_temp * (1.0f + mTotal_PercentAdd);
        }

        void ITezBonusModifierCalculator.clear(ITezBonusModifierContainer container)
        {
            mBase_SumAdd = 0;
            mBase_PercentAdd = 0;
            mTotal_SumAdd = 0;
            mTotal_PercentAdd = 0;
        }

        void ITezCloseable.close()
        {

        }
    }
    */


    public class TezBonusModifierCalculator_List : ITezBonusModifierCalculator
    {
        List<TezBonusModifierEntry> mFixed = new List<TezBonusModifierEntry>();
        List<TezBonusModifierEntry> mFixed2 = new List<TezBonusModifierEntry>();
        List<TezBonusModifierEntry> mCurrent = null;
        List<TezBonusModifierEntry> mNext = null;

        public TezBonusModifierCalculator_List()
        {
            mCurrent = mFixed;
            mNext = mFixed2;
        }

        bool ITezBonusModifierCalculator.tryAdd(ITezBonusModifierContainer masterContainer, ITezBonusModifier addingModifier)
        {
            //一个Modifier只能被加入一个Container中一次
            if (addingModifier.tryAddContainer(masterContainer, out var entry))
            {
                mCurrent.Add(entry);
                return true;
            }

            return false;
        }

        bool ITezBonusModifierCalculator.tryRemove(ITezBonusModifierContainer masterContainer, ITezBonusModifier removingModifier)
        {
            //
            if(removingModifier.tryRemoveContainer(masterContainer, out var entry))
            {
                entry.setModifier(null);
                return true;
            }
            
            return false;
        }

        void ITezBonusModifierCalculator.removeModifierByModifierClosed(ITezBonusModifier removeModifier)
        {
            //这里是被动关闭
            //这里不做任何事情,是为了兼容上面纯数值计算器,因为上面纯数值计算器是直接把modifier的value减掉
            //此时modifier里保存的entry会自动把modifier置为null
            //下次计算时会被抛弃
        }

        private void updateList(ref float baseSumAdd, ref float basePercentAdd, ref float totalSumAdd, ref float totalPercentAdd)
        {
            ITezBonusModifier modifier = null;
            foreach (var entry in mCurrent)
            {
                modifier = entry.modifier;
                //如果有需要删除的Modifier,在更新后抛弃Modifier
                //此时Modifier本身已经不包含此Container
                if (entry.isNeedRemove)
                {
                    TezBonusModifierEntry.recycle(entry);
                }
                else
                {
                    mNext.Add(entry);
                    switch (modifier.calculateRule)
                    {
                        case TezBonusModifierCalculateRule.Base_SumAdd:
                            baseSumAdd += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Base_PercentAdd:
                            basePercentAdd += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Total_SumAdd:
                            totalSumAdd += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Total_PercentAdd:
                            totalPercentAdd += modifier.value;
                            break;
                        default:
                            break;
                    }
                }
            }
            mCurrent.Clear();

            var temp = mCurrent;
            mCurrent = mNext;
            mNext = temp;
        }

        int ITezBonusModifierCalculator.calculate(ref int baseValue)
        {
            float base_sum_add = 0;
            float base_percent_add = 0;
            float total_sum_add = 0;
            float total_percent_add = 0;

            this.updateList(ref base_sum_add, ref base_percent_add, ref total_sum_add, ref total_percent_add);

            var base_temp = base_sum_add + baseValue * (1.0f + base_percent_add);
            return (int)(total_sum_add + base_temp * (1.0f + total_percent_add));
        }

        float ITezBonusModifierCalculator.calculate(ref float baseValue)
        {
            float base_sum_add = 0;
            float base_percent_add = 0;
            float total_sum_add = 0;
            float total_percent_add = 0;

            this.updateList(ref base_sum_add, ref base_percent_add, ref total_sum_add, ref total_percent_add);

            var base_temp = base_sum_add + baseValue * (1.0f + base_percent_add);
            return total_sum_add + base_temp * (1.0f + total_percent_add);
        }

        void ITezBonusModifierCalculator.clear(ITezBonusModifierContainer container)
        {
            foreach (var item in mCurrent)
            {
                item.notifyByContainerIsCleared(container);
            }
            mCurrent.Clear();
        }

        void ITezCloseable.close()
        {
            mFixed.Clear();
            mFixed2.Clear();

            mFixed = null;
            mFixed2 = null;
        }
    }

    public class TezBonusModifierContainer : ITezBonusModifierContainer
    {
        ITezBonusModifierCalculator mModifierCalculator = null;

        bool mIsDirty = false;
        bool ITezBonusModifierContainer.isDirty => mIsDirty;

        public TezBonusModifierContainer(ITezBonusModifierCalculator modifierCalculator)
        {
            mModifierCalculator = modifierCalculator;
        }

        public TezBonusModifierContainer()
        {
            mModifierCalculator = new TezBonusModifierCalculator_List();
        }

        void ITezBonusModifierContainer.setDirty()
        {
            mIsDirty = true;
        }

        void ITezBonusModifierContainer.addModifier(ITezBonusModifier addModifier)
        {
            //一个Modifier只能被加入一个Container中一次
            //先让calculator计算一下
            if (mModifierCalculator.tryAdd(this, addModifier))
            {
                mIsDirty = true;
            }
            else
            {
                throw new System.Exception($"Modifier {addModifier} already has a master, cannot add to this container.");
            }
        }

        bool ITezBonusModifierContainer.removeModifier(ITezBonusModifier removeModifier)
        {
            //先让calculator计算一下
            if (mModifierCalculator.tryRemove(this, removeModifier))
            {
                mIsDirty = true;
                return true;
            }

            return false;
        }

        void ITezBonusModifierContainer.removeModifierByModifierClosed(ITezBonusModifier removeModifier)
        {
            mIsDirty = true;
            mModifierCalculator.removeModifierByModifierClosed(removeModifier);
        }

        int ITezBonusModifierContainer.calculate(ref int baseValue)
        {
            mIsDirty = false;
            return mModifierCalculator.calculate(ref baseValue);
        }

        float ITezBonusModifierContainer.calculate(ref float baseValue)
        {
            mIsDirty = false;
            return mModifierCalculator.calculate(ref baseValue);
        }

        void ITezBonusModifierContainer.clear()
        {
            mIsDirty = true;
            mModifierCalculator.clear(this);
        }

        void ITezCloseable.close()
        {
            mModifierCalculator.clear(this);
            mModifierCalculator.close();
            mModifierCalculator = null;
        }
    }
}
