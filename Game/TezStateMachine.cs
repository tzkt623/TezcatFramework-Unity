using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat
{
    public class TezState
    {
        public int state;
        public TezEventBus.Action<Camera, EventSystem> function;
    }

    public static class TezStateMachine
    {
        public static event TezEventBus.Action<int> onPop;
        public static event TezEventBus.Action<int> onPush;

        static Camera m_Camera = null;
        static EventSystem m_EventSystem = null;

        static Stack<TezState> m_StateLogic = new Stack<TezState>();
        static List<TezState> m_FunctionList = new List<TezState>();

        static TezState m_CurrentState = null;
        public static int current
        {
            get { return m_CurrentState.state; }
        }

        public static void initialization(Camera camera, EventSystem event_system)
        {
            m_EventSystem = event_system;
            m_Camera = camera;
        }

        public static void register(int state, TezEventBus.Action<Camera, EventSystem> function)
        {
            while (m_FunctionList.Count <= state)
            {
                m_FunctionList.Add(null);
            }

            m_FunctionList[state] = new TezState() { state = state, function = function };
        }

        public static void push(int state)
        {
            m_StateLogic.Push(m_FunctionList[state]);
            onPush?.Invoke(state);
            m_CurrentState = m_StateLogic.Peek();
        }

        public static void pop()
        {
            var state = m_StateLogic.Pop();
            onPop?.Invoke(state.state);
            m_CurrentState = m_StateLogic.Peek();
        }

        public static void change(int state)
        {
            pop();
            push(state);
        }

        public static void logic()
        {
            m_CurrentState.function(m_Camera, m_EventSystem);
        }
    }
}