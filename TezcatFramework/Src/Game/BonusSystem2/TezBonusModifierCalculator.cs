using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusModifierCalculator : ITezCloseable
    {
        //         bool tryAdd(ITezBonusModifierContainer master, ITezBonusModifier modifier);
        //         bool tryRemove(ITezBonusModifierContainer master, ITezBonusModifier modifier);
        int calculate(ref int baseValue);
        float calculate(ref float baseValue);
        void clear(ITezBonusModifierContainer container);
        bool tryRemoveEntry(TezBonusModifierEntry entry);
        bool tryAddEntry(TezBonusModifierEntry entry);
        void addValue(TezBonusModifierEntry entry);
        void removeValue(TezBonusModifierEntry entry);
        void fastUpdate(int oldValue, TezBonusModifierEntry entry);
    }

    public class TezBonusModifierCalculator_Value : ITezBonusModifierCalculator
    {
        HashSet<TezBonusModifierEntry> mEntryList = new HashSet<TezBonusModifierEntry>();

        int mBase_SumAdd = 0;
        int mBase_PercentAdd = 0;
        int mTotal_SumAdd = 0;
        int mTotal_PercentAdd = 0;

        int ITezBonusModifierCalculator.calculate(ref int baseValue)
        {
            var base_temp = mBase_SumAdd * TezBonusModifierConfig.reciprocal + baseValue * (1.0f + mBase_PercentAdd * TezBonusModifierConfig.reciprocal);
            return (int)(mTotal_SumAdd * TezBonusModifierConfig.reciprocal + base_temp * (1.0f + mTotal_PercentAdd * TezBonusModifierConfig.reciprocal));
        }

        float ITezBonusModifierCalculator.calculate(ref float baseValue)
        {
            var base_temp = mBase_SumAdd * TezBonusModifierConfig.reciprocal + baseValue * (1.0f + mBase_PercentAdd * TezBonusModifierConfig.reciprocal);
            return mTotal_SumAdd * TezBonusModifierConfig.reciprocal + base_temp * (1.0f + mTotal_PercentAdd * TezBonusModifierConfig.reciprocal);
        }

        void ITezBonusModifierCalculator.clear(ITezBonusModifierContainer container)
        {
            foreach (var item in mEntryList)
            {
                item.closeByTargetValueClosed();
            }

            mEntryList.Clear();
            mBase_SumAdd = 0;
            mBase_PercentAdd = 0;
            mTotal_SumAdd = 0;
            mTotal_PercentAdd = 0;
        }

        void ITezCloseable.close()
        {
            mEntryList = null;
        }

        bool ITezBonusModifierCalculator.tryAddEntry(TezBonusModifierEntry entry)
        {
            return mEntryList.Add(entry);
        }

        bool ITezBonusModifierCalculator.tryRemoveEntry(TezBonusModifierEntry entry)
        {
            if (mEntryList.Remove(entry))
            {
                TezBonusModifierEntry.recycle(entry);
                return true;
            }

            return false;
        }

        void ITezBonusModifierCalculator.addValue(TezBonusModifierEntry entry)
        {
            var modifier = entry.sourceModifier;
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
        }

        void ITezBonusModifierCalculator.removeValue(TezBonusModifierEntry entry)
        {
            var modifier = entry.sourceModifier;
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
        }

        void ITezBonusModifierCalculator.fastUpdate(int oldValue, TezBonusModifierEntry entry)
        {
            var modifier = entry.sourceModifier;
            switch (modifier.calculateRule)
            {
                case TezBonusModifierCalculateRule.Base_SumAdd:
                    mBase_SumAdd -= oldValue;
                    mBase_SumAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentAdd:
                    mBase_PercentAdd -= oldValue;
                    mBase_PercentAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_SumAdd:
                    mTotal_SumAdd -= oldValue;
                    mTotal_SumAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Total_PercentAdd:
                    mTotal_PercentAdd -= oldValue;
                    mTotal_PercentAdd += modifier.value;
                    break;
                default:
                    break;
            }
        }
    }


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

        private void updateList(ref int baseSumAdd, ref int basePercentAdd, ref int totalSumAdd, ref int totalPercentAdd)
        {
            ITezBonusModifier modifier = null;
            foreach (var entry in mCurrent)
            {
                modifier = entry.sourceModifier;
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
            int base_sum_add = 0;
            int base_percent_add = 0;
            int total_sum_add = 0;
            int total_percent_add = 0;

            this.updateList(ref base_sum_add, ref base_percent_add, ref total_sum_add, ref total_percent_add);

            var base_temp = base_sum_add * TezBonusModifierConfig.reciprocal + baseValue * (1.0f + base_percent_add * TezBonusModifierConfig.reciprocal);
            return (int)(total_sum_add * TezBonusModifierConfig.reciprocal + base_temp * (1.0f + total_percent_add * TezBonusModifierConfig.reciprocal));
        }

        float ITezBonusModifierCalculator.calculate(ref float baseValue)
        {
            int base_sum_add = 0;
            int base_percent_add = 0;
            int total_sum_add = 0;
            int total_percent_add = 0;

            this.updateList(ref base_sum_add, ref base_percent_add, ref total_sum_add, ref total_percent_add);

            var base_temp = base_sum_add * TezBonusModifierConfig.reciprocal + baseValue * (1.0f + base_percent_add * TezBonusModifierConfig.reciprocal);
            return total_sum_add * TezBonusModifierConfig.reciprocal + base_temp * (1.0f + total_percent_add * TezBonusModifierConfig.reciprocal);
        }

        void ITezBonusModifierCalculator.clear(ITezBonusModifierContainer container)
        {
            foreach (var item in mCurrent)
            {
                item.closeByTargetValueClosed();
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

        bool ITezBonusModifierCalculator.tryRemoveEntry(TezBonusModifierEntry entry)
        {
            return true;
        }

        bool ITezBonusModifierCalculator.tryAddEntry(TezBonusModifierEntry entry)
        {
            mCurrent.Add(entry);
            return true;
        }

        void ITezBonusModifierCalculator.addValue(TezBonusModifierEntry entry)
        {

        }

        void ITezBonusModifierCalculator.removeValue(TezBonusModifierEntry entry)
        {

        }

        void ITezBonusModifierCalculator.fastUpdate(int oldValue, TezBonusModifierEntry entry)
        {

        }
    }
}
