﻿using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// AStar Path Finder
    /// </summary>
    /// <typeparam name="Wrapper">路径包装器</typeparam>
    /// <typeparam name="BlockData">包装器中的实际路块数据</typeparam>
    public abstract class TezAStarSystem<Wrapper, BlockData>
        : ITezCloseable
        where Wrapper : TezAStarDataWrapper<Wrapper, BlockData>, new()
    {
        #region Tool
        public static void closePools()
        {
            TezObjectPool.destroy<Wrapper>();
            TezObjectPool.destroy<TezObjectPool.HashSet<Wrapper>>();
            TezObjectPool.destroy<TezObjectPool.List<BlockData>>();
            TezObjectPool.destroy<TezObjectPool.List<Wrapper>>();
        }

        protected static TezObjectPool.List<BlockData> createBlockDataList()
        {
            return TezObjectPool.create<TezObjectPool.List<BlockData>>();
        }

//         protected static void recycleBlockDataList(TezObjectPool.List<BlockData> blockDatas)
//         {
//             TezObjectPool.recycle(blockDatas);
//         }

        protected static Wrapper createWrapper()
        {
            return TezObjectPool.create<Wrapper>();
        }

//         protected static void recycleWrapper(Wrapper wrapper)
//         {
//             TezObjectPool.recycle(wrapper);
//         }

        protected static TezObjectPool.List<Wrapper> createWrapperList()
        {
            return TezObjectPool.create<TezObjectPool.List<Wrapper>>();
        }

//         protected static void recycleWrapperList(TezObjectPool.List<Wrapper> wrappers)
//         {
//             TezObjectPool.recycle(wrappers);
//         }

        protected static TezObjectPool.HashSet<Wrapper> createWapperHashSet()
        {
            return TezObjectPool.create<TezObjectPool.HashSet<Wrapper>>();
        }

//         protected static void recycleWrapperHashSet(TezObjectPool.HashSet<Wrapper> wrappers)
//         {
//             TezObjectPool.recycle(wrappers);
//         }
        #endregion

        Dictionary<BlockData, Wrapper> mSaveWrappers = new Dictionary<BlockData, Wrapper>();

        /// <summary>
        /// 路径找到
        /// </summary>
        public event TezEventExtension.Action<List<BlockData>> evtPathFound;
        /// <summary>
        /// 路径没找到
        /// </summary>
        public event TezEventExtension.Action evtPathNotFound;

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            evtPathFound = null;
            evtPathNotFound = null;

            mSaveWrappers.Clear();
            mSaveWrappers = null;
        }
        /// <summary>
        /// 使用二叉堆加速型的find
        /// </summary>
        public void findPath(Wrapper start, Wrapper end, TezBinaryHeap<Wrapper> openSet)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            this.saveWrapper(start);
            this.saveWrapper(end);

            if (end.isBlocked())
            {
                this.onPathFindComplete();
                evtPathNotFound.Invoke();
                return;
            }

            var close_set = createWapperHashSet();
            openSet.push(start);

            while (openSet.count > 0)
            {
                var current_node = openSet.pop();
                if (current_node.Equals(end))
                {
                    //stopwatch.Stop();
                    //Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");
                    this.retracePath(start, current_node);
                    this.onPathFindComplete(openSet, close_set);
                    return;
                }

                close_set.Add(current_node);

                var neighbours = this.calculateNeighbours(current_node);
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.isBlocked() || close_set.Contains(neighbour))
                    {
                        continue;
                    }

                    var not_in_open = !openSet.contains(neighbour);
                    int new_cost = current_node.gCost + this.calculateGPreCost(current_node, neighbour);
                    if (new_cost < neighbour.gCost || not_in_open)
                    {
                        neighbour.gCost = new_cost;
                        neighbour.parent = current_node;
                        neighbour.hCost = this.calculateHCost(neighbour, end);

                        if (not_in_open)
                        {
                            openSet.push(neighbour);
                        }
                    }
                }

                neighbours.poolRecycle();

                //this.recycleNeighbourList(neighbours);
            }

            evtPathNotFound.Invoke();
            this.onPathFindComplete(openSet, close_set);
        }

//         private void recycleNeighbourList(TezObjectPool.List<Wrapper> neighbours)
//         {
//             neighbours.Clear();
//             recycleWrapperList(neighbours);
//         }

        public void retracePath(Wrapper start, Wrapper end)
        {
            List<BlockData> path = new List<BlockData>();
            Wrapper current = end;

            while (!current.Equals(start))
            {
                path.Add(current.blockData);
                current = (Wrapper)current.parent;
            }
            //path.Reverse();

            evtPathFound.Invoke(path);
        }
        /// <summary>
        /// 计算G的前置代价
        /// 为当前块到邻居块的移动代价
        /// </summary>
        protected abstract int calculateGPreCost(Wrapper current, Wrapper neighbour);

        /// <summary>
        /// 计算HCost
        /// 无视阻挡到终点的最近距离
        /// </summary>
        protected abstract int calculateHCost(Wrapper neighbour, Wrapper end);

        /// <summary>
        /// 计算当前块的邻居
        /// </summary>
        private TezObjectPool.List<Wrapper> calculateNeighbours(Wrapper current)
        {
            TezObjectPool.List<Wrapper> wrappers = createWrapperList();
            this.calculateNeighbours(wrappers, current);
            return wrappers;
        }

        protected abstract void calculateNeighbours(TezObjectPool.List<Wrapper> wrappers, Wrapper blockData);

        protected Wrapper getOrCreateWrapper(BlockData blockData)
        {
            if (mSaveWrappers.TryGetValue(blockData, out Wrapper neighbor))
            {
                if (!blockData.Equals(neighbor.blockData))
                {
                    throw new Exception("BlockData Must The Same");
                }
            }
            else
            {
                neighbor = createWrapper();
                neighbor.blockData = blockData;
                this.saveWrapper(neighbor);
            }

            return neighbor;
        }


        /// <summary>
        /// 所有生成的块全都会被清理
        /// </summary>
        protected virtual void onPathFindComplete()
        {
            foreach (var pair in mSaveWrappers)
            {
                pair.Value.poolRecycle();
                //recycleWrapper(pair.Value);
            }
            mSaveWrappers.Clear();
        }

        protected virtual void onPathFindComplete(TezBinaryHeap<Wrapper> openSet, TezObjectPool.HashSet<Wrapper> closeSet)
        {
            this.onPathFindComplete();

            openSet.clear();
            //closeSet.Clear();
            closeSet.poolRecycle();
            //recycleWrapperHashSet(closeSet);
        }

        private void saveWrapper(Wrapper wrapper)
        {
            mSaveWrappers.Add(wrapper.blockData, wrapper);
        }

        /// <summary>
        /// 普通find
        /// </summary>
        [Obsolete("Do not use this method!!! use the other one that with BinaryHeap")]
        private void findPath(Wrapper start, Wrapper end)
        {
            //             Stopwatch stopwatch = new Stopwatch();
            //             stopwatch.Start();

            if (end.isBlocked())
            {
                evtPathNotFound.Invoke();
                return;
            }

            List<Wrapper> open_set = new List<Wrapper>();
            var close_set = createWapperHashSet();

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
                    //recycleWrapperHashSet(close_set);
                    close_set.poolRecycle();
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

                    var not_in_open = open_set.Find((Wrapper node) =>
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

            close_set.poolRecycle();
            //recycleWrapperHashSet(close_set);
            evtPathNotFound.Invoke();
        }
    }
}
