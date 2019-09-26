using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public abstract class TezResource<T>
        : ITezService
        where T : ITezPrototype<T>
    {
        Dictionary<string, int> m_Dic = new Dictionary<string, int>();
        List<T> m_List = new List<T>();

        public void load(TezReader reader)
        {
            var count = reader.count;
            for (int i = 0; i < count; i++)
            {
                reader.beginObject(i);
                this.onLoad(i, reader);
                reader.endObject(i);
            }
        }

        public void save(TezWriter writer)
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                writer.beginObject(i);
                this.onSave(i, writer);
                writer.endObject(i);
            }
        }

        public T get(string name)
        {
            return m_List[m_Dic[name]].clone();
        }

        public T get(int index)
        {
            return m_List[index].clone();
        }

        public virtual void close()
        {
            m_Dic.Clear();
            m_List.Clear();

            m_Dic = null;
            m_List = null;
        }

        protected abstract void onLoad(int index, TezReader reader);
        protected abstract void onSave(int index, TezWriter writer);
    }
}

