using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Layer层
    /// 
    /// 用于在多种操作模式下设定各自的按键规则
    /// 
    /// 比如
    /// Layer1设置在战斗状态下A.B键的作用
    /// Layer2设置在普通状态下A,B键的作用
    /// 防止按键冲突
    /// </summary>
    public class TezKeyConfigLayer : ITezCloseable
    {
        public IReadOnlyDictionary<string, TezKeyConfig> configs => m_Dic;
        public int index { get; set; }

        Dictionary<string, TezKeyConfig> m_Dic = new Dictionary<string, TezKeyConfig>();

        public void addConfig(TezKeyConfig config)
        {
            m_Dic.Add(config.name, config);
        }

        public bool tryGetConfig(string name, out TezKeyConfig config)
        {
            return m_Dic.TryGetValue(name, out config);
        }

        public void resetToDefault(TezReader reader)
        {
            this.readFromSave(reader);
        }

        void ITezCloseable.closeThis()
        {
            m_Dic.Clear();
        }

        #region SaveLoad
        public void writeToSave(TezWriter writer)
        {
            foreach (var pair in m_Dic)
            {
                writer.beginArray(pair.Key);
                pair.Value.writeToSave(writer);
                writer.endArray(pair.Key);
            }
        }

        public void readFromSave(TezReader reader)
        {
            var keys = reader.getKeys();
            foreach (var key in keys)
            {
                reader.beginArray(key);
                m_Dic[key].readFromSave(reader);
                reader.endArray(key);
            }
        }
        #endregion
    }
}