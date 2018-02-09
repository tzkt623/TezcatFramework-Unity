using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public abstract class TezObject
    {
        public string name { get; private set; }

        public TezResUID resUID
        {
            get; protected set;
        }

        TezUID m_UID = new TezUID();
        public TezUID UID
        {
            get { return m_UID; }
        }

        public TezObject()
        {

        }

        public virtual void initialization()
        {
            resUID = null;
            name = null;
        }

        public virtual void clear()
        {
            this.resUID = null;
        }

        public void setItem(int group_id, int type_id, int self_id)
        {
            var item = TezResourceSystem.instance.getItem(group_id, type_id, self_id);
            this.resUID = item.resUID;
            this.name = item.name;
            this.setItem(item);
        }

        public void setItem(TezItem item)
        {
            this.onItemSet(item);
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
            TezDebug.isTrue(this.prefabID() >= 0, "TezObject", "pushToGenerator", "no prefab");
            TezGenerator.instance.pushObject(this);
        }

        public void pushToGenerator(Transform parent, Vector3 position)
        {
            TezDebug.isTrue(this.prefabID() >= 0, "TezObject", "pushToGenerator", "no prefab");
            TezGenerator.instance.pushObject(this, parent, position);
        }
    }
}

