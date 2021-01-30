using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.InputSystem
{
    public abstract class TezKeyWrapper : ITezCloseable
    {
        public event TezEventExtension.Action<TezKeyWrapper> onPrepare;
        public event TezEventExtension.Action<TezKeyWrapper> onSave;
        public event TezEventExtension.Action<TezKeyWrapper> onCancel;

        public virtual string name { get; }

        public abstract bool active();
        public virtual bool deactive()
        {
            return !this.active();
        }

        public void waitingKey()
        {
            if (this.onChangeKeyDown())
            {
                this.saveChange();
            }
        }

        protected abstract bool onChangeKeyDown();

        public virtual void prepareChange()
        {
            onPrepare?.Invoke(this);
        }

        public virtual void cancelChange()
        {
            onCancel?.Invoke(this);
        }

        public virtual void saveChange()
        {
            onSave?.Invoke(this);
        }

        public virtual void close()
        {
            onCancel = null;
            onSave = null;
            onPrepare = null;
        }
    }

    public abstract class TezKeyConfig
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

        public TezKeyWrapper getWrapper(int index)
        {
            return m_Wrappers[index];
        }
    }

    public class TezKeyConfigSet : ITezCloseable
    {
        public IReadOnlyDictionary<string, TezKeyConfig> configs => m_Dic;
        public int index { get; set; }

        Dictionary<string, TezKeyConfig> m_Dic = new Dictionary<string, TezKeyConfig>();

        public void addConfig(TezKeyConfig config)
        {
            m_Dic.Add(config.name, config);
        }

        public void close()
        {
            m_Dic.Clear();
        }
    }

    public class TezKeyConfigManager
    {
        public event TezEventExtension.Action<TezKeyWrapper> onPrepare;
        public event TezEventExtension.Action<TezKeyWrapper> onOK;
        public event TezEventExtension.Action<TezKeyWrapper> onCancel;

        List<TezKeyConfigSet> m_List = new List<TezKeyConfigSet>();
        Dictionary<string, TezKeyConfigSet> m_Dic = new Dictionary<string, TezKeyConfigSet>();

        TezKeyWrapper m_ChangeKey = null;

        public TezKeyConfigSet getOrCreateConfigSet(string layerName)
        {
            if (!m_Dic.TryGetValue(layerName, out TezKeyConfigSet configSet))
            {
                configSet = new TezKeyConfigSet()
                {
                    index = m_List.Count
                };
                m_List.Add(configSet);
                m_Dic.Add(layerName, configSet);
            }

            return configSet;
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
            m_ChangeKey?.waitingKey();
        }
    }
}