using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public class TezPoolMB : MonoBehaviour
    {
        [SerializeField]
        bool m_Dynamic = false;
        [SerializeField]
        Dictionary<System.Type, Pool> m_Pools = new Dictionary<System.Type, Pool>();

        class Pool
        {
            Transform m_Parent;

            [SerializeField]
            Transform m_Prefab = null;
            [SerializeField]
            int m_InitCount = 0;

            List<Transform> m_Pool = new List<Transform>();
            int m_Pos = 0;

            public Pool(Transform transform)
            {
                m_Parent = transform;
            }

            public void init()
            {
                for (int i = 0; i < m_InitCount; i++)
                {
                    var obj = Instantiate(m_Prefab, m_Parent);
                    obj.gameObject.SetActive(false);
                    m_Pool.Add(obj);
                }
            }

            public Transform create(Transform parent)
            {
                Transform obj = null;

                if (m_Pos >= m_Pool.Count)
                {
                    obj = Object.Instantiate(m_Prefab, parent);
                    m_Pool.Add(obj);
                    m_Pos += 1;
                    return obj;
                }

                obj = m_Pool[m_Pos++];
                obj.transform.SetParent(parent);
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                obj.gameObject.SetActive(true);
                return obj;

            }

            public void recycle(Transform obj)
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(m_Parent);
                m_Pos -= 1;
                if (m_Pos < 0)
                {
                    m_Pos = 0;
                    m_Pool.Add(obj);
                }
                else
                {
                    m_Pool[m_Pos] = obj;
                }
            }

            public void clear()
            {
                foreach (var item in m_Pool)
                {
                    Destroy(item.gameObject);
                }

                m_Pool.Clear();
            }
        }
    }
}
