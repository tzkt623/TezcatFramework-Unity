using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Unity
{
    public enum TezUpdateHandlerType
    {
        Once,
        Loop
    }

    /// <summary>
    /// 更新控制器
    /// 
    /// 用于对对象在各个时间点进行更新
    /// </summary>
    public interface ITezUpdateHandler
    {
        bool allowAdd { get; set; }
        bool isComplete { get; set; }
        /// <summary>
        /// 主循环更新
        /// </summary>
        void updateOnMainLoopOnce(float dt);
        void updateOnMainLoopLoop(float dt);
        /// <summary>
        /// 对象延迟初始化时更新
        /// </summary>
        void updateOnDelayInit();
    }

    public static class TezUpdateHandlerManager
    {
        public class OncePack
        {
            Queue<ITezUpdateHandler> mQueue = new Queue<ITezUpdateHandler>();

            public int queueCount => mQueue.Count;
            public event Action<int> evtCountor;

            public void update()
            {
                evtCountor?.Invoke(mQueue.Count);

                var dt = Time.deltaTime;
                ITezUpdateHandler handler;
                while (mQueue.Count > 0)
                {
                    handler = mQueue.Dequeue();
                    handler.updateOnDelayInit();
                }
            }

            public void add(ITezUpdateHandler handler)
            {
                mQueue.Enqueue(handler);
            }
        }

        public class HandlerPack
        {
            Queue<ITezUpdateHandler> mQueue = new Queue<ITezUpdateHandler>();
            LinkedList<ITezUpdateHandler> mLinkedList = new LinkedList<ITezUpdateHandler>();

            public int queueCount => mQueue.Count;
            public int listCount => mLinkedList.Count;

            public event Action<int, int> evtCountor;

            public void update()
            {
                evtCountor?.Invoke(mQueue.Count, mLinkedList.Count);

                var dt = Time.deltaTime;
                ITezUpdateHandler handler;
                while (mQueue.Count > 0)
                {
                    handler = mQueue.Dequeue();
                    handler.updateOnMainLoopOnce(dt);
                    handler.allowAdd = true;
                }

                var current = mLinkedList.First;
                while (current != null)
                {
                    if (current.Value.isComplete)
                    {
                        current.Value.allowAdd = true;

                        var next = current.Next;
                        mLinkedList.Remove(current);

                        current = next;
                    }
                    else
                    {
                        current.Value.updateOnMainLoopLoop(dt);
                        current = current.Next;
                    }
                }
            }

            public void add(ITezUpdateHandler handler, TezUpdateHandlerType type)
            {
                if (handler.allowAdd)
                {
                    handler.allowAdd = false;

                    switch (type)
                    {
                        case TezUpdateHandlerType.Once:
                            mQueue.Enqueue(handler);
                            break;
                        case TezUpdateHandlerType.Loop:
                            handler.isComplete = false;
                            mLinkedList.AddLast(handler);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public class CoroutineHandlerPack : HandlerPack
        {
            bool mRunning = false;
            YieldInstruction mYieldInstruction = null;

            private IEnumerator enumUpdate()
            {
                this.update();
                yield return mYieldInstruction;
            }

            public void setYieldInstruction(YieldInstruction yieldInstruction)
            {
                mYieldInstruction = yieldInstruction;
            }

            public void startByCoroutine()
            {
                if (!mRunning)
                {
                    mRunning = true;
                    monoBehaviour.StartCoroutine(this.enumUpdate());
                }
            }

            public void stopByCoroutine()
            {
                if (mRunning)
                {
                    mRunning = false;
                    monoBehaviour.StopCoroutine(this.enumUpdate());
                }
            }
        }

        public static MonoBehaviour monoBehaviour { get; set; }
        public static OncePack delayInit => mDelayInitPack;
        public static HandlerPack mainLoop => mMainLoopPack;
        public static IReadOnlyDictionary<string, CoroutineHandlerPack> coroutineHandlerDict => mCoroutineHandlerPackDict;

        public static event Action<CoroutineHandlerPack> evtCoroutineHandlerPackCreated;

        static OncePack mDelayInitPack = new OncePack();
        static HandlerPack mMainLoopPack = new HandlerPack();

        static Dictionary<string, CoroutineHandlerPack> mCoroutineHandlerPackDict = new Dictionary<string, CoroutineHandlerPack>();

        static CoroutineHandlerPack createPack(string name, YieldInstruction yieldInstruction)
        {
            if (mCoroutineHandlerPackDict.TryGetValue(name, out CoroutineHandlerPack handlerPack))
            {
                return handlerPack;
            }

            CoroutineHandlerPack pack = new CoroutineHandlerPack();
            pack.setYieldInstruction(yieldInstruction);
            mCoroutineHandlerPackDict.Add(name, pack);
            evtCoroutineHandlerPackCreated?.Invoke(pack);

            return pack;
        }

        public static void addDelayInitHandler(this ITezUpdateHandler updateHandler)
        {
            mDelayInitPack.add(updateHandler);
        }

        public static void addMainLoopUpdateHandler(this ITezUpdateHandler updateHandler, TezUpdateHandlerType type)
        {
            mMainLoopPack.add(updateHandler, type);
        }

        public static void removeMainLoopUpdateHandler(this ITezUpdateHandler updateHandler)
        {
            if (!updateHandler.allowAdd)
            {
                updateHandler.isComplete = true;
            }
        }

        public static void update()
        {
            //Debug.Log($"TezUpdateHandlerManager:Queue{mMainLoopPack.queueCount} List{mMainLoopPack.listCount}");
            //延迟初始化
            mDelayInitPack.update();
            //主循环
            mMainLoopPack.update();
        }
    }
}