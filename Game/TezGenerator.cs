using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace tezcat
{
    public class TezGenerator : TezSingleton<TezGenerator>
    {
        public class Data
        {
            public TezObject obj;
            public Vector3 position;
            public Transform parent;
        }

        List<TezObjectMB> m_PrefabList = new List<TezObjectMB>();
        List<Stack<Data>> m_Datas = new List<Stack<Data>>();

        public void register(TezObjectMB obj)
        {
            Assert.IsTrue(obj.classID() > -1);

            while (obj.classID() >= m_PrefabList.Count)
            {
                m_PrefabList.Add(null);
            }

            m_PrefabList[obj.classID()] = obj;

            while(obj.classID() >= m_Datas.Count)
            {
                m_Datas.Add(new Stack<Data>());
            }
        }

        public Data get(int class_id)
        {
#if Tez_Debug
            TezDebug.info("TezGenerator", "get", class_id.ToString() + "|" + m_Datas[class_id].Count.ToString());
#endif
            return m_Datas[class_id].Pop();
        }

        public void pushObject(TezObject obj, Transform parent, Vector3 position)
        {
            int class_id = obj.prefabID();
            m_Datas[class_id].Push(new Data()
            {
                obj = obj,
                parent = parent,
                position = position
            });

            UnityEngine.Object.Instantiate(m_PrefabList[class_id], parent);
        }

        public void pushObject(TezObject obj)
        {
            int class_id = obj.prefabID();
            m_Datas[class_id].Push(new Data()
            {
                obj = obj,
                parent = null,
                position = Vector3.zero
            });

            UnityEngine.Object.Instantiate(m_PrefabList[class_id]);
        }
    }
}