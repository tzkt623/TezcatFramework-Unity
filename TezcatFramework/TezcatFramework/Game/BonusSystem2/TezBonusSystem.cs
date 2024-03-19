using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public class TezBonusableValueArray : TezValueWrapperArray<ITezBonusableValue>
    {

    }

    public interface ITezBonusSystemHolder
    {
        TezBonusSystem bonusSystem { get; }
    }

    public class TezBonusSystem : ITezCloseable
    {
        ITezBonusableValue[] mArray = null;

        public void init(int bonusTokenTypeID)
        {
            var capacity =TezBonusToken.getTokenCapacity(bonusTokenTypeID);
            mArray = new ITezBonusableValue[capacity];
        }

        public void set(ITezBonusableValue bonusableValue)
        {
            var token = bonusableValue.bonusToken;
            if (mArray[token.indexID] != null)
            {
                throw new Exception();
            }
            mArray[token.indexID] = bonusableValue;
        }

        public void addModifier(ITezBonusModifier modifier)
        {
            var iid = modifier.bonusToken.indexID;
            mArray[iid].addModifier(modifier);
        }

        public void removeModifier(ITezBonusModifier modifier)
        {
            var iid = modifier.bonusToken.indexID;
            mArray[iid].removeModifier(modifier);
        }

        public void close()
        {
            mArray = null;
        }
    }
}

