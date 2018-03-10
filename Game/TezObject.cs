﻿using System;
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

        public virtual void initialization()
        {

        }

        public virtual void clear()
        {
            this.sharedItem = null;
            m_ObjectName.reset();
        }

        public void setItem(TezItem item)
        {
            this.onItemSet(item);
            this.sharedItem = item;
        }

        protected abstract void onItemSet(TezItem item);

        public TezItem generateItem()
        {
            var item = this.onGenerateItem();
            return item;
        }

        protected virtual TezItem onGenerateItem()
        {
            return null;
        }

        public virtual int prefabID()
        {
            return -1;
        }

        public void pushToGenerator()
        {
            TezGenerator.instance.pushObject(this);
        }

        public void pushToGenerator(Transform parent, Vector3 position)
        {
            TezGenerator.instance.pushObject(this, parent, position);
        }
    }
}

