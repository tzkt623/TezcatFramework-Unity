using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public abstract class TezObjectMB : MonoBehaviour
    {
        public static T createMB<T>(T obj) where T : TezObjectMB, new()
        {
            return Instantiate<T>(obj);
        }

        private void Awake()
        {
            this.onAwake();
        }

        private void Start()
        {
            var data = TezGenerator.instance.get(this.classID());
            if(data != null)
            {
                this.transform.parent = data.parent;
                this.transform.localPosition = data.position;
                this.onStart(data.obj);
            }
        }

        private void OnDestroy()
        {
            this.onDelete();
        }

        public virtual void onAwake()
        {

        }

        public virtual void onStart(TezObject obj)
        {

        }

        public virtual void onDelete()
        {

        }

        /// <summary>
        /// 与Object拥有同一个ID
        /// 用于prefab的注册
        /// </summary>
        /// <returns></returns>
        public virtual int classID()
        {
            return -1;
        }
    }
}