using System.Collections.Generic;
using tezcat.Signal;

namespace tezcat.Utility
{
    public interface ITezListSortItem
    {
        int sortID { get; }
    }

    public static class TezListExtension
    {
        /// <summary>
        /// 把要删除的Item和最后的Item交换后再删除
        /// </summary>
        /// <param name="remove_id">要移除的item的ID</param>
        /// <param name="on_swap">非最后一个Item的删除前处理(remove, last)</param>
        /// <param name="on_not_swap">最后一个Item的删除前处理(remove)</param>
        public static void Remove<T>(this List<T> list, int remove_id, TezEventCenter.Action<T, T> on_swap, TezEventCenter.Action<T> on_not_swap)
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

        public static bool binaryFind<T>(this List<T> list, int target_id, out T result) where T : ITezListSortItem
        {
            int min = 0;
            int max = list.Count;
            int mid = -1;

            while(min <= max)
            {
                mid = min + (max - min) / 2;
                var item = list[mid];
                if (item.sortID > target_id)
                {
                    max = mid - 1;
                }
                else if(item.sortID < target_id)
                {
                    min = mid + 1;
                }
                else
                {
                    result = item;
                    return true;
                }
            }

            result = default(T);
            return false;
        }
    }
}