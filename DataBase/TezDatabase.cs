using System.Collections.Generic;
using tezcat.Core;
using tezcat.TypeTraits;

namespace tezcat.DataBase
{
    public sealed class TezDatabase : ITezService
    {
        public delegate void TypeChecker(System.Type type, int id);
        public delegate void ProtoChecker(ITezPrototype prototype);

        interface IContainer : ITezCloseable
        {
            void foreachData(ProtoChecker checker);
            System.Type GetType();
        }

        class Container<T>
            : IContainer
            where T : ITezPrototype<T>
        {
            Dictionary<string, T> m_NameDic = new Dictionary<string, T>();

            public void add(string name, T prototype)
            {
                m_NameDic[name] = prototype;
            }

            public void remove(string name)
            {
                m_NameDic.Remove(name);
            }

            public T get(string name)
            {
                return m_NameDic[name];
            }

            public void close()
            {
                m_NameDic.Clear();
                m_NameDic = null;
            }

            public void foreachData(ProtoChecker checker)
            {
                foreach (var pair in m_NameDic)
                {
                    checker(pair.Value);
                }
            }
        }

        sealed class DataID<Type> : TezTypeInfo<Type, TezDatabase>
        {
            private DataID() { }
        }

        Dictionary<string, int> m_DataDic = new Dictionary<string, int>();
        List<IContainer> m_DataList = new List<IContainer>();

        public void register<T>() where T : ITezPrototype<T>
        {
            DataID<T>.setID(m_DataList.Count);
            while (DataID<T>.ID >= m_DataList.Count)
            {
                m_DataList.Add(null);
            }

            m_DataList[DataID<T>.ID] = new Container<T>();
        }

        public void add<T>(T data) where T : ITezPrototype<T>
        {
            var cg = (Container<T>)m_DataList[DataID<T>.ID];
            cg.add(data.prototypeName, data);
        }

        public void remove<T>(T data) where T : ITezPrototype<T>
        {
            var cg = (Container<T>)m_DataList[DataID<T>.ID];
            cg.remove(data.prototypeName);
        }

        public T get<T>(string name) where T : ITezPrototype<T>
        {
            return ((Container<T>)m_DataList[DataID<T>.ID]).get(name);
        }

        public void foreachData(TypeChecker type_checker, ProtoChecker proto_checker)
        {
            for (int i = 0; i < m_DataList.Count; i++)
            {
                var container = m_DataList[i];
                type_checker(container.GetType(), i);
                container.foreachData(proto_checker);
            }
        }

        public void close()
        {
            foreach (var container in m_DataList)
            {
                container.close();
            }

            m_DataList.Clear();
            m_DataDic.Clear();

            m_DataList = null;
            m_DataDic = null;
        }
    }
}