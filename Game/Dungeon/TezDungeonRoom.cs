using tezcat.Extension;

namespace tezcat.Game
{
    public class TezDungeonRoom
    {
        public int ox { get; set; }
        public int oy { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        public int maxX
        {
            get { return ox + width - 1; }
        }

        public int maxY
        {
            get { return oy + height - 1; }
        }


        public bool collision(TezDungeonRoom room)
        {
            return !(room.maxX < this.ox - 1 || room.ox > this.maxX + 1 || room.maxY < this.oy - 1 || room.oy > this.maxY + 1);
        }

        public bool collision(int ox, int oy, int widht, int height)
        {
            int maxX = ox + widht - 1;
            int maxy = oy + height - 1;

            return !(maxX < this.ox - 1 || ox > this.maxY + 1 || maxY < this.oy - 1 || oy > this.maxY + 1);
        }

        public void foreachRoom(TezEventExtension.Action<int, int> in_room, TezEventExtension.Action<int, int> in_wall)
        {
            for (int y = -1; y < height + 1; y++)
            {
                for (int x = -1; x < width + 1; x++)
                {
                    if((x == - 1 || x == width) || (y == - 1 || y == height))
                    {
                        in_wall(x + ox, y + oy);
                    }
                    else
                    {
                        in_room(x + ox, y + oy);
                    }
                }
            }
        }
    }
}

