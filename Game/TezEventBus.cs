using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{
    public static class TezEventExtension
    {
        public static void launch(this TezEventBus.Action action)
        {
            if(action != null)
            {
                action();
            }
        }

        public static void launch<T>(this TezEventBus.Action<T> action, T arg)
        {
            if (action != null)
            {
                action(arg);
            }
        }

        public static void launch<T1, T2>(this TezEventBus.Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                action(arg1, arg2);
            }
        }

        public static void launch<T1, T2, T3>(this TezEventBus.Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3);
            }
        }

        public static void launch<T1, T2, T3, T4>(this TezEventBus.Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
            }
        }

        public static void launch<T1, T2, T3, T4, T5>(this TezEventBus.Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4, arg5);
            }
        }

        public static void launch<T1, T2, T3, T4, T5, T6>(this TezEventBus.Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }

        public static void launch<T1, T2, T3, T4, T5, T6, T7>(this TezEventBus.Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
        }

        public static void launch<T1, T2, T3, T4, T5, T6, T7, T8>(this TezEventBus.Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
        }
    }

    public class TezEventBus : TezSingleton<TezEventBus>
    {
        public delegate void Action();
        public delegate void Action<T>(T arg);
        public delegate void Action<T1, T2>(T1 arg1, T2 arg2);
        public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
        public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

        public delegate R Function<R>();
        public delegate R Function<R, T>(T arg);
        public delegate R Function<R, T1, T2>(T1 arg1, T2 arg2);
        public delegate R Function<R, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
        public delegate R Function<R, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate R Function<R, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate R Function<R, T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate R Function<R, T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate R Function<R, T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);


        public abstract class Event
        {
            public abstract int getEventID();
        }

        List<Dictionary<object, Action<Event>>> m_Listeners = new List<Dictionary<object, Action<Event>>>();

        public void init(int max_event)
        {
            for (int i = 0; i < max_event; i++)
            {
                m_Listeners.Add(new Dictionary<object, Action<Event>>());
            }
        }

        public void subscribe(object obj, int event_id, Action<Event> function)
        {
            var dic = m_Listeners[event_id];
            dic.Add(obj, function);
        }

        public void unsubscribe(object obj, int event_id)
        {
            var dic = m_Listeners[event_id];
            dic.Remove(obj);
        }

        public void dispatchEvent(Event evt)
        {
            var dic = m_Listeners[evt.getEventID()];
            foreach (var pair in dic)
            {
                pair.Value(evt);
            }
        }
    }
}