using tezcat.Core;

namespace tezcat.DataBase
{
    public class TezItemSlot : TezSingleDataSlot<TezItem>
    {
        /// <summary>
        /// 槽ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 槽物品
        /// </summary>
        public TezItem item
        {
            get { return this.data; }
            set { this.data = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Type slotType
        {
            get { return this.GetType(); }
        }
    }
}
