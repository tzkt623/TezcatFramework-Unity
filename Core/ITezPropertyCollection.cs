using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezPropertyCollection
        : IEnumerable
        , ITezCloseable
    {
        int count { get; }

        void register(TezValueWrapper vw);
        bool unregister(ITezValueDescriptor name);

        TezValueWrapper get(ITezValueDescriptor name);
        TezValueWrapper<T> get<T>(ITezValueDescriptor name);

        TezValueWrapper get(int index);
        TezValueWrapper<T> get<T>(int index);

        void set<T>(ITezValueDescriptor name, TezEventExtension.Action<TezValueWrapper<T>> action);

        bool has(ITezValueDescriptor name);

        void sort();

        void copyFrom(ITezPropertyCollection collection);

        void foreachProperty(TezEventExtension.Action<TezValueWrapper> action);
    }

    public class TezPropertyList : ITezPropertyCollection
    {
        List<TezValueWrapper> m_List = new List<TezValueWrapper>();


        int ITezPropertyCollection.count
        {
            get { return m_List.Count; }
        }

        void ITezPropertyCollection.register(TezValueWrapper vw)
        {
            m_List.Add(vw);
        }

        bool ITezPropertyCollection.unregister(ITezValueDescriptor name)
        {
            var index = m_List.FindIndex((TezValueWrapper vw) =>
            {
                return (vw.descriptor == name);
            });

            if (index > -1)
            {
                m_List.RemoveAt(index);
                return true;
            }

            return false;
        }

        void ITezPropertyCollection.set<T>(ITezValueDescriptor name, TezEventExtension.Action<TezValueWrapper<T>> action)
        {
            var result = m_List.Find((TezValueWrapper vw) =>
            {
                return (vw.descriptor == name);
            });

            if (result)
            {
                action((TezValueWrapper<T>)result);
            }
            else
            {

            }
        }

        TezValueWrapper ITezPropertyCollection.get(ITezValueDescriptor name)
        {
            TezValueWrapper result;
            m_List.binaryFind(name.ID, out result);
            return result;
        }

        TezValueWrapper<T> ITezPropertyCollection.get<T>(ITezValueDescriptor name)
        {
            TezValueWrapper result;
            m_List.binaryFind(name.ID, out result);
            return (TezValueWrapper<T>)result;
        }

        TezValueWrapper ITezPropertyCollection.get(int index)
        {
            return m_List[index];
        }

        TezValueWrapper<T> ITezPropertyCollection.get<T>(int index)
        {
            return (TezValueWrapper<T>)m_List[index];
        }

        bool ITezPropertyCollection.has(ITezValueDescriptor name)
        {
            TezValueWrapper result;
            return m_List.binaryFind(name.ID, out result);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        void ITezPropertyCollection.sort()
        {
            m_List.Sort();
        }

        void ITezPropertyCollection.copyFrom(ITezPropertyCollection collection)
        {
            var list = collection as TezPropertyList;
            if(list != null)
            {
                m_List.Clear();
                m_List = new List<TezValueWrapper>(list.m_List);
            }
        }

        void ITezPropertyCollection.foreachProperty(TezEventExtension.Action<TezValueWrapper> action)
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                action(m_List[i]);
            }
        }

        void ITezCloseable.close()
        {
            m_List.Clear();
            m_List = null;
        }
    }

    public class TezPropertyDic : ITezPropertyCollection
    {
        Dictionary<ITezValueDescriptor, TezValueWrapper> m_PropertyDic = new Dictionary<ITezValueDescriptor, TezValueWrapper>();

        int ITezPropertyCollection.count
        {
            get { return m_PropertyDic.Count; }
        }

        void ITezPropertyCollection.register(TezValueWrapper vw)
        {
            m_PropertyDic.Add(vw.descriptor, vw);
        }

        bool ITezPropertyCollection.unregister(ITezValueDescriptor name)
        {
            return m_PropertyDic.Remove(name);
        }

        TezValueWrapper ITezPropertyCollection.get(ITezValueDescriptor name)
        {
            return m_PropertyDic[name];
        }

        TezValueWrapper<T> ITezPropertyCollection.get<T>(ITezValueDescriptor name)
        {
            return (TezValueWrapper<T>)m_PropertyDic[name];
        }

        TezValueWrapper ITezPropertyCollection.get(int index)
        {
            int count = 0;
            var keys = m_PropertyDic.Keys;
            foreach (var key in keys)
            {
                if(count == index)
                {
                    return m_PropertyDic[key];
                }
                count += 1;
            }

            return null;
        }

        TezValueWrapper<T> ITezPropertyCollection.get<T>(int index)
        {
            int count = 0;
            var keys = m_PropertyDic.Keys;
            foreach (var key in keys)
            {
                if (count == index)
                {
                    return (TezValueWrapper<T>)m_PropertyDic[key];
                }
                count += 1;
            }

            return null;
        }

        void ITezPropertyCollection.set<T>(ITezValueDescriptor name, TezEventExtension.Action<TezValueWrapper<T>> action)
        {
            TezValueWrapper vw = null;
            if (m_PropertyDic.TryGetValue(name, out vw))
            {
                action((TezValueWrapper<T>)vw);
            }
            else
            {

            }
        }

        bool ITezPropertyCollection.has(ITezValueDescriptor name)
        {
            return m_PropertyDic.ContainsKey(name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_PropertyDic.GetEnumerator();
        }

        void ITezPropertyCollection.sort()
        {

        }

        public void copyFrom(ITezPropertyCollection collection)
        {
            var dic = collection as TezPropertyDic;
            if (dic != null)
            {
                m_PropertyDic.Clear();
                m_PropertyDic = new Dictionary<ITezValueDescriptor, TezValueWrapper>(dic.m_PropertyDic);
            }
        }

        void ITezPropertyCollection.foreachProperty(TezEventExtension.Action<TezValueWrapper> action)
        {
            foreach (var pair in m_PropertyDic)
            {
                action(pair.Value);
            }
        }

        void ITezPropertyCollection.copyFrom(ITezPropertyCollection collection)
        {
            var dic = collection as TezPropertyDic;
            if (dic != null)
            {
                m_PropertyDic.Clear();
                m_PropertyDic = new Dictionary<ITezValueDescriptor, TezValueWrapper>(dic.m_PropertyDic);
            }
        }

        void ITezCloseable.close()
        {
            m_PropertyDic.Clear();
            m_PropertyDic = null;
        }
    }
}
