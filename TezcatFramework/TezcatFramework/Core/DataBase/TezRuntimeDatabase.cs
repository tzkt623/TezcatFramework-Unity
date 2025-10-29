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
                protoObjectData.protoInfo.setRTID(id);
                mItemList[id] = protoObjectData;
            }
            else
            {
                protoObjectData.protoInfo.setRTID(mItemList.Count);
                mItemList.Add(protoObjectData);
            }

            protoObjectData.protoInfo.setOnClear(() =>
            {
                this.unregisterItem(protoObjectData);
            });
        }

        public void unregisterItem(TezProtoObjectData protoInfo)
        {
            mItemList[protoInfo.protoInfo.itemID.RTID] = null;
            mFreeIDs.Enqueue(protoInfo.protoInfo.itemID.RTID);
        }
    }
}