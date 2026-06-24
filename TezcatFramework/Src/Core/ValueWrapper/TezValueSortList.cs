using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    public class TezValueSortList<Value>
        : TezList<Value>
        where Value : ITezValueWrapper
    {
        public TezValueSortList(int capacity) : base(capacity)
        {

        }

        public void addValue(Value property)
        {
            if (!this.binaryFind(property.ID, out int index))
            {
                this.insert(index, property);
            }
            else
            {
                throw new System.Exception(string.Format("{0} is existed", property.name));
            }
        }

        public bool removeValue(Value property)
        {
            if (this.binaryFind(property.ID, out int index))
            {
                this.removeAt(index);
                return true;
            }

            return false;
        }

        public Value binaryFind(int property_id)
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

            return default;
        }

        /// <summary>
        /// 查找一个Property
        /// 如果没找到 返回的Index为此Property应该被插入的位置
        /// 如果找到 返回的Index为此Property的位置
        /// </summary>
        public bool binaryFind(int property_id, out int index)
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
