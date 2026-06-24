using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezDungeonGenerator
    {
        public event TezEventExtension.Action<int, int> onBlockFilled;
        public event TezEventExtension.Action<int, int> onCanPassCreated;
        public event TezEventExtension.Action<int, int> onCanNotPassCreated;
        public event TezEventExtension.Action<int, int> onCanPassConfirmed;

        List<TezDungeonRoom> m_RoomList = new List<TezDungeonRoom>();

        TezDungeonBlock[,] m_BlockArray = null;
        TezRandom m_Random = new TezRandom();

        int m_BlockWidth;
        int m_BlockHeight;

        int m_Width = 0;
        int m_Height = 0;

        public Vector2 getPosition(int x, int y)
        {
            return new Vector2(x * m_BlockWidth, y * m_BlockHeight);
        }

        public void init(int block_width, int block_height, int width, int height)
        {
            m_BlockWidth = block_width;
            m_BlockHeight = block_height;

            m_Width = width;
            m_Height = height;

            m_BlockArray = new TezDungeonBlock[m_Width, m_Height];
        }

        public TezDungeonBlock getBlock(TezDungeonUtility.Direction dir, TezDungeonBlock current)
        {
            int x, y;
            if (this.getBlock(dir, current.x, current.y, out x, out y))
            {
                return m_BlockArray[x, y];
            }

            return null;
        }

        public bool getBlock(TezDungeonUtility.Direction dir, int x, int y, out int result_x, out int result_y)
        {
            result_x = x;
            result_y = y;

            switch (dir)
            {
                case TezDungeonUtility.Direction.N:
                    result_y += 1;
                    break;
                case TezDungeonUtility.Direction.S:
                    result_y -= 1;
                    break;
                case TezDungeonUtility.Direction.W:
                    result_x -= 1;
                    break;
                case TezDungeonUtility.Direction.E:
                    result_x += 1;
                    break;
                case TezDungeonUtility.Direction.WN:
                    result_x -= 1;
                    result_y += 1;
                    break;
                case TezDungeonUtility.Direction.EN:
                    result_x += 1;
                    result_y += 1;
                    break;
                case TezDungeonUtility.Direction.WS:
                    result_x -= 1;
                    result_y -= 1;
                    break;
                case TezDungeonUtility.Direction.ES:
                    result_x += 1;
                    result_y -= 1;
                    break;
            }

            return (result_x >= 0 && result_x < m_Width && result_y >= 0 && result_y < m_Height);
        }

        public void setRoom(int count)
        {

        }

        private IEnumerator createRoom(List<TezDungeonBlock> total)
        {
            int count = 2000;
            while (count > 0)
            {
                count -= 1;
                bool find = true;

                int width = m_Random.nextInt(2, 10);
                int height = m_Random.nextInt(2, 10);

                var x = m_Random.nextInt(0, (m_Width - width) / 2) * 2 + 1;
                var y = m_Random.nextInt(0, (m_Height - height) / 2) * 2 + 1;

                for (int i = 0; i < m_RoomList.Count; i++)
                {
                    if (m_RoomList[i].collision(x, y, width, height))
                    {
                        find = false;
                        break;
                    }
                }

                if (!find)
                {
                    continue;
                }
                else
                {
                    var room = new TezDungeonRoom()
                    {
                        ox = x,
                        oy = y,
                        width = width,
                        height = height
                    };
                    m_RoomList.Add(room);

                    room.foreachRoom(
                    (int ox, int oy) =>
                    {
                        if ((ox >= 0 && ox < m_Width) && (oy >= 0 && oy < m_Height))
                        {
                            m_BlockArray[ox, oy].isInited = true;
                            m_BlockArray[ox, oy].state = TezDungeonBlock.State.CanPass;
                            total.Remove(m_BlockArray[ox, oy]);
                            onCanPassCreated?.Invoke(ox, oy);
                        }
                    },

                    (int ox, int oy) =>
                    {
                        if ((ox >= 0 && ox < m_Width) && (oy >= 0 && oy < m_Height))
                        {
                            m_BlockArray[ox, oy].isInited = true;
                            m_BlockArray[ox, oy].state = TezDungeonBlock.State.CanNotPass;
                            total.Remove(m_BlockArray[ox, oy]);
                            onCanNotPassCreated?.Invoke(ox, oy);
                        }
                    });
                }

                yield return null;
            }
        }

        private IEnumerator createMaze(List<TezDungeonBlock> total)
        {
            var first = total[m_Random.nextInt(0, total.Count - 1)];
            Stack<TezDungeonBlock> selected = new Stack<TezDungeonBlock>();
            first.isInited = true;
            first.state = TezDungeonBlock.State.CanPass;
            selected.Push(first);
            onCanPassCreated?.Invoke(first.x, first.y);

            List<TezDungeonBlock> neighbor_list = new List<TezDungeonBlock>();
            while (selected.Count > 0 || total.Count > 0)
            {
                if (selected.Count == 0)
                {
                    var index = m_Random.nextInt(0, total.Count - 1);
                    var temp = total[index];
                    temp.isInited = true;
                    temp.state = TezDungeonBlock.State.CanPass;
                    selected.Push(temp);
                    onCanPassCreated?.Invoke(temp.x, temp.y);
                }

                var block = selected.Peek();
                var neighbor = this.getBlock(TezDungeonUtility.Direction.N, block);
                if (neighbor != null && !neighbor.isInited)
                {
                    neighbor_list.Add(neighbor);
                }

                neighbor = this.getBlock(TezDungeonUtility.Direction.S, block);
                if (neighbor != null && !neighbor.isInited)
                {
                    neighbor_list.Add(neighbor);
                }

                neighbor = this.getBlock(TezDungeonUtility.Direction.W, block);
                if (neighbor != null && !neighbor.isInited)
                {
                    neighbor_list.Add(neighbor);
                }

                neighbor = this.getBlock(TezDungeonUtility.Direction.E, block);
                if (neighbor != null && !neighbor.isInited)
                {
                    neighbor_list.Add(neighbor);
                }

                if (neighbor_list.Count > 0)
                {
                    var choosed = neighbor_list[m_Random.nextInt(0, neighbor_list.Count - 1)];
                    choosed.isInited = true;
                    choosed.state = TezDungeonBlock.State.CanPass;
                    selected.Push(choosed);
                    total.Remove(choosed);
                    onCanPassCreated?.Invoke(choosed.x, choosed.y);
                    neighbor_list.Remove(choosed);

                    foreach (var remain in neighbor_list)
                    {
                        remain.isInited = true;
                        remain.state = TezDungeonBlock.State.CanNotPass;
                        total.Remove(remain);
                        onCanNotPassCreated?.Invoke(remain.x, remain.y);
                    }
                    neighbor_list.Clear();
                }
                else
                {
                    selected.Pop();
                    total.Remove(block);
                    onCanPassConfirmed?.Invoke(block.x, block.y);
                }

                yield return null;
            }
        }

        public IEnumerator create()
        {
            List<TezDungeonBlock> total = new List<TezDungeonBlock>();
            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    m_BlockArray[x, y] = new TezDungeonBlock(x, y);
                    total.Add(m_BlockArray[x, y]);
                    onBlockFilled?.Invoke(x, y);
                }
            }

            yield return this.createRoom(total);
            yield return this.createMaze(total);
        }
    }
}