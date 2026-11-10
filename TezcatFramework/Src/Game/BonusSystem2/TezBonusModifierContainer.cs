using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Modifier间接容器
    /// 用于统计Modifier数据
    /// </summary>
    public interface ITezBonusModifierContainer : ITezCloseable
    {
        bool isDirty { get; }
        void setDirty();

        void addModifier(ITezBonusModifier modifier);
        //bool removeModifier(ITezBonusModifier modifier);

        int calculate(ref int baseValue);
        float calculate(ref float baseValue);

        void clear();
        bool tryRemoveEntry(TezBonusModifierEntry entry);
        void removeValue(TezBonusModifierEntry entry);
        void fastUpdate(int oldValue, TezBonusModifierEntry entry);
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
            mModifierCalculator = new TezBonusModifierCalculator_Value();
        }

        void ITezBonusModifierContainer.setDirty()
        {
            mIsDirty = true;
        }

        void ITezBonusModifierContainer.addModifier(ITezBonusModifier addModifier)
        {
            if(!addModifier.tryAddContainer(this, out var entry))
            {
                throw new Exception($"Modifier {addModifier} repeat add to this Value Container");
            }

            if(!mModifierCalculator.tryAddEntry(entry))
            {
                throw new Exception($"ModifierEntry {entry} repeat add to this Value Calculator");
            }

            mIsDirty = true;
            entry.initCalculator(this);
            mModifierCalculator.addValue(entry);
        }

//         bool ITezBonusModifierContainer.removeModifier(ITezBonusModifier removeModifier)
//         {
//             if(!removeModifier.tryRemoveContainer(this, out var entry))
//             {
//                 throw new Exception($"Modifier {removeModifier} Remove From ValueContainer Failed");
//             }
// 
//             if(!mModifierCalculator.tryRemoveEntry(entry))
//             {
//                 throw new Exception($"ModifierEntry {entry} remove from ValueCalculator Failed");
//             }
// 
//             mIsDirty = true;
//             mModifierCalculator.removeValue(entry);
// 
//             return true;
//         }

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

        bool ITezBonusModifierContainer.tryRemoveEntry(TezBonusModifierEntry entry)
        {
            if(mModifierCalculator.tryRemoveEntry(entry))
            {
                mIsDirty = true;
                return true;
            }

            return false;
        }

        void ITezBonusModifierContainer.removeValue(TezBonusModifierEntry entry)
        {
            mModifierCalculator.removeValue(entry);
        }

        void ITezBonusModifierContainer.fastUpdate(int oldValue, TezBonusModifierEntry entry)
        {
            mModifierCalculator.fastUpdate(oldValue, entry);
        }
    }
}
