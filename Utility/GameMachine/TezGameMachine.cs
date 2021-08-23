using System;
using System.Collections.Generic;
using tezcat.Framework.AI;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
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

        public event TezEventExtension.Action<TState> eventPush;
        public event TezEventExtension.Action<TState> eventPop;
        public event TezEventExtension.Action<TState, TState> eventChange;

        public IReadOnlyCollection<TState> stack => m_Stack;
        Stack<TState> m_Stack = new Stack<TState>();

        public void push<State>() where State : TState, new()
        {
            eventPush?.Invoke(Singleton<State>.instance);

            if (m_CurrentState != null)
            {
                m_CurrentState.pause(m_Blackboard);
                m_Stack.Push(m_CurrentState);
            }

            m_CurrentState = Singleton<State>.instance;
            m_CurrentState.gameMachine = this;
            m_CurrentState.enter(m_Blackboard);
        }

        public void pop<State>() where State : TState, new()
        {
            if (m_CurrentState != Singleton<State>.instance)
            {
                throw new Exception(string.Format("TezGameMachine : Pop [Current:{0}] [YourWant:{1}]", m_CurrentState.GetType().Name, typeof(State).Name));
            }

            if (m_Stack.Count == 0)
            {
                throw new Exception("TezGameMachine : Stack Count 0");
            }

            eventPop?.Invoke(m_CurrentState);
            m_CurrentState.exit(m_Blackboard);
            m_CurrentState = m_Stack.Pop();
            m_CurrentState.resume(m_Blackboard);
        }

        public void change<State>() where State : TState, new()
        {
            eventChange?.Invoke(m_CurrentState, Singleton<State>.instance);
            this.change(Singleton<State>.instance);
        }

        public bool isThis<State>() where State : TState, new()
        {
            return m_CurrentState == Singleton<State>.instance;
        }

        public Stack<TState> dump()
        {
            Stack<TState> dump = new Stack<TState>();
            if (m_CurrentState != null)
            {
                dump.Push(m_CurrentState);
            }

            foreach (var item in m_Stack)
            {
                dump.Push(item);
            }

            return dump;
        }

        public override void close()
        {
            base.close();

            foreach (var item in m_Stack)
            {
                item.close();
            }

            m_Stack.Clear();
            m_Stack = null;

            eventPop = null;
            eventPush = null;
            eventChange = null;
        }
    }
}