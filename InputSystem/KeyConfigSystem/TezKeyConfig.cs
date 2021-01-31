using System;
using System.Collections;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.InputSystem
{
    /// <summary>
    /// 具体配置类
    /// 通过和TezKeyWrapper配合
    /// 来配置具体按键规则以触发按键效果
    /// </summary>
    public abstract class TezKeyConfig : ITezCloseable
    {
        public string name { get; }

        TezKeyWrapper[] m_Wrappers = null;

        public TezKeyConfig(string name, int keyCount)
        {
            this.name = name;
            m_Wrappers = new TezKeyWrapper[keyCount];
        }

        public abstract bool active();

        public void setWrapper(int index, TezKeyWrapper wrapper)
        {
            m_Wrappers[index] = wrapper;
        }

        public void resetToDefault(TezReader reader)
        {
            this.readFromSave(reader);
        }

        public TezKeyWrapper getWrapper(int index)
        {
            return m_Wrappers[index];
        }

        public void writeToSave(TezWriter writer)
        {
            for (int i = 0; i < m_Wrappers.Length; i++)
            {
                writer.beginObject(i);
                m_Wrappers[i].writeToSave(writer);
                writer.endObject(i);
            }
        }
        public void readFromSave(TezReader reader)
        {
            for (int i = 0; i < reader.count; i++)
            {
                reader.beginObject(i);
                m_Wrappers[i].readFromSave(reader);
                reader.endObject(i);
            }
        }

        public virtual void close()
        {
            foreach (var item in m_Wrappers)
            {
                item.close();
            }
            m_Wrappers = null;
        }
    }
}