using tezcat.Framework.Extension;

namespace tezcat.Event
{
    public abstract class TezBaseAction
    {
        protected int m_Count = 0;
        public abstract void clear();
    }

    #region Event
    /// <summary>
    /// 
    /// </summary>
    public class TezAction : TezBaseAction
    {
        TezEventExtension.Action m_OnHandler = null;

        public TezAction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Action action)
        {
            if(m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Action action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public void invoke()
        {
            m_OnHandler();
        }

        void defaultFunction()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class TezAction<T1> : TezBaseAction
    {
        TezEventExtension.Action<T1> m_OnHandler = null;

        public TezAction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Action<T1> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Action<T1> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public void invoke(T1 t1)
        {
            m_OnHandler(t1);
        }

        void defaultFunction(T1 t1)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class TezAction<T1, T2> : TezBaseAction
    {
        TezEventExtension.Action<T1, T2> m_OnHandler = null;

        public TezAction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Action<T1, T2> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Action<T1, T2> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public void invoke(T1 t1, T2 t2)
        {
            m_OnHandler(t1, t2);
        }

        void defaultFunction(T1 t1, T2 t2)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class TezAction<T1, T2, T3> : TezBaseAction
    {
        TezEventExtension.Action<T1, T2, T3> m_OnHandler = null;

        public TezAction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Action<T1, T2, T3> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Action<T1, T2, T3> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public void invoke(T1 t1, T2 t2, T3 t3)
        {
            m_OnHandler(t1, t2, t3);
        }

        void defaultFunction(T1 t1, T2 t2, T3 t3)
        {

        }
    }
    #endregion

    #region Function
    public class TezFunction<R> : TezBaseAction
    {
        TezEventExtension.Function<R> m_OnHandler = null;

        public TezFunction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Function<R> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Function<R> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public R invoke()
        {
            return m_OnHandler();
        }

        R defaultFunction()
        {
            return default(R);
        }
    }

    public class TezFunction<R, T1> : TezBaseAction
    {
        TezEventExtension.Function<R, T1> m_OnHandler = null;

        public TezFunction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Function<R, T1> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Function<R, T1> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public R invoke(T1 t1)
        {
            return m_OnHandler(t1);
        }

        R defaultFunction(T1 t1)
        {
            return default(R);
        }
    }

    public class TezFunction<R, T1, T2> : TezBaseAction
    {
        TezEventExtension.Function<R, T1, T2> m_OnHandler = null;

        public TezFunction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Function<R, T1, T2> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Function<R, T1, T2> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public R invoke(T1 t1, T2 t2)
        {
            return m_OnHandler(t1, t2);
        }

        R defaultFunction(T1 t1, T2 t2)
        {
            return default(R);
        }
    }

    public class TezFunction<R, T1, T2, T3> : TezBaseAction
    {
        TezEventExtension.Function<R, T1, T2, T3> m_OnHandler = null;

        public TezFunction()
        {
            m_OnHandler = this.defaultFunction;
        }

        public void add(TezEventExtension.Function<R, T1, T2, T3> action)
        {
            if (m_Count == 0)
            {
                m_OnHandler = action;
            }
            else
            {
                m_OnHandler += action;
            }
            m_Count += 1;
        }

        public void remove(TezEventExtension.Function<R, T1, T2, T3> action)
        {
            m_Count -= 1;
            if (m_Count == 0)
            {
                m_OnHandler = this.defaultFunction;
            }
            else
            {
                m_OnHandler -= action;
            }
        }

        public override void clear()
        {
            m_OnHandler = null;
        }

        public R invoke(T1 t1, T2 t2, T3 t3)
        {
            return m_OnHandler(t1, t2, t3);
        }

        R defaultFunction(T1 t1, T2 t2, T3 t3)
        {
            return default(R);
        }
    }
    #endregion
}