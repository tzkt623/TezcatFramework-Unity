using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusModifierContainer : ITezCloseable
    {
        bool isDirty { get; }
        void setDirty();

        void add(ITezBonusModifier modifier);
        bool remove(ITezBonusModifier modifier);

        int calculate(ref int baseValue);
        float calculate(ref float baseValue);

        void clear();
    }

    /// <summary>
    /// 
    /// 用List存储Modifier
    /// 
    /// <para>
    /// 优点
    /// 存有所有Modifier,可以查询
    /// </para>
    /// 
    /// <para>
    /// 缺点
    /// 计算值需要遍历整个List
    /// 在删除Modifier时会花费大量时间
    /// </para>
    /// 
    /// </summary>
    public class TezBonusModifierList : ITezBonusModifierContainer
    {
        List<ITezBonusModifier> mList = new List<ITezBonusModifier>();
        bool mIsDirty = false;
        bool ITezBonusModifierContainer.isDirty => mIsDirty;

        void ITezBonusModifierContainer.setDirty()
        {
            mIsDirty = true;
        }

        void ITezBonusModifierContainer.add(ITezBonusModifier modifier)
        {
            if (modifier.addMaster(this))
            {
                mIsDirty = true;
                mList.Add(modifier);
            }
        }

        bool ITezBonusModifierContainer.remove(ITezBonusModifier modifier)
        {
            if(modifier.removeMaster(this))
            {
                mIsDirty = true;
                modifier.isNeedRemove = true;
                //return mList.Remove(modifier);
                return true;
            }

            return false;
        }

        int ITezBonusModifierContainer.calculate(ref int baseValue)
        {
            float sum_add = 0;
            float percent_add = 0;
            float percent_multi = 0;

            for (int i = mList.Count - 1; i >= 0; i--)
            {
                var modifier = mList[i];
                if(modifier.isNeedRemove)
                {
                    mList.RemoveAt(i);
                }
                else
                {
                    switch ((TezBonusModifierCalculateRule)modifier.calculateRule)
                    {
                        case TezBonusModifierCalculateRule.Base_SumAdd:
                            sum_add += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Base_PercentAdd:
                            percent_add += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Base_PercentMulti:
                            percent_multi += modifier.value;
                            break;
                        default:
                            break;
                    }
                }
            }

            int final = (int)(baseValue + sum_add
                + baseValue * (percent_add * (1.0f + percent_multi)));

            mIsDirty = false;

            return final;
        }

        float ITezBonusModifierContainer.calculate(ref float baseValue)
        {
            float sum_add = 0;
            float percent_add = 0;
            float percent_multi = 0;

            for (int i = mList.Count - 1; i >= 0; i--)
            {
                var modifier = mList[i];
                if (modifier.isNeedRemove)
                {
                    mList.RemoveAt(i);
                }
                else
                {
                    switch ((TezBonusModifierCalculateRule)modifier.calculateRule)
                    {
                        case TezBonusModifierCalculateRule.Base_SumAdd:
                            sum_add += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Base_PercentAdd:
                            percent_add += modifier.value;
                            break;
                        case TezBonusModifierCalculateRule.Base_PercentMulti:
                            percent_multi += modifier.value;
                            break;
                        default:
                            break;
                    }
                }
            }

            float final = baseValue + sum_add
                + baseValue * (percent_add * (1.0f + percent_multi));
            mIsDirty = false;

            return final;
        }

        void ITezBonusModifierContainer.clear()
        {
            foreach (var item in mList)
            {
                item.removeMaster(this);
            }

            mList.Clear();
            mIsDirty = true;
        }

        void ITezCloseable.close()
        {
            foreach (var item in mList)
            {
                item.removeMaster(this);
            }

            mList.Clear();
            mList = null;
        }
    }

    /// <summary>
    /// 用Cache存储Modifier
    /// 
    /// <para>
    /// 优点
    /// 
    /// 计算值时速度很快
    /// </para>
    /// 
    /// <para>
    /// 缺点
    /// 
    /// 无法知道当前Modifier的具体信息
    /// </para>
    /// 
    /// </summary>
    public class TezBonusModifierCache : ITezBonusModifierContainer
    {
        bool mIsDirty = false;
        bool ITezBonusModifierContainer.isDirty => mIsDirty;

        float mBase_SumAdd = 0;
        float mBase_PercentAdd = 0;
        float mBase_PercentMulti = 0;

        void ITezBonusModifierContainer.setDirty()
        {
            mIsDirty = true;
        }

        void ITezBonusModifierContainer.add(ITezBonusModifier modifier)
        {
            if(!modifier.addMaster(this))
            {
                return;
            }

            switch ((TezBonusModifierCalculateRule)modifier.calculateRule)
            {
                case TezBonusModifierCalculateRule.Base_SumAdd:
                    mBase_SumAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentAdd:
                    mBase_PercentAdd += modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentMulti:
                    mBase_PercentMulti += modifier.value;
                    break;
                default:
                    break;
            }

            mIsDirty = true;
        }

        bool ITezBonusModifierContainer.remove(ITezBonusModifier modifier)
        {
            if (!modifier.removeMaster(this))
            {
                return false;
            }

            switch ((TezBonusModifierCalculateRule)modifier.calculateRule)
            {
                case TezBonusModifierCalculateRule.Base_SumAdd:
                    mBase_SumAdd -= modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentAdd:
                    mBase_PercentAdd -= modifier.value;
                    break;
                case TezBonusModifierCalculateRule.Base_PercentMulti:
                    mBase_PercentMulti -= modifier.value;
                    break;
            }

            mIsDirty = true;

            return true;
        }

        int ITezBonusModifierContainer.calculate(ref int baseValue)
        {
            int final = (int)(baseValue + mBase_SumAdd + baseValue * (mBase_PercentAdd * (1.0f + mBase_PercentMulti)));

            mIsDirty = false;
            return final;
        }

        float ITezBonusModifierContainer.calculate(ref float baseValue)
        {
            float final = baseValue + mBase_SumAdd + baseValue * (mBase_PercentAdd * (1.0f + mBase_PercentMulti));

            mIsDirty = false;
            return final;
        }

        void ITezCloseable.close()
        {

        }

        void ITezBonusModifierContainer.clear()
        {

        }
    }
}

