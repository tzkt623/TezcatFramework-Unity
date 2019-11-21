using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.InputSystem
{
    public class TezInputController : ITezService
    {
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
            Debug.Log(string.Format("InputController >> Stack {0}", m_Stack.Count));
        }

        /// <summary>
        /// 将当前状态转变成
        /// </summary>
        public void to<State>(ITezTuple extra_data = null) where State : TezInputState, new()
        {
            m_Current?.onExit();
            m_Current = Handler<State>.state;
            m_Current.onEnter();
            m_Current.setExtraData(extra_data);
            Debug.Log(string.Format("InputController >> Stack {0}", m_Stack.Count));
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
            Debug.Log(string.Format("InputController >> Stack {0}", m_Stack.Count));
        }

        /// <summary>
        /// 如果与当前压入的状态参数不同
        /// 则压入当前状态
        /// 否则不变
        /// </summary>
        public void pushIfNot<State>(ITezTuple extra_data = null) where State : TezInputState, new()
        {
            if (m_Current != null)
            {
                if (m_Current != Handler<State>.state)
                {
                    this.push<State>(extra_data);
                }
            }
            else
            {
                this.push<State>(extra_data);
            }
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

            m_Current.onExit();
            m_Current = m_Stack.Pop();
            m_Current.onEnter();
            Debug.Log(string.Format("InputController >> Stack {0}", m_Stack.Count));
        }

        public void update()
        {
            m_Current.update();
        }

        public void close()
        {
            m_Stack.Clear();
            m_Stack = null;
        }
    }
}