using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.InputSystem
{
    public class TezInputController : ITezService
    {
        static void onDefault(TezInputState state) { }
        static void onDefault(TezInputState from, TezInputState to) { }

        TezEventExtension.Action<TezInputState> m_OnPop = onDefault;
        TezEventExtension.Action<TezInputState> m_OnPush = onDefault;
        TezEventExtension.Action<TezInputState> m_OnClear = onDefault;
        TezEventExtension.Action<TezInputState, TezInputState> m_OnTo = onDefault;

        TezInputState m_Current = null;
        Stack<TezInputState> m_Stack = new Stack<TezInputState>();

        class Handler<T> where T : TezInputState, new()
        {
            public static readonly T state = new T();
        }

        public bool isBlocked()
        {
            return m_Current.blocked;
        }

        public void setListeners(
            TezEventExtension.Action<TezInputState> on_pop,
            TezEventExtension.Action<TezInputState> on_push,
            TezEventExtension.Action<TezInputState> on_clear,
            TezEventExtension.Action<TezInputState, TezInputState> on_to)
        {
            m_OnPop = on_pop;
            m_OnPush = on_push;
            m_OnClear = on_clear;
            m_OnTo = on_to;
        }

        public void clear<State>(ITezTuple extra_data = null) where State : TezInputState, new()
        {
            while (m_Stack.Count > 0)
            {
                m_Stack.Pop().onExit();
            }

            m_Current?.onExit();
            m_Current = Handler<State>.state;
            m_Current.onEnter();
            m_Current.setExtraData(extra_data);
            m_OnClear(m_Current);
        }

        /// <summary>
        /// 将当前状态转变成
        /// </summary>
        public void to<State>(ITezTuple extra_data = null) where State : TezInputState, new()
        {
            var old = m_Current;
            m_Current?.onExit();
            m_Current = Handler<State>.state;
            m_Current.onEnter();
            m_Current.setExtraData(extra_data);
            m_OnTo(old, m_Current);
        }

        /// <summary>
        /// 压入一个新状态
        /// </summary>
        public void push<State>(ITezTuple extra_data = null) where State : TezInputState, new()
        {
            m_Stack.Push(m_Current);

            m_Current?.onExit();
            m_Current = Handler<State>.state;
            m_Current.onEnter();
            m_Current.setExtraData(extra_data);
            m_OnPush(m_Current);
        }

        public void pop<State>() where State : TezInputState, new()
        {
            if (m_Stack.Count == 0)
            {
                throw new ArgumentOutOfRangeException("InputController >> Input State Should Be Not Empty");
            }

            if (m_Current != Handler<State>.state)
            {
                throw new InvalidCastException(string.Format("InputController >> {0} To {1}",
                    m_Current.name,
                    Handler<State>.state.name));
            }

            m_OnPop(m_Current);
            m_Current.onExit();
            m_Current = m_Stack.Pop();
            m_Current.onEnter();
        }

        public void update()
        {
            m_Current.update();
        }

        public void close()
        {
            m_Stack.Clear();
            m_Stack = null;

            m_OnClear = null;
            m_OnPop = null;
            m_OnPush = null;
            m_OnTo = null;
        }
    }
}