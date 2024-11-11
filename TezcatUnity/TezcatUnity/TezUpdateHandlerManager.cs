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
        void updateOnMainLoop(float dt);
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

            public void update()
            {
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

            public void update()
            {
                var dt = Time.deltaTime;
                ITezUpdateHandler handler;
                while (mQueue.Count > 0)
                {
                    handler = mQueue.Dequeue();
                    handler.updateOnMainLoop(dt);
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
                        current.Value.updateOnMainLoop(dt);
                        current = current.Next;
                    }
                }
            }

            public void register(ITezUpdateHandler handler, TezUpdateHandlerType type)
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

        public class HandlerPackCoroutine : HandlerPack
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

        static OncePack mDelayInitPack = new OncePack();
        static HandlerPack mMainLoopPack = new HandlerPack();

        static Dictionary<string, HandlerPackCoroutine> mHandlerPackCoroutineDict = new Dictionary<string, HandlerPackCoroutine>();

        static HandlerPackCoroutine createPack(string name, YieldInstruction yieldInstruction)
        {
            if (mHandlerPackCoroutineDict.TryGetValue(name, out HandlerPackCoroutine handlerPack))
            {
                return handlerPack;
            }

            HandlerPackCoroutine pack = new HandlerPackCoroutine();
            pack.setYieldInstruction(yieldInstruction);
            mHandlerPackCoroutineDict.Add(name, pack);

            return pack;
        }

        public static void addDelayInitHandler(this ITezUpdateHandler updateHandler)
        {
            mDelayInitPack.add(updateHandler);
        }

        public static void addMainLoopUpdateHandler(this ITezUpdateHandler updateHandler, TezUpdateHandlerType type)
        {
            mMainLoopPack.register(updateHandler, type);
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