using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public abstract class TezAStarNode<Self>
        : ITezBinaryHeapItem<Self>
        , IEquatable<Self>
        where Self : TezAStarNode<Self>
    {
        public Self parent { get; set; }
        public int index { get; set; } = -1;

        /// <summary>
        /// 移动的代价
        /// 上一个点的代价+上一个点到此点的代价
        /// 等于此点的gCost
        /// </summary>
        public virtual int gCost { get; set; }

        /// <summary>
        /// 距离终点无视任何阻挡的最短路径
        /// </summary>
        public virtual int hCost { get; set; }

        /// <summary>
        /// 所有Cost
        /// </summary>
        public virtual int fCost
        {
            get { return this.gCost + this.hCost; }
        }

        public abstract bool blocked();

        public abstract List<Self> getNeighbours();

        public abstract int getCostFrom(Self other);

        public abstract int CompareTo(Self other);

        public abstract bool sameAs(Self other);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public abstract bool Equals(Self other);
    }

    public class TezAStar<Node> where Node : TezAStarNode<Node>
    {
        #region Pool
        static Queue<HashSet<Node>> Pool = new Queue<HashSet<Node>>();
        #endregion

        public event TezEventExtension.Action<List<Node>> onPathFound;
        public event TezEventExtension.Action onPathNotFound;

        public void findPath(Node start, Node end, TezBinaryHeap<Node> open_set)
        {

//             Stopwatch stopwatch = new Stopwatch();
//             stopwatch.Start();
            if (end.blocked())
            {
                onPathNotFound?.Invoke();
                return;
            }

            HashSet<Node> closeSet = new HashSet<Node>();
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

        public void findPath(Node start, Node end)
        {
//             Stopwatch stopwatch = new Stopwatch();
//             stopwatch.Start();

            if (end.blocked())
            {
                onPathNotFound?.Invoke();
                return;
            }

            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();

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

                    var inOpen = openSet.Find((Node node) =>
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

        public void retracePath(Node start, Node end)
        {
            List<Node> path = new List<Node>();
            Node current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }

            onPathFound?.Invoke(path);
        }
    }
}

