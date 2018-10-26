using System.Collections.Generic;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public class TezAStar
    {
        public abstract class Node : ITezBinaryHeapItem<Node>
        {
            public Node parent { get; set; }
            public int index { get; set; }

            /// <summary>
            /// 距离起点的Cost
            /// </summary>
            /// <returns></returns>
            public abstract int getGCost();
            public abstract void setGCost(int cost);

            /// <summary>
            /// 距离终点的Cost
            /// </summary>
            /// <returns></returns>
            public abstract int getHCost();
            public abstract void setHCost(int cost);

            /// <summary>
            /// 所有Cost
            /// </summary>
            /// <returns></returns>
            public virtual int getFCost()
            {
                return getGCost() + getHCost();
            }

            public abstract bool blocked();

            public abstract int weight();

            public abstract List<Node> getNeighbours();

            public abstract int getDistanceFrom(Node other);

            public abstract int CompareTo(Node other);
        }

        public event System.Action<List<Node>> onPathFinded;

        public void findPath(Node start, Node end, TezBinaryHeap<Node> open_set)
        {
            //             Stopwatch stopwatch = new Stopwatch();
            //             stopwatch.Start();

            if(end.blocked())
            {
                return;
            }

            HashSet<Node> closeSet = new HashSet<Node>();
            open_set.push(start);

            while (open_set.count > 0)
            {
                var currentNode = open_set.popFirst();
                if (currentNode == end)
                {
                    //                     stopwatch.Stop();
                    //                     UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
                    this.retracePath(start, end);
                    break;
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
                    int newCost = currentNode.getGCost() + currentNode.getDistanceFrom(neighbour);
                    if (newCost < neighbour.getGCost() || !inOpen)
                    {
                        neighbour.setGCost(newCost);
                        neighbour.setHCost(neighbour.getDistanceFrom(end));

                        neighbour.parent = currentNode;

                        if (!inOpen)
                        {
                            open_set.push(neighbour);
                        }
                    }
                }
            }
        }

        public void findPath(Node start, Node end)
        {
            //             Stopwatch stopwatch = new Stopwatch();
            //             stopwatch.Start();

            if(end.blocked())
            {
                return;
            }

            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].getFCost() < currentNode.getFCost() ||
                        (openSet[i].getFCost() == currentNode.getFCost() && openSet[i].getHCost() < currentNode.getHCost()))
                    {
                        currentNode = openSet[i];
                    }
                }

                if (currentNode == end)
                {
                    //                    stopwatch.Stop();
                    //                    UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds + "ms");
                    this.retracePath(start, end);
                    break;
                }

                openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                var neighbours = currentNode.getNeighbours();
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.blocked() || closeSet.Contains(neighbour))
                    {
                        continue;
                    }

                    var inOpen = openSet.Contains(neighbour);
                    int newCost = currentNode.getGCost() + currentNode.getDistanceFrom(neighbour);
                    if (newCost < neighbour.getGCost() || !inOpen)
                    {
                        neighbour.setGCost(newCost);
                        neighbour.setHCost(neighbour.getDistanceFrom(end));

                        neighbour.parent = currentNode;

                        if (!inOpen)
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
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

            if(onPathFinded != null)
            {
                onPathFinded(path);
            }
        }
    }
}

