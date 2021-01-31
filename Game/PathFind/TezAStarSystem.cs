using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public abstract class TezAStarSystem<Node> where Node : TezAStarNode<Node>, new()
    {
        #region Pool
        public static TezEventExtension.Action<HashSet<Node>> onCloseRecycle;

        static Queue<HashSet<Node>> s_CloseListPool = new Queue<HashSet<Node>>();
        static HashSet<Node> createCloseList()
        {
            if (s_CloseListPool.Count > 0)
            {
                return s_CloseListPool.Dequeue();
            }

            return new HashSet<Node>();
        }

        static void recycleCloseList(HashSet<Node> list)
        {
            foreach (var item in list)
            {
                item.close();
            }
            list.Clear();
            s_CloseListPool.Enqueue(list);
        }
        #endregion

        /// <summary>
        /// 路径找到
        /// </summary>
        public event TezEventExtension.Action<List<Node>> onPathFound;
        /// <summary>
        /// 路径没找到
        /// </summary>
        public event TezEventExtension.Action onPathNotFound;

        /// <summary>
        /// 计算G的前置代价
        /// 为当前块到邻居块的移动代价
        /// </summary>
        protected abstract int calculateGPreCost(Node current, Node neighbour);

        /// <summary>
        /// 计算HCost
        /// 无视阻挡到终点的最近距离
        /// </summary>
        protected abstract int calculateHCost(Node neighbour, Node end);

        /// <summary>
        /// 计算当前块的邻居
        /// </summary>
        protected abstract ICollection<Node> calculateNeighbours(Node current);

        /// <summary>
        /// 使用二叉堆加速型的find
        /// </summary>
        public void findPath(Node start, Node end, TezBinaryHeap<Node> openSet)
        {

            //             Stopwatch stopwatch = new Stopwatch();
            //             stopwatch.Start();
            if (end.isBlocked())
            {
                start.close();
                end.close();
                onPathNotFound.Invoke();
                return;
            }

            HashSet<Node> close_set = createCloseList();
            openSet.push(start);

            while (openSet.count > 0)
            {
                var current_node = openSet.pop();
                if (current_node.Equals(end))
                {
                    //                     stopwatch.Stop();
                    //                     UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds + "ms");
                    foreach (var item in openSet)
                    {
                        item.close();
                    }
                    recycleCloseList(close_set);
                    this.retracePath(start, current_node);
                    return;
                }

                close_set.Add(current_node);

                var neighbours = this.calculateNeighbours(current_node);
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.isBlocked() || close_set.Contains(neighbour))
                    {
                        neighbour.close();
                        continue;
                    }

                    var not_in_open = !openSet.contains(neighbour);
                    int new_cost = current_node.gCost + this.calculateGPreCost(current_node, neighbour);
                    if (new_cost < neighbour.gCost || not_in_open)
                    {
                        neighbour.gCost = new_cost;
                        neighbour.hCost = this.calculateHCost(neighbour, end);
                        neighbour.parent = current_node;

                        if (not_in_open)
                        {
                            openSet.push(neighbour);
                        }
                    }
                }
            }

            recycleCloseList(close_set);
            onPathNotFound.Invoke();
        }

        /// <summary>
        /// 普通find
        /// </summary>
        public void findPath(Node start, Node end)
        {
            //             Stopwatch stopwatch = new Stopwatch();
            //             stopwatch.Start();

            if (end.isBlocked())
            {
                onPathNotFound.Invoke();
                return;
            }

            List<Node> open_set = new List<Node>();
            HashSet<Node> close_set = createCloseList();

            open_set.Add(start);
            int remove_index = 0;

            while (open_set.Count > 0)
            {
                ///找到当前列表中Cost最小的路径点
                remove_index = 0;
                var current_node = open_set[0];
                for (int i = 1; i < open_set.Count; i++)
                {
                    var temp = open_set[i];
                    if (temp.fCost < current_node.fCost
                        || (temp.fCost == current_node.fCost && temp.hCost < current_node.hCost))
                    {
                        current_node = temp;
                        remove_index = i;
                    }
                }

                ///如果等于结束点,则找到完整路径
                if (current_node.Equals(end))
                {
                    //                     stopwatch.Stop();
                    //                     UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds + "ms");
                    recycleCloseList(close_set);
                    this.retracePath(start, current_node);
                    return;
                }

                ///从开列表中移除当前选择的路径点
                open_set.RemoveAt(remove_index);
                ///加入关列表
                close_set.Add(current_node);

                ///获取当前路径点的邻居
                ///计算邻居Cost值
                ///并且加入开列表中
                var neighbours = this.calculateNeighbours(current_node);
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.isBlocked() || close_set.Contains(neighbour))
                    {
                        continue;
                    }

                    var not_in_open = open_set.Find((Node node) =>
                    {
                        return node.Equals(neighbour);
                    }) == null;

                    int newCost = current_node.gCost + this.calculateGPreCost(current_node, neighbour);
                    if (newCost < neighbour.gCost || not_in_open)
                    {
                        neighbour.gCost = newCost;
                        neighbour.hCost = this.calculateHCost(neighbour, end);
                        neighbour.parent = current_node;

                        if (not_in_open)
                        {
                            open_set.Add(neighbour);
                        }
                    }
                }
            }

            recycleCloseList(close_set);
            onPathNotFound.Invoke();
        }

        public void retracePath(Node start, Node end)
        {
            List<Node> path = new List<Node>();
            Node current = end;

            while (!current.Equals(start))
            {
                path.Add(current);
                current = (Node)current.parent;
            }
            path.Reverse();

            onPathFound.Invoke(path);
        }
    }
}

