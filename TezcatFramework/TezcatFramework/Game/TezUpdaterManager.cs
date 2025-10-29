using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using static tezcat.Framework.Game.TezUpdaterManager;

namespace tezcat.Framework.Game
{
    public enum TezUpdateLoopType
    {
        Once,
        Loop,
        Count
    }

    public interface ITezUpdaterHolder : ITezCloseable
    {
        bool isValid();
        void setComplete();

        bool addDelayInitUpdater(Action<float> function);
        bool addMainLoopUpdater(Action<float> function);
        bool addCountLoopUpdater(Action<float> function, int count);
        bool addOnceLoopUpdater(Action<float> function);
    }

    public interface ITezUpdater
    {
        bool isComplete { get; set; }
        void setRegister(TezUpdaterRegister register);
    }

    public class TezUpdaterRegister
    {
        public static readonly TezUpdaterRegister defaultRegister = new TezUpdaterRegister();

        ITezUpdater mUpdater = null;
        TezUpdaterManager mManager = TezcatFramework.updaterManager;

        public bool isValid()
        {
            return mUpdater != null;
        }

        internal void clear()
        {
            mUpdater = null;
        }

        public void setComplete()
        {
            if (mUpdater != null)
            {
                mUpdater.isComplete = true;
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void close()
        {
            ///如果持有Updater，则将其标记为完成，并且将注册器重置为默认注册器
            ///然后删除当前的引用
            if (mUpdater != null)
            {
                mUpdater.isComplete = true;
                mUpdater.setRegister(TezUpdaterRegister.defaultRegister);
                mUpdater = null;
            }

            mManager = null;
        }

        public bool addDelayInitUpdater(Action<float> function)
        {
            if (mUpdater == null)
            {
                mUpdater = mManager.addDelayInitUpdater(function);
                mUpdater.setRegister(this);
                return true;
            }

            return false;
        }

        public bool addMainLoopUpdater(Action<float> function)
        {
            if (mUpdater == null)
            {
                mUpdater = mManager.addMainLoopUpdater(function);
                mUpdater.setRegister(this);
                return true;
            }

            return false;
        }

        public bool addCountLoopUpdater(Action<float> function, int count)
        {
            if (mUpdater == null)
            {
                mUpdater = mManager.addCountLoopUpdater(function, count);
                mUpdater.setRegister(this);
                return true;
            }

            return false;
        }

        public bool addOnceLoopUpdater(Action<float> function)
        {
            if (mUpdater == null)
            {
                mUpdater = mManager.addOnceLoopUpdater(function);
                mUpdater.setRegister(this);
                return true;
            }

            return false;
        }
    }

    public class TezUpdaterManager
    {
        class Updater : ITezUpdater
        {
            TezUpdaterRegister mRegister = null;

            Action<float> mUpdate = null;
            int mLoopCount = 0;
            public int loopCount => mLoopCount;
            public bool isComplete { get; set; } = false;

            public Updater()
            {
                mRegister = TezUpdaterRegister.defaultRegister;
            }

            void ITezUpdater.setRegister(TezUpdaterRegister holder)
            {
                mRegister = holder;
            }

            public void setFunction(Action<float> update, int loopCount = -1)
            {
                mLoopCount = loopCount;
                mUpdate = update;
                this.isComplete = false;
            }

            public void update(float dt)
            {
                mUpdate(dt);
            }

            public void close()
            {
                mRegister.clear();
                mUpdate = null;
            }

            public void reduceCount()
            {
                if (mLoopCount > 0)
                {
                    mLoopCount--;
                }
                else
                {
                    this.isComplete = true;
                }
            }

            internal void setComplete()
            {
                this.isComplete = true;
            }
        }

        List<Updater> mDelayInitList = new List<Updater>();
        List<Updater> mMainLoopList = new List<Updater>();
        List<Updater> mOnceLoopList = new List<Updater>();
        List<Updater> mCountLoopList = new List<Updater>();
        Queue<Updater> mPool = new Queue<Updater>();

        public event Action<int> evtDelayCount;
        public event Action<int> evtOnceCount;
        public event Action<int> evtCountCount;
        public event Action<int> evtMainCount;
        public event Action<int> evtFreeUpdateCount;
        public int freeUpdaterCount => mPool.Count;

        private Updater create(Action<float> function, int count)
        {
            if (mPool.Count > 0)
            {
                var temp = mPool.Dequeue();
                temp.setFunction(function, count);
                this.evtFreeUpdateCount?.Invoke(mPool.Count);
                return temp;
            }
            else
            {
                var temp = new Updater();
                temp.setFunction(function, count);
                return temp;
            }
        }

        private void recycle(Updater updater)
        {
            updater.close();
            mPool.Enqueue(updater);
            this.evtFreeUpdateCount?.Invoke(mPool.Count);
        }

        public ITezUpdater addDelayInitUpdater(Action<float> function)
        {
            var updater = this.create(function, -1);
            mDelayInitList.Add(updater);
            return updater;
        }

        public ITezUpdater addMainLoopUpdater(Action<float> function)
        {
            var updater = this.create(function, -1);
            mMainLoopList.Add(updater);
            return updater;
        }

        public ITezUpdater addCountLoopUpdater(Action<float> function, int count)
        {
            var updater = this.create(function, count);
            mCountLoopList.Add(updater);
            return updater;
        }

        public ITezUpdater addOnceLoopUpdater(Action<float> function)
        {
            var updater = this.create(function, -1);
            mOnceLoopList.Add(updater);
            return updater;
        }

        private void updateDelayInit(float dt)
        {
            this.evtDelayCount?.Invoke(mDelayInitList.Count);
            foreach (var item in mDelayInitList)
            {
                item.update(dt);
                item.close();
                this.recycle(item);
            }
            mDelayInitList.Clear();
        }

        private void updateMainLoop(float dt)
        {
            this.evtMainCount?.Invoke(mMainLoopList.Count);
            for (int i = mMainLoopList.Count - 1; i >= 0; i--)
            {
                var item = mMainLoopList[i];
                if (item.isComplete)
                {
                    mMainLoopList.RemoveAt(i);
                    item.close();
                    this.recycle(item);
                    continue;
                }
                else
                {
                    item.update(dt);
                }
            }
        }

        private void updateOnceLoop(float dt)
        {
            this.evtOnceCount?.Invoke(mOnceLoopList.Count);
            foreach (var item in mOnceLoopList)
            {
                item.update(dt);
                item.close();
                this.recycle(item);
            }
            mOnceLoopList.Clear();
        }

        private void updateCountLoop(float dt)
        {
            this.evtCountCount?.Invoke(mCountLoopList.Count);
            for (int i = mCountLoopList.Count - 1; i >= 0; i--)
            {
                var item = mCountLoopList[i];
                item.reduceCount();
                if (item.isComplete)
                {
                    mCountLoopList.RemoveAt(i);
                    item.close();
                    this.recycle(item);
                    continue;
                }
                else
                {
                    item.update(dt);
                }
            }
        }

        public void update(float dt)
        {
            this.updateDelayInit(dt);

            this.updateOnceLoop(dt);
            this.updateCountLoop(dt);
            this.updateMainLoop(dt);
        }
    }
}