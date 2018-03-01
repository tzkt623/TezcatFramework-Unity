using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public abstract class TezObject
    {
        public TezUID UID { get; private set; }

        public int objectID { get; private set; }

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
            m_ObjectName.reset();
        }

        public void setItem(int group_id, int type_id, int self_id)
        {
            var item = GameDataBase.DB.getItem(group_id, type_id, self_id);
            this.setItem(item);
        }

        public void setItem(TezItem item)
        {
            objectID = item.objectID;
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

