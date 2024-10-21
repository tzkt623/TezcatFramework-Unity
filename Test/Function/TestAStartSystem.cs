using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class Map
    {
        Block[,] mAllBlock;
        public Block[,] blocks => mAllBlock;

        public void init(int width, int hegiht)
        {
            var random = new TezRandom();
            mAllBlock = new Block[width, hegiht];

            for (int y = 0; y < hegiht; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mAllBlock[x, y] = new Block(x, y, random.nextInt(-1, 10));
                }
            }
        }

        public void getNeighbors(Block center, Action<Block> action)
        {
            for (int cy = -1; cy <= 1; cy++)
            {
                for (int cx = -1; cx <= 1; cx++)
                {
                    if (cx == 0 && cy == 0)
                    {
                        continue;
                    }

                    int pos_x = center.x + cx;
                    int pos_y = center.y + cy;

                    if (pos_x >= mAllBlock.GetLength(0) || pos_x < 0
                        || pos_y >= mAllBlock.GetLength(1) || pos_y < 0)
                    {
                        continue;
                    }

                    action(mAllBlock[pos_x, pos_y]);
                }
            }
        }

        public void close()
        {
            mAllBlock = null;
        }
    }

    public class Block
    {
        /// <summary>
        /// block = -1;
        /// </summary>
        public int costRate;

        public int x, y;

        public Block(int x, int y, int cost)
        {
            this.x = x;
            this.y = y;
            this.costRate = cost;
        }

        public int getDistanceFrom(Block other)
        {
            return (int)Math.Sqrt(Math.Pow(this.x - (double)other.x, 2.0) + Math.Pow(this.y - (double)other.y, 2.0)) * 10;
        }

        public string draw()
        {
            switch (costRate)
            {
                case -1:
                    return "x";
                default:
                    return this.costRate.ToString();
            }
        }
    }

    public class PathWrapper : TezAStarDataWrapper<PathWrapper, Block>
    {
        public override bool isBlocked()
        {
            return this.blockData.costRate == -1;
        }
    }

    public class PathFinder : TezAStarSystem<PathWrapper, Block>
    {
        public Map map { get; set; }
        TezBinaryHeap<PathWrapper> mHeap = new TezBinaryHeap<PathWrapper>();

        protected override void onClose()
        {
            base.onClose();
            mHeap.close();
            mHeap = null;
            this.map = null;
        }

        public void find(Block start, Block end)
        {
            mHeap.clear();
            var start_wrapper = createWrapper();
            start_wrapper.blockData = start;

            var end_wrapper = createWrapper();
            end_wrapper.blockData = end;

            this.findPath(start_wrapper, end_wrapper, mHeap);
        }

        protected override int calculateGPreCost(PathWrapper current, PathWrapper neighbour)
        {
            return current.blockData.getDistanceFrom(neighbour.blockData);
        }

        protected override int calculateHCost(PathWrapper neighbour, PathWrapper end)
        {
            return end.blockData.getDistanceFrom(neighbour.blockData) * neighbour.blockData.costRate;
        }

        protected override void calculateNeighbours(TezObjectPool.List<PathWrapper> wrappers, PathWrapper wrapper)
        {
            this.map.getNeighbors(wrapper.blockData, (Block block) =>
            {
                if (block != null)
                {
                    wrappers.Add(this.getOrCreateWrapper(block));
                }
            });
        }
    }


    public class TestAStarSystem : TezBaseTest
    {
        Map mMap = null;
        PathFinder mPathFinder = null;

        public TestAStarSystem() : base("AStar Path Finder")
        {

        }

        protected override void onClose()
        {
            mPathFinder.close();
            mMap.close();

            mMap = null;
            mPathFinder = null;
        }

        public override void init()
        {
            mMap = new Map();
            mMap.init(10, 10);

            mPathFinder = new PathFinder()
            {
                map = mMap
            };
            mPathFinder.evtPathFound += onPathFound;
            mPathFinder.evtPathNotFound += onPathNotFound;
        }

        private void onPathNotFound()
        {
            Console.WriteLine("Path Not Found!!!");
        }

        private void onPathFound(List<Block> list)
        {
            for (int y = -1; y < mMap.blocks.GetLength(1); y++)
            {
                for (int x = -1; x < mMap.blocks.GetLength(0); x++)
                {
                    if (x == -1 && y == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else if (x == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(y);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else if (y == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(x);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        var point = list.Find((Block waypoint) =>
                        {
                            return waypoint.x == x && waypoint.y == y;
                        });

                        if (point != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(mMap.blocks[x, y].draw());
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            Console.Write(mMap.blocks[x, y].draw());
                        }
                    }

                    Console.Write(" ");
                }
                Console.WriteLine("");
            }
        }

        public override void run()
        {
            for (int y = -1; y < mMap.blocks.GetLength(1); y++)
            {
                for (int x = -1; x < mMap.blocks.GetLength(0); x++)
                {
                    if (x == -1 && y == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else if (x == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(y);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else if (y == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(x);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.Write(mMap.blocks[x, y].draw());
                    }

                    Console.Write(" ");
                }
                Console.WriteLine("");
            }

            Console.WriteLine("Input Begin Pos >> ");
            Console.Write("X:");
            int bx = int.Parse(Console.ReadLine());
            Console.Write("Y:");
            int by = int.Parse(Console.ReadLine());

            Console.WriteLine("Input End Pos >> ");
            Console.Write("X:");
            int ex = int.Parse(Console.ReadLine());
            Console.Write("Y:");
            int ey = int.Parse(Console.ReadLine());

            mPathFinder.find(mMap.blocks[bx, by], mMap.blocks[ex, ey]);
        }
    }
}