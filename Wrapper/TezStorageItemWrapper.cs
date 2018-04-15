using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Wrapper
{
    public interface ITezStorageItemWrapper : ITezObjectWrapper
    {
        int count { get; }
        ITezItem item { get; }

        void add(int count);
        void remove(int count);

        void showTip();
        void hideTip();
    }

    public class TezStorageItemWrapper : ITezStorageItemWrapper
    {
        TezStorage m_Storage = null;
        int m_ID = 0;

        public string name
        {
            get { return Localization.getName(item.name); }
        }

        public string description
        {
            get { return Localization.getDescription(item.name); }
        }

        public int count
        {
            get { return m_Storage.getItemCount(m_ID); }
        }

        public ITezItem item
        {
            get { return m_Storage.getItem(m_ID); }
        }

        public TezStorageItemWrapper(int id, TezStorage storage)
        {
            m_Storage = storage;
            m_ID = id;
        }

        public void add(int count)
        {
            m_Storage.add(m_ID, count);
        }

        public void remove(int count)
        {
            m_Storage.remove(m_ID, count);
        }

        public virtual void clear()
        {
            m_Storage = null;
            m_ID = -1;
        }

        public virtual void showTip()
        {

        }

        public virtual void hideTip()
        {

        }
    }
}

