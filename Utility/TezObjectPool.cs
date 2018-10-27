using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// Begin End
    /// 他们分别指向可被使用的最上面的一个和最下面的一个
    /// 他们之间的位置表示是被使用的Obj
    /// 
    /// ======
    ///  |
    ///  |
    ///  |  -----Begin
    ///  |
    ///  |
    ///  |
    ///  |
    ///  |
    ///  |
    ///  |  -----End
    ///  |
    ///  |
    /// ======
    /// 
    /// </summary>
    public abstract class TezObjectPool<T> : MonoBehaviour where T : MonoBehaviour, new()
    {
        bool m_Inited = false;
        T m_Prefab;
        List<T> m_Pool = new List<T>();

        int m_PointBegin = -1;
        int m_PointEnd = 0;

        public void init(int size, T prefab)
        {
            m_Inited = true;
            m_Prefab = prefab;
            for (int i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(m_Prefab);
                obj.gameObject.SetActive(false);
                m_Pool.Add(obj);
            }
        }

        public T create(Transform parent)
        {
            T obj = null;

            if (m_PointEnd >= m_Pool.Count)
            {
                if (m_PointBegin > -1)
                {
                    obj = m_Pool[m_PointBegin];
                    m_PointBegin -= 1;
                    return obj;
                }
                else
                {
                    obj = Object.Instantiate(m_Prefab, parent);
                    m_Pool.Add(obj);
                    return obj;
                }
            }
            else
            {
                obj = m_Pool[m_PointEnd++];
                obj.transform.parent = parent;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        public void recycle(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(this.transform);
            m_PointBegin += 1;
            m_Pool[m_PointBegin] = obj;

            if (m_PointBegin + 1 == m_PointEnd)
            {
                m_PointBegin = -1;
                m_PointEnd = 0;
            }
        }
    }
}