using System.Collections.Generic;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.InputSystem
{
    public class TezKeyConfigManager
    {
        public event TezEventExtension.Action<TezKeyWrapper> onPrepare;
        public event TezEventExtension.Action<TezKeyWrapper> onOK;
        public event TezEventExtension.Action<TezKeyWrapper> onCancel;

        public bool isWaitingKey { get { return m_ChangeKey != null; } }

        List<TezKeyConfigLayer> m_List = new List<TezKeyConfigLayer>();
        Dictionary<string, TezKeyConfigLayer> m_Dic = new Dictionary<string, TezKeyConfigLayer>();

        TezKeyWrapper m_ChangeKey = null;

        public TezKeyConfigLayer getOrCreateConfigLayer(string layerName)
        {
            if (!m_Dic.TryGetValue(layerName, out TezKeyConfigLayer configLayer))
            {
                configLayer = this.createConfigLayer();
                this.addConfig(layerName, configLayer);
            }

            return configLayer;
        }

        private void addConfig(string layerName, TezKeyConfigLayer configLayer)
        {
            configLayer.index = m_List.Count;
            m_List.Add(configLayer);
            m_Dic.Add(layerName, configLayer);
        }

        public void resetToDefault(TezReader reader)
        {
            this.readFromSave(reader);
        }

        public void resetToDefault(TezReader reader, string layerName)
        {
            reader.beginObject(layerName);
            m_Dic[layerName].resetToDefault(reader);
            reader.endObject(layerName);
        }

        protected virtual TezKeyConfigLayer createConfigLayer()
        {
            return new TezKeyConfigLayer();
        }

        public void setChangeKey(string layerName, string configName, int index)
        {
            this.setChangeKey(m_Dic[layerName].configs[configName].getWrapper(index));
        }

        public void setChangeKey(TezKeyWrapper wrapper)
        {
            wrapper.onPrepare += onChangeKeyPrepare;
            wrapper.onSave += onChangeKeySave;
            wrapper.onCancel += onChangeKeyCancel;

            wrapper.prepareChange();
        }

        private void resetChangeKey()
        {
            m_ChangeKey.onPrepare -= onChangeKeyPrepare;
            m_ChangeKey.onSave -= onChangeKeySave;
            m_ChangeKey.onCancel -= onChangeKeyCancel;
            m_ChangeKey = null;
        }

        protected virtual void onChangeKeyCancel(TezKeyWrapper wrapper)
        {
            this.resetChangeKey();
            onCancel?.Invoke(wrapper);
        }

        protected virtual void onChangeKeySave(TezKeyWrapper wrapper)
        {
            this.resetChangeKey();
            onOK?.Invoke(wrapper);
        }

        protected virtual void onChangeKeyPrepare(TezKeyWrapper wrapper)
        {
            m_ChangeKey = wrapper;
            onPrepare?.Invoke(wrapper);
        }

        public void waitingKey()
        {
            m_ChangeKey.waitingKey();
        }

        #region SaveLoad
        public void writeToSave(TezWriter writer)
        {
            foreach (var pair in m_Dic)
            {
                writer.beginObject(pair.Key);
                pair.Value.writeToSave(writer);
                writer.endObject(pair.Key);
            }
        }

        public void readFromSave(TezReader reader)
        {
            var layer_names = reader.getKeys();
            foreach (var layer_name in layer_names)
            {
                reader.beginObject(layer_name);
                m_Dic[layer_name].readFromSave(reader);
                reader.endObject(layer_name);
            }
        }
        #endregion

    }
}