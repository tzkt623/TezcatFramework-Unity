using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 游戏状态机
    /// 整个游戏的状态机
    /// </summary>
    public class TezGameMachine<TBlackborad, TState>
        : TezBaseFSM<TBlackborad, TState>
        , ITezGameMachine<TBlackborad, TState>
        where TBlackborad : TezBaseFSMBlackboard
        where TState : TezGameMachineState<TBlackborad, TState>
    {
        class Singleton<State> where State : TState, new()
        {
            public static State instance = new State();
        }

        public event TezEventExtension.Action<TState> evtPush;
        public event TezEventExtension.Action<TState> evtPop;
        public event TezEventExtension.Action<TState, TState> evtChange;
        public event TezEventExtension.Action evtRefresh;

        public IReadOnlyCollection<TState> stack => mStack;
        Stack<TState> mStack = new Stack<TState>();

        public State push<State>() where State : TState, new()
        {
            evtPush?.Invoke(Singleton<State>.instance);

            if (mCurrentState != null)
            {
                mCurrentState.pause(mBlackboard);
                mStack.Push(mCurrentState);
            }

            mCurrentState = Singleton<State>.instance;
            mCurrentState.gameMachine = this;
            mCurrentState.enter(mBlackboard);

            evtRefresh?.Invoke();
            return (State)mCurrentState;
        }

        /// <summary>
        /// 用于在
        /// </summary>
        /// <typeparam name="State"></typeparam>
        public void pop<State>() where State : TState, new()
        {
            if (mStack.Count == 0)
            {
                throw new Exception("TezGameMachine : Stack Count Must > 0");
            }

            //如果与当前状态不同,说明是标记弹出
            if (mCurrentState != Singleton<State>.instance)
            {
                //                throw new Exception(string.Format("TezGameMachine : Pop [Current:{0}] [YourWant:{1}]", mCurrentState.GetType().Name, typeof(State).Name));

                //先弹出了再说
                Singleton<State>.instance.markPop();
                //只退出一次
                Singleton<State>.instance.exit(mBlackboard);
            }
            else
            {
                evtPop?.Invoke(mCurrentState);
                mCurrentState.exit(mBlackboard);
                mCurrentState = mStack.Pop();

                //延迟处理所有标记弹出
                while (mCurrentState.needMarkPop())
                {
                    evtPop?.Invoke(mCurrentState);
                    mCurrentState = mStack.Pop();
                }

                mCurrentState.resume(mBlackboard);
            }

            evtRefresh?.Invoke();
        }

        public void change<State>() where State : TState, new()
        {
            evtChange?.Invoke(mCurrentState, Singleton<State>.instance);
            this.change(Singleton<State>.instance);
        }

        public bool isThis<State>() where State : TState, new()
        {
            return mCurrentState == Singleton<State>.instance;
        }

        public Stack<TState> dump()
        {
            Stack<TState> dump = new Stack<TState>();
            if (mCurrentState != null)
            {
                dump.Push(mCurrentState);
            }

            foreach (var item in mStack)
            {
                dump.Push(item);
            }

            return dump;
        }

        protected override void onClose()
        {
            base.onClose();

            foreach (var item in mStack)
            {
                item.close();
            }

            mStack.Clear();
            mStack = null;

            evtPop = null;
            evtPush = null;
            evtChange = null;
        }
    }
}