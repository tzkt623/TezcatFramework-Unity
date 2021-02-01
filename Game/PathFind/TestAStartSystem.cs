using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class AllBlock
    {
        public static readonly AllBlock instance = new AllBlock();

        Block[] m_AllBlock;

        public void getNeighbors(TezHexCubeCoordinate coordinate, Action<Block> action)
        {
            foreach (var item in m_AllBlock)
            {
                /*伪码
                    if(item is coordinate`s Neighbors)
                    {
                        action(item);
                    }
                */
            }
        }
    }

    public class Block : TezHexBlock
    {

    }

    public class PathWrapper : TezAStarDataWrapper<PathWrapper, Block>
    {
        public override bool isBlocked()
        {
            return this.blockData == null;
        }
    }

    public class PathFinder : TezAStarSystem<PathWrapper, Block>
    {
        TezBinaryHeap<PathWrapper> m_Heap = new TezBinaryHeap<PathWrapper>();

        public void find(Block start, Block end)
        {
            m_Heap.clear();
            var start_wrapper = createWrapper();
            start_wrapper.blockData = start;

            var end_wrapper = createWrapper();
            end_wrapper.blockData = end;

            this.findPath(start_wrapper, end_wrapper, m_Heap);
            /*
            Debug.Log(string.Format("Total:[{0}] [{1}] [{2}] [{3}]", TezSamplePool<PathWrapper>.instance.createCount, TezSamplePool<HashSet<PathWrapper>>.instance.createCount, TezSamplePool<List<Block>>.instance.createCount, TezSamplePool<List<PathWrapper>>.instance.createCount));
            Debug.Log(string.Format("Recycle:[{0}] [{1}] [{2}] [{3}]", TezSamplePool<PathWrapper>.instance.recycleCount, TezSamplePool<HashSet<PathWrapper>>.instance.recycleCount, TezSamplePool<List<Block>>.instance.recycleCount, TezSamplePool<List<PathWrapper>>.instance.recycleCount));
            */
        }

        protected override int calculateGPreCost(PathWrapper current, PathWrapper neighbour)
        {
            return current.blockData.coordinate.getDistanceFrom(neighbour.blockData.coordinate);
        }

        protected override int calculateHCost(PathWrapper neighbour, PathWrapper end)
        {
            return end.blockData.coordinate.getDistanceFrom(neighbour.blockData.coordinate);
        }

        protected override void calculateNeighbours(List<PathWrapper> wrappers, PathWrapper wrapper)
        {
            AllBlock.instance.getNeighbors(wrapper.blockData.coordinate, (Block block) =>
            {
                if (block != null)
                {
                    wrappers.Add(this.getOrCreateWrapper(block));
                }
            });
        }
    }
}
