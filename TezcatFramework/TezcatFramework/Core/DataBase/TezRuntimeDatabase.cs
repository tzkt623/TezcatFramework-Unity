using System.Collections.Generic;


namespace tezcat.Framework.Core
{
    public class TezRuntimeDatabase
    {
        List<TezProtoObjectData> mItemList = new List<TezProtoObjectData>();
        Queue<int> mFreeIDs = new Queue<int>();

        public void registerItem(TezProtoObjectData protoObjectData)
        {
            if(mFreeIDs.Count > 0)
            {
                var id = mFreeIDs.Dequeue();
                protoObjectData.itemInfo.setRTID(id);
                mItemList[id] = protoObjectData;
            }
            else
            {
                protoObjectData.itemInfo.setRTID(mItemList.Count);
                mItemList.Add(protoObjectData);
            }

            protoObjectData.itemInfo.setOnClear(() =>
            {
                this.unregisterItem(protoObjectData);
            });
        }

        public void unregisterItem(TezProtoObjectData protoInfo)
        {
            mItemList[protoInfo.itemInfo.itemID.RTID] = null;
            mFreeIDs.Enqueue(protoInfo.itemInfo.itemID.RTID);
        }
    }
}