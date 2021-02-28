using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.ECS
{
    public class TezEntityManager
    {
        public static TezEntityManager instance { get; } = new TezEntityManager();

        Queue<TezEntity> m_FreeEntity = new Queue<TezEntity>();

        protected TezEntityManager()
        {

        }

        public TezEntity create()
        {
            if (m_FreeEntity.Count > 0)
            {
                return m_FreeEntity.Dequeue();
            }

            return null;
        }

        public void recycle(TezEntity entity)
        {
            m_FreeEntity.Enqueue(entity);
        }

        public void close()
        {
            m_FreeEntity.Clear();
            m_FreeEntity = null;
        }
    }
}