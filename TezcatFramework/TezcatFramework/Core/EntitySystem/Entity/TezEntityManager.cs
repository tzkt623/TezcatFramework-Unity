using System.Collections.Generic;

namespace tezcat.Framework.ECS
{
    public class TezEntityManager
    {
        public static TezEntityManager instance { get; } = new TezEntityManager();

        Queue<TezEntity> mFreeEntity = new Queue<TezEntity>();

        protected TezEntityManager()
        {

        }

        public TezEntity create()
        {
            if (mFreeEntity.Count > 0)
            {
                var entity = mFreeEntity.Dequeue();
                return entity;
            }

            return null;
        }

        public void recycle(TezEntity entity)
        {
            mFreeEntity.Enqueue(entity);
        }

        public void close()
        {
            mFreeEntity.Clear();
            mFreeEntity = null;
        }
    }
}