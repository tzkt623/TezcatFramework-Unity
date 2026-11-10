
namespace tezcat.Framework.Game
{
    /// <summary>
    /// 
    /// Entry是为了解决Modifier和Container之间的生命周期问题
    /// 当Entry销毁自己时,Container需要在下一个更新周期中移除Modifier
    /// 此时Modififer可能已经被销毁了,所以Container不能直接访问Modifier
    /// 只能通过Entry来间接验证
    /// 
    /// Entry只能被Container销毁
    /// 因为Modifier销毁后,Container需要在下一个更新周期中移除Modifier
    /// Container销毁后,Entry需要通知Modifier立即移除自己,否则会造成内存泄漏
    /// 所以Entry的生命周期由Container来主动管理
    /// 
    /// Modifer只负责生成Entry
    /// 
    /// </summary>
    public class TezBonusModifierEntry
    {
        ITezBonusModifier mSourceModifier = null;
        ITezBonusModifierContainer mTargetValue = null;

        public bool isNeedRemove => mSourceModifier == null;
        public ITezBonusModifier sourceModifier => mSourceModifier;
        public ITezBonusModifierContainer targetValue => mTargetValue;
        private bool mIsRemoved = false;

        public void initCalculator(ITezBonusModifierContainer calculator)
        {
            if (mTargetValue != null)
            {
                throw new InvalidOperationException();
            }

            mTargetValue = calculator;
        }

        public void initModifier(ITezBonusModifier sourceModifier)
        {
            if (mSourceModifier != null)
            {
                throw new InvalidOperationException();
            }

            mSourceModifier = sourceModifier;
        }

        /// <summary>
        /// 如果是Modifier关闭了导致Entry关闭
        /// 只需要清理数据就行了
        /// 不需要通知SourceModifier删除自己
        /// 
        /// 这时Entry还留在TargetValue的列表中
        /// 这时不能回收自己
        /// 需要在TargetValue中的必要时刻回收自己
        /// </summary>
        public void closeByModifierClosed()
        {
            if(mIsRemoved)
            {
                return;
            }

            if(!mTargetValue.tryRemoveEntry(this))
            {
                throw new Exception($"ModifierEntry {this} remove from ValueContainer Failed");
            }

            mTargetValue.removeValue(this);

            mSourceModifier = null;
            mTargetValue = null;
            mIsRemoved = true;
        }


        /// <summary>
        /// 如果是自己主动关闭
        /// 通知SourceModifier删除自己
        /// 通知TargetValue删除自己
        /// 
        /// 然后立刻回收自己
        /// </summary>
        public void closeBySelfClosed()
        {
            if (mIsRemoved)
            {
                return;
            }

            if (!mSourceModifier.tryRemoveEntry(this))
            {
                throw new Exception($"ModifierEntry [{this}] Remove Self From SourceModifier Failed");
            }

            if (!mTargetValue.tryRemoveEntry(this))
            {
                throw new Exception($"ModifierEntry [{this}] Remove Self From TargetValue Failed");
            }

            mTargetValue.removeValue(this);

            mTargetValue = null;
            mSourceModifier = null;
            mIsRemoved = true;

            recycle(this);
        }

        /// <summary>
        /// 如果是被加成的值关闭了
        /// 那么不需要清理值的数据
        /// 只需要通知SourceModifier清理
        /// 然后立刻回收自己
        /// </summary>
        public void closeByTargetValueClosed()
        {
            if (mIsRemoved)
            {
                return;
            }

            if (!mSourceModifier.tryRemoveEntry(this))
            {
                throw new Exception($"ModifierEntry [{this}] Remove Self From SourceModifier Failed");
            }

            mSourceModifier = null;
            mTargetValue = null;
            mIsRemoved = true;

            recycle(this);
        }

        private static Stack<TezBonusModifierEntry> sCache = new Stack<TezBonusModifierEntry>();

        public static TezBonusModifierEntry peek()
        {
            if (sCache.Count == 0)
            {
                sCache.Push(new TezBonusModifierEntry());
            }

            return sCache.Peek();
        }

        public static TezBonusModifierEntry pop()
        {
            var temp = sCache.Pop();
            temp.mIsRemoved = false;
            return temp;
        }

        public static void recycle(TezBonusModifierEntry entry)
        {
            sCache.Push(entry);
        }
    }
}