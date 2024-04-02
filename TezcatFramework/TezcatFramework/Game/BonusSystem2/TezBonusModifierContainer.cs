using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusModifierContainer : ITezCloseable
    {
        void add(ITezBonusModifier modifier);
        bool remove(ITezBonusModifier modifier);

        int calculate(ref int baseValue);
        float calculate(ref float baseValue);
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

        public void add(ITezBonusModifier modifier)
        {
            mList.Add(modifier);
        }

        public bool remove(ITezBonusModifier modifier)
        {
            return mList.Remove(modifier);
        }

        public int calculate(ref int baseValue)
        {
            float sum_add = 0;
            float percent_add = 0;
            float percent_multi = 0;

            for (int i = mList.Count - 1; i >= 0; i--)
            {
                var modifier = mList[i];

                switch ((TezBonusModifierType)modifier.modifyType)
                {
                    case TezBonusModifierType.Base_SumAdd:
                        sum_add += modifier.value;
                        break;
                    case TezBonusModifierType.Base_PercentAdd:
                        percent_add += modifier.value;
                        break;
                    case TezBonusModifierType.Base_PercentMulti:
                        percent_multi += modifier.value;
                        break;
                    default:
                        break;
                }
            }

            int final = (int)(baseValue + sum_add
                + baseValue * (percent_add * (1.0f + percent_multi)));

            return final;
        }

        public float calculate(ref float baseValue)
        {
            float sum_add = 0;
            float percent_add = 0;
            float percent_multi = 0;

            for (int i = mList.Count - 1; i >= 0; i--)
            {
                var modifier = mList[i];

                switch ((TezBonusModifierType)modifier.modifyType)
                {
                    case TezBonusModifierType.Base_SumAdd:
                        sum_add += modifier.value;
                        break;
                    case TezBonusModifierType.Base_PercentAdd:
                        percent_add += modifier.value;
                        break;
                    case TezBonusModifierType.Base_PercentMulti:
                        percent_multi += modifier.value;
                        break;
                    default:
                        break;
                }
            }

            float final = baseValue + sum_add
                + baseValue * (percent_add * (1.0f + percent_multi));

            return final;
        }

        void ITezCloseable.deleteThis()
        {
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
        float mBase_SumAdd = 0;
        float mBase_PercentAdd = 0;
        float mBase_PercentMulti = 0;

        public void add(ITezBonusModifier modifier)
        {
            switch ((TezBonusModifierType)modifier.modifyType)
            {
                case TezBonusModifierType.Base_SumAdd:
                    mBase_SumAdd += modifier.value;
                    break;
                case TezBonusModifierType.Base_PercentAdd:
                    mBase_PercentAdd += modifier.value;
                    break;
                case TezBonusModifierType.Base_PercentMulti:
                    mBase_PercentMulti += modifier.value;
                    break;
                default:
                    break;
            }
        }

        public bool remove(ITezBonusModifier modifier)
        {
            switch ((TezBonusModifierType)modifier.modifyType)
            {
                case TezBonusModifierType.Base_SumAdd:
                    mBase_SumAdd -= modifier.value;
                    break;
                case TezBonusModifierType.Base_PercentAdd:
                    mBase_PercentAdd -= modifier.value;
                    break;
                case TezBonusModifierType.Base_PercentMulti:
                    mBase_PercentMulti -= modifier.value;
                    break;
                default:
                    return false;
            }

            return true;
        }

        public int calculate(ref int baseValue)
        {
            int final = (int)(baseValue + mBase_SumAdd + baseValue * (mBase_PercentAdd * (1.0f + mBase_PercentMulti)));

            return final;
        }

        public float calculate(ref float baseValue)
        {
            float final = baseValue + mBase_SumAdd + baseValue * (mBase_PercentAdd * (1.0f + mBase_PercentMulti));

            return final;
        }

        void ITezCloseable.deleteThis()
        {

        }
    }
}

