using System;

namespace tezcat.Framework.Game
{
    public interface ITezBonusSystemHolder
    {
        TezBonusSystem bonusSystem { get; }
    }

    public class TezBonusSystem
    {
        TezBonusBaseModifierContainer[] mContainerArray = null;

        public void init(int valueCapacity)
        {
            mContainerArray = new TezBonusBaseModifierContainer[valueCapacity];
        }

        public void register<ModifierContainer>(ITezBonusableValue bonusableValue, TezBonusToken bonusToken)
            where ModifierContainer : TezBonusModifierContainer, new()
        {
            var container = new ModifierContainer();
            container.init(bonusToken);

            bonusableValue.setContainer(container);
            if (mContainerArray[bonusToken.indexID] != null)
            {
                throw new Exception();
            }
            mContainerArray[bonusToken.indexID] = bonusableValue.modifierContainer;
        }

        public void addModifier(TezBonusModifier modifier)
        {
            var iid = modifier.bonusToken.indexID;
            mContainerArray[iid].add(modifier);
        }

        public void removeModifier(TezBonusModifier modifier)
        {
            var iid = modifier.bonusToken.indexID;
            mContainerArray[iid].remove(modifier);
        }

        public void close()
        {
            foreach (var item in mContainerArray)
            {
                item?.close();
            }

            mContainerArray = null;
        }
    }
}

