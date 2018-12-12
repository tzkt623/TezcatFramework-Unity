using System.Collections.Generic;
using System.Diagnostics;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public abstract class TezAStarNode : ITezBinaryHeapItem<TezAStarNode>
    {
        public TezAStarNode parent { get; set; }
        public int index { get; set; } = -1;

        /// <summary>
        /// 距离起点的Cost
        /// </summary>
        /// <returns></returns>
        public virtual int gCost { get; set; }

        /// <summary>
        /// 距离终点的Cost
        /// </summary>
        /// <returns></returns>
        public virtual int hCost { get; set; }

        /// <summary>
        /// 所有Cost
        /// </summary>
        public virtual int fCost
        {
            get { return this.gCost + this.hCost; }
        }

        public abstract bool blocked();

        public abstract List<TezAStarNode> getNeighbours();

        public abstract int getCostFrom(TezAStarNode other);

        public abstract int CompareTo(TezAStarNode other);

        public abstract bool sameAs(TezAStarNode other);
    }

    public class TezAStar
    {
        public event TezEventExtension.Action<List<TezAStarNode>> onPathFound;
        public event TezEventExtension.Action onPathNotFound;

        public void findPath(TezAStarNode start, TezAStarNode end, TezBinaryHeap<TezAStarNode> open_set)
        {
//             Stopwatch stopwatch = new Stopwatch();
//             stopwatch.Start(
            if (end.blocked())
            {
                return;
            }

            HashSet<TezAStarNode> closeSet = new HashSet<TezAStarNode>();
            open_set.push(start);

            while (open_set.count > 0)
            {
                var currentNode = open_set.pop();
                if (currentNode == end)
                {
//                     stopwatch.Stop();
//                     UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds + "ms");
                    this.retracePath(start, end);
                    return;
                }

                closeSet.Add(currentNode);

                var neighbours = currentNode.getNeighbours();
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.blocked() || closeSet.Contains(neighbour))
                    {
                        continue;
                    }

                    var inOpen = open_set.contains(neighbour);
                    int newCost = currentNode.gCost + currentNode.getCostFrom(neighbour);
                    if (newCost < neighbour.gCost || !inOpen)
                    {
                        neighbour.gCost = newCost;
                        neighbour.hCost = neighbour.getCostFrom(end);

                        neighbour.parent = currentNode;

                        if (!inOpen)
                        {
                            open_set.push(neighbour);
                        }
                    }
                }
            }

            onPathNotFound?.Invoke();
        }

        public void findPath(TezAStarNode start, TezAStarNode end)
        {
//             Stopwatch stopwatch = new Stopwatch();
//             stopwatch.Start();

            if (end.blocked())
            {
                return;
            }

            List<TezAStarNode> openSet = new List<TezAStarNode>();
            HashSet<TezAStarNode> closeSet = new HashSet<TezAStarNode>();

            openSet.Add(start);
            int removeIndex = 0;

            while (openSet.Count > 0)
            {
                ///找到当前列表中Cost最小的路径点
                removeIndex = 0;
                var currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    var temp = openSet[i];
                    if (temp.fCost < currentNode.fCost
                        || (temp.fCost == currentNode.fCost && temp.hCost < currentNode.hCost))
                    {
                        currentNode = temp;
                        removeIndex = i;
                    }
                }

                ///如果等于结束点,则找到完整路径
                if (currentNode == end)
                {
//                     stopwatch.Stop();
//                     UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds + "ms");
                    this.retracePath(start, end);
                    return;
                }

                ///从开列表中移除当前选择的路径点
                openSet.RemoveAt(removeIndex);
                ///加入关列表
                closeSet.Add(currentNode);

                ///获取当前路径点的邻居
                ///计算邻居Cost值
                ///并且加入开列表中
                var neighbours = currentNode.getNeighbours();
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.blocked() || closeSet.Contains(neighbour))
                    {
                        continue;
                    }

                    var inOpen = openSet.Find((TezAStarNode node) =>
                    {
                        return node.sameAs(neighbour);
                    }) != null;

                    int newCost = currentNode.gCost + currentNode.getCostFrom(neighbour);
                    if (newCost < neighbour.gCost || !inOpen)
                    {
                        if (!inOpen)
                        {
                            openSet.Add(neighbour);
                        }

                        neighbour.gCost = newCost;
                        neighbour.hCost = neighbour.getCostFrom(end);

                        neighbour.parent = currentNode;
                    }
                }
            }

            onPathNotFound?.Invoke();
        }

        public void retracePath(TezAStarNode start, TezAStarNode end)
        {
            List<TezAStarNode> path = new List<TezAStarNode>();
            TezAStarNode current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }

            onPathFound?.Invoke(path);
        }
    }
}

