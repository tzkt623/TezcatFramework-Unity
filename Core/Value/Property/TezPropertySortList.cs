using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    public class TezPropertySortList : TezList<ITezProperty>
    {
        public TezPropertySortList() : base(4)
        {

        }

        public void addProperty(ITezProperty property)
        {
            int index = 0;
            if (!this.binarySearch(property.ID, out index))
            {
                this.insert(index, property);
            }
        }

        public bool removeProperty(ITezProperty property)
        {
            int index = 0;
            if (this.binarySearch(property.ID, out index))
            {
                this.removeAt(index);
                return true;
            }

            return false;
        }

        public ITezProperty binaryFind(ITezValueDescriptor descriptor)
        {
            return this.binaryFind(descriptor.ID);
        }

        public ITezProperty binaryFind(int property_id)
        {
            int begin_pos = 0;
            int end_pos = this.count - 1;
            while (begin_pos <= end_pos)
            {
                int mid_pos = begin_pos + end_pos >> 1;
                int current_id = m_Data[mid_pos].ID;
                if (current_id < property_id)
                {
                    begin_pos = mid_pos + 1;
                }
                else
                {
                    if (current_id == property_id)
                    {
                        return m_Data[mid_pos];
                    }
                    end_pos = mid_pos - 1;
                }
            }
            return null;
        }

        public bool binarySearch(ITezValueDescriptor descriptor, out int index)
        {
            return this.binarySearch(descriptor.ID, out index);
        }

        public bool binarySearch(int property_id, out int index)
        {
            int begin_pos = 0;
            int end_pos = this.count - 1;
            while (begin_pos <= end_pos)
            {
                int mid_pos = begin_pos + end_pos >> 1;
                int current_id = m_Data[mid_pos].ID;
                ///当前权重小于查询权重
                if (current_id < property_id)
                {
                    ///查找右半部分
                    begin_pos = mid_pos + 1;
                }
                ///当前权重大于等于查询权重
                else
                {
                    if (current_id == property_id)
                    {
                        index = mid_pos;
                        return true;
                    }
                    ///查找左半部分
                    end_pos = mid_pos - 1;
                }
            }

            index = begin_pos;
            return false;
        }
    }
}