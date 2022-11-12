namespace tezcat.Framework.Database
{
    /// <summary>
    /// 物品类型ID
    /// 数据库可以使用此类型ID对数据进行分类存储
    /// 
    /// 如何处理多级分类
    /// 比如EVE的市场分类
    /// 舰船装备--炮台和发射器--射弹炮台--自动加农炮--中型 ===> 舰船装备--自动加农炮
    /// 舰船装备--武器升级--散热槽 ===> 舰船装备--散热槽
    /// 舰船--巡洋舰--标准巡洋舰--米玛塔尔 ===> 舰船--巡洋舰
    /// </summary>
    public static class TezItemTypeID
    {
        public static int generateID(int managerID, int typeID)
        {
            return (managerID << 16) & typeID;
        }

        public static int getManagerID(int itemTypeID)
        {
            return itemTypeID >> 16;
        }

        public static int getTypeID(int itemTypeID)
        {
            return itemTypeID & 0x0000_ffff;
        }
    }

    public interface ITezDBItemObject
    {
        /// <summary>
        /// 物品类型ID
        /// 如果此ID为-1
        /// 则表示此对象没有分类系统
        /// </summary>
        int dbUID { get; }

        void initWithData(ITezSerializable item);

        bool compare(ITezDBItemObject other);
    }
}