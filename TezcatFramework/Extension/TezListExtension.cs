using System;
using System.Collections.Generic;

namespace tezcat.Framework.Extension
{
    public interface ITezBinarySearchItem
    {
        int binaryWeight { get; }
    }

    public static class TezListExtension
    {
        /// <summary>
        /// 把要删除的Item和最后的Item交换后再删除
        /// </summary>
        /// <param name="remove_id">要移除的item的ID</param>
        /// <param name="on_swap">非最后一个Item的删除前处理(remove, last)</param>
        /// <param name="on_not_swap">最后一个Item的删除前处理(remove)</param>
        public static void Remove<T>(this List<T> list, int remove_id, TezEventExtension.Action<T, T> on_swap, TezEventExtension.Action<T> on_not_swap)
        {
            int last_id = list.Count - 1;
            if (remove_id != last_id)
            {
                var last_item = list[last_id];
                var remove_item = list[remove_id];
                on_swap(remove_item, last_item);
                list[remove_id] = last_item;
            }
            else
            {
                on_not_swap(list[remove_id]);
            }

            list.RemoveAt(last_id);
        }

        /// <summary>
        /// 把要删除的Item和最后的Item交换后再删除
        /// </summary>
        /// <param name="remove_id">要移除的物品的ID</param>
        public static void Remove<T>(this List<T> list, int remove_id)
        {
            int last_id = list.Count - 1;
            if (remove_id != last_id)
            {
                var last_item = list[last_id];
                var remove_item = list[remove_id];
                list[remove_id] = last_item;
            }

            list.RemoveAt(last_id);
        }

        public static bool binaryFind<T>(this List<T> list, int target_weight, out T result) where T : ITezBinarySearchItem
        {
            int begin_pos = 0;
            int end_pos = list.Count - 1;

            while(begin_pos <= end_pos)
            {
                var mid_pos = begin_pos + end_pos >> 1;
                var item = list[mid_pos];
                if (item.binaryWeight > target_weight)
                {
                    end_pos = mid_pos - 1;
                }
                else if(item.binaryWeight < target_weight)
                {
                    begin_pos = mid_pos + 1;
                }
                else
                {
                    result = item;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static bool binaryFind<T>(this List<T> list, int target_weight, out int index) where T : ITezBinarySearchItem
        {
            int begin_pos = 0;
            int end_pos = list.Count - 1;

            while (begin_pos <= end_pos)
            {
                var mid_pos = begin_pos + end_pos >> 1;
                var item = list[mid_pos];
                if (item.binaryWeight > target_weight)
                {
                    end_pos = mid_pos - 1;
                }
                else if (item.binaryWeight < target_weight)
                {
                    begin_pos = mid_pos + 1;
                }
                else
                {
                    index = mid_pos;
                    return true;
                }
            }

            index = begin_pos;
            return false;
        }

        public static void addWithBinaryWeight<T>(this List<T> list, T item) where T : ITezBinarySearchItem
        {
            int index = 0;
            list.binaryFind(item.binaryWeight, out index);
            list.Insert(index, item);
        }

        /// <summary>
        /// 随机打乱数组
        /// </summary>
        public static void shuffle<T>(this List<T> list, Func<int, int> random)
        {
            var count = list.Count - 1;
            while (count > -1)
            {
                var index = random(list.Count);
                var value = list[index];
                list[index] = list[count];
                list[count] = value;
                count--;
            }
        }

        public static void shuffle<T>(this List<T> list)
        {
            Random random = new Random();
            
            var count = list.Count - 1;
            while (count > -1)
            {
                var index = random.Next(count);
                var value = list[index];
                list[index] = list[count];
                list[count] = value;
                count--;
            }
        }

        public static T randomGet<T>(this List<T> list, Func<int, int> random)
        {
            var index = random(list.Count);
            var result = list[index];

            var last = list.Count - 1;
            if (index != last)
            {
                list[index] = list[last];
            }

            list.RemoveAt(last);

            return result;
        }
    }
}