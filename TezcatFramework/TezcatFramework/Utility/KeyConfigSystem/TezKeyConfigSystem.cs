using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 
    /// 按键配置管理器类
    /// 
    /// 具体使用方法看TestKeyConfig
    /// 
    /// </summary>
    public class TezKeyConfigManager : ITezCloseable
    {
        public event TezEventExtension.Action<TezKeyWrapper> evtPrepare;
        public event TezEventExtension.Action<TezKeyWrapper> evtOK;
        public event TezEventExtension.Action<TezKeyWrapper> evtCancel;

        public bool isWaitingChangeKey { get { return mChangeKey != null; } }

        List<TezKeyConfigLayer> mList = new List<TezKeyConfigLayer>();
        Dictionary<string, TezKeyConfigLayer> mDict = new Dictionary<string, TezKeyConfigLayer>();

        TezKeyWrapper mChangeKey = null;

        public virtual void close()
        {
            for (int i = 0; i < mList.Count; i++)
            {
                mList[i].close();
            }
            mList.Clear();
            mDict.Clear();

            mList = null;
            mDict = null;
            mChangeKey = null;

            this.evtPrepare = null;
            this.evtOK = null;
            this.evtCancel = null;
        }

        public TezKeyConfigLayer getOrCreateConfigLayer(string layerName)
        {
            if (!mDict.TryGetValue(layerName, out TezKeyConfigLayer configLayer))
            {
                configLayer = this.createConfigLayer();
                this.addConfig(layerName, configLayer);
            }

            return configLayer;
        }

        private void addConfig(string layerName, TezKeyConfigLayer configLayer)
        {
            configLayer.index = mList.Count;
            mList.Add(configLayer);
            mDict.Add(layerName, configLayer);
        }

        public void resetToDefault(TezReader reader)
        {
            this.readFromSave(reader);
        }

        public void resetToDefault(TezReader reader, string layerName)
        {
            reader.beginObject(layerName);
            mDict[layerName].resetToDefault(reader);
            reader.endObject(layerName);
        }

        protected virtual TezKeyConfigLayer createConfigLayer()
        {
            return new TezKeyConfigLayer();
        }

        public void setChangeKey(string layerName, string configName, int index)
        {
            this.setChangeKey(mDict[layerName].configs[configName].getWrapper(index));
        }

        public void setChangeKey(TezKeyWrapper wrapper)
        {
            wrapper.evtPrepare += onChangeKeyPrepare;
            wrapper.evtSave += onChangeKeySave;
            wrapper.evtCancel += onChangeKeyCancel;

            wrapper.prepareChange();
        }

        private void resetChangeKey()
        {
            mChangeKey.evtPrepare -= onChangeKeyPrepare;
            mChangeKey.evtSave -= onChangeKeySave;
            mChangeKey.evtCancel -= onChangeKeyCancel;
            mChangeKey = null;
        }

        protected virtual void onChangeKeyCancel(TezKeyWrapper wrapper)
        {
            this.resetChangeKey();
            evtCancel?.Invoke(wrapper);
        }

        protected virtual void onChangeKeySave(TezKeyWrapper wrapper)
        {
            this.resetChangeKey();
            evtOK?.Invoke(wrapper);
        }

        protected virtual void onChangeKeyPrepare(TezKeyWrapper wrapper)
        {
            mChangeKey = wrapper;
            evtPrepare?.Invoke(wrapper);
        }

        /// <summary>
        /// 等待改建按键
        /// </summary>
        public void waitingChangeKey()
        {
            mChangeKey.waitingKey();
        }

        #region SaveLoad
        public void writeToSave(TezWriter writer)
        {
            foreach (var pair in mDict)
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
                mDict[layer_name].readFromSave(reader);
                reader.endObject(layer_name);
            }
        }
        #endregion
    }
}