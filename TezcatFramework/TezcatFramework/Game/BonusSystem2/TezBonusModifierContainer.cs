using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public abstract class TezBonusBaseModifierContainer : ITezCloseable
    {
        protected List<ITezBonusModifier> mList = new List<ITezBonusModifier>(8);

        protected TezBonusToken mToken = null;
        public TezBonusToken bonusToken => mToken;

        protected bool mDirty = true;
        public bool isDirty => mDirty;

        public void init(TezBonusToken token)
        {
            mToken = token;
        }

        /// <summary>
        /// 标记为脏
        /// </summary>
        public void markDirty()
        {
            mDirty = true;
        }

        public void add(ITezBonusModifier modifier)
        {
            mDirty = true;
            mList.Add(modifier);
            this.onAdd(modifier);
        }

        protected abstract void onAdd(ITezBonusModifier modifier);

        public void remove(ITezBonusModifier modifier)
        {
            mDirty = true;
            mList.Remove(modifier);
            this.onRemove(modifier);
        }

        protected abstract void onRemove(ITezBonusModifier modifier);

        public void removeFrom(object source)
        {
            for (int i = mList.Count - 1; i >= 0; i--)
            {
                if (mList[i].owner == source)
                {
                    this.onRemove(mList[i]);
                    mList.RemoveAt(i);
                    mDirty = true;
                }
            }
        }

        public void close()
        {
            mList.Clear();
            mList = null;
            mToken = null;
        }

        public int calculate(ref int baseValue)
        {
            mDirty = false;
            return this.onCalculate(ref baseValue);
        }

        public float calculate(ref float baseValue)
        {
            mDirty = false;
            return this.onCalculate(ref baseValue);
        }

        protected abstract int onCalculate(ref int baseValue);
        protected abstract float onCalculate(ref float baseValue);
    }

    public class TezBonusModifierContainer : TezBonusBaseModifierContainer
    {
//         float mBase_SumAdd = 0;
//         float mBase_PercentAdd = 0;
//         float mBase_PercentMulti = 0;

        protected override void onAdd(ITezBonusModifier modifier)
        {
//             switch ((TezBonusModifierType)modifier.modifyType)
//             {
//                 case TezBonusModifierType.Base_SumAdd:
//                     mBase_SumAdd += modifier.value;
//                     break;
//                 case TezBonusModifierType.Base_PercentAdd:
//                     mBase_PercentAdd += modifier.value;
//                     break;
//                 case TezBonusModifierType.Base_PercentMulti:
//                     mBase_PercentMulti += modifier.value;
//                     break;
//                 default:
//                     break;
//             }
        }

        protected override void onRemove(ITezBonusModifier modifier)
        {
//             switch ((TezBonusModifierType)modifier.modifyType)
//             {
//                 case TezBonusModifierType.Base_SumAdd:
//                     mBase_SumAdd -= modifier.value;
//                     break;
//                 case TezBonusModifierType.Base_PercentAdd:
//                     mBase_PercentAdd -= modifier.value;
//                     break;
//                 case TezBonusModifierType.Base_PercentMulti:
//                     mBase_PercentMulti -= modifier.value;
//                     break;
//                 default:
//                     break;
//             }
        }

        protected override float onCalculate(ref float baseValue)
        {
            float sum_add = 0;
            float percent_add = 0;
            float percent_multi = 0;

            foreach (var modifier in mList)
            {
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

        protected override int onCalculate(ref int baseValue)
        {
            float sum_add = 0;
            float percent_add = 0;
            float percent_multi = 0;

            foreach (var modifier in mList)
            {
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
    }
}

