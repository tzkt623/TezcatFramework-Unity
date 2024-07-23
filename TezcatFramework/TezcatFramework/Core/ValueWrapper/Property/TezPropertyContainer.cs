using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public abstract class TezPropertyContainer : ITezCloseable
    {
        public abstract T create<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new();
        public abstract bool remove(ITezValueDescriptor descriptor);
        public abstract void clear();

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }
        protected abstract void onClose();

        public abstract ITezProperty get(int id);
        public abstract ITezProperty get(ITezValueDescriptor vd);

        public abstract bool tryGet(int id, out ITezProperty property);
        public virtual bool tryGet(ITezValueDescriptor vd, out ITezProperty property)
        {
            return this.tryGet(vd.ID, out property);
        }
    }

    public class TezPropertyListContainer : TezPropertyContainer
    {
        List<ITezProperty> mList = new List<ITezProperty>();

        public override T create<T>(ITezValueDescriptor descriptor)
        {
            var id = descriptor.ID;
            if (mList.Count <= id)
            {
                mList.AddRange(new ITezProperty[id - mList.Count + 1]);
            }

            T property = (T)mList[id];
            if (property != null)
            {
                return property;
            }

            property = new T();
            property.descriptor = descriptor;
            mList[id] = property;
            return property;
        }

        public override bool remove(ITezValueDescriptor descriptor)
        {
            var id = descriptor.ID;
            var result = mList[id];

            if (result != null)
            {
                result.close();
                mList[id] = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 清理所有属性数据
        /// </summary>
        public override void clear()
        {
            for (int i = 0; i < mList.Count; i++)
            {
                mList[i]?.close();
            }

            mList.Clear();
        }

        /// <summary>
        /// 清理并关闭
        /// </summary>
        protected override void onClose()
        {
            this.clear();
            mList = null;
        }

        public override ITezProperty get(int id)
        {
            return mList[id];
        }

        public override ITezProperty get(ITezValueDescriptor vd)
        {
            return mList[vd.ID];
        }

        public override bool tryGet(int id, out ITezProperty property)
        {
            property = mList[id];
            return property != null;
        }
    }

    public class TezPropertySortListContainer : TezPropertyContainer
    {
        TezValueSortList<ITezProperty> mList = new TezValueSortList<ITezProperty>(4);

        public override T create<T>(ITezValueDescriptor descriptor)
        {
            if (mList.binaryFind(descriptor.ID, out int index))
            {
                return (T)mList[index];
            }
            else
            {
                var property = new T();
                property.descriptor = descriptor;
                mList.insert(index, property);
                return property;
            }
        }

        public override void clear()
        {
            for (int i = 0; i < mList.count; i++)
            {
                mList[i].close();
            }

            mList.clear();
        }

        protected override void onClose()
        {
            this.clear();
            mList = null;
        }

        public override ITezProperty get(int id)
        {
            return mList.binaryFind(id);
        }

        public override ITezProperty get(ITezValueDescriptor vd)
        {
            return mList.binaryFind(vd.ID);
        }

        public override bool remove(ITezValueDescriptor descriptor)
        {
            if (mList.binaryFind(descriptor.ID, out int index))
            {
                mList.removeAt(index);
                return true;
            }

            return false;
        }

        public override bool tryGet(int id, out ITezProperty property)
        {
            if (mList.binaryFind(id, out int index))
            {
                property = mList[id];
                return true;
            }

            property = null;
            return false;
        }
    }
}