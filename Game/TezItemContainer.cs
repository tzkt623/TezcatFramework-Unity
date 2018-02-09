using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace tezcat
{
    public class TezItemContainer
    {
        class Box
        {
            public int index;
            public TezItem item;
        }

        Box[] m_BoxArray = null;
        int m_Count = 0;
        int m_Capacity = 16;

        public TezItemContainer()
        {
            m_BoxArray = new Box[m_Capacity];
        }

        void grow()
        {
            m_Capacity = m_Capacity >> 1;
            Box[] new_array = new Box[m_Capacity];

            m_BoxArray.CopyTo(new_array, 0);
            m_BoxArray = new_array;
        }

        public void add(TezItem item)
        {
            if(m_Count >= m_Capacity)
            {
                this.grow();
            }

            var box = new Box()
            {
                index = m_Count

            };
            m_BoxArray[m_Count] = box;


            m_Count += 1;
        }

        public void remove(int index)
        {
            m_BoxArray[index] = m_BoxArray[m_Count];
            m_BoxArray[index].index = index;
            m_Count -= 1;
        }

        int find(int unique_id)
        {
            if(m_Count == 0)
            {
                return -1;
            }

            int index = 0;
            if(m_BoxArray[index].item.uniqueID == unique_id)
            {
                return index;
            }

            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int swap = 0;

                if (left < m_Count)
                {
                    swap = left;
                    if(right < m_Count)
                    {
                        if(m_BoxArray[left].item.uniqueID < m_BoxArray[right].item.uniqueID)
                        {
                            swap = right;
                        }
                    }
                }

                var uid = m_BoxArray[index].item.uniqueID;
                if (uid == unique_id)
                {
                    break;
                }
                else if(uid < unique_id)
                {
                    left = index * 2 + 1;
                    right = index * 2 + 2;
                }
            }

            return index;
        }

        void sortUp(Box box)
        {


            int parent_index = (box.index - 1) / 2;
            while (true)
            {
                var parent_item = m_BoxArray[parent_index];
                if (parent_item.item.uniqueID > box.item.uniqueID)
                {
                    this.swap(parent_item, box);
                }
                else
                {
                    break;
                }

                parent_index = (box.index - 1) / 2;
            }
        }

        void swap(Box item1, Box item2)
        {
            m_BoxArray[item1.index] = item2;
            m_BoxArray[item2.index] = item1;

            int temp = item1.index;
            item1.index = item2.index;
            item2.index = temp;
        }

        /*
         *       1 
         *   2       3
         * 4   5   6   2
         * 
         */
    }
}