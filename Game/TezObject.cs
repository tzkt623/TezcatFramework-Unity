using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public abstract class TezObject
    {
        public TezUID UID { get; private set; }

        public TezItem sharedItem
        {
            get; protected set;
        }

        TezStaticString m_ObjectName = new TezStaticString();
        public string objectName
        {
            get { return m_ObjectName.convertToString(); }
        }

        public string customName { get; set; }


        public TezObject()
        {
            UID = new TezUID();
        }

        public virtual void clear()
        {
            this.sharedItem = null;
            m_ObjectName.reset();
        }

        public void setItem(TezItem item)
        {
            this.sharedItem?.subRef();
            this.sharedItem = item;
            this.sharedItem.addRef();
            this.onItemSet(item);
        }

        protected abstract void onItemSet(TezItem item);
    }
}

