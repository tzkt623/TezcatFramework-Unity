using System.Collections.Generic;

namespace tezcat.Utility
{
    public static class TezListEx
    {
        /// <summary>
        /// 把要删除的Item和最后的Item交换后再删除
        /// </summary>
        /// <param name="remove_id">要移除的item的ID</param>
        /// <param name="on_swap">交换后的处理(remove, last)</param>
        public static void Remove<T>(this List<T> list, int remove_id, TezEventBus.Action<T, T> on_swap)
        {
            int last_id = list.Count - 1;
            if (remove_id != last_id)
            {
                var last_item = list[last_id];
                var remove_item = list[remove_id];
                on_swap(remove_item, last_item);
                list[remove_id] = last_item;
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
    }
}