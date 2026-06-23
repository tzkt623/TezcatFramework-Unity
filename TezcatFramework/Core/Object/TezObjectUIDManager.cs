using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezObjectUID
    {
        uint objectUID { get; set; }
    }

    /// <summary>
    /// 实体对象的唯一ID
    /// </summary>
    public static class TezObjectUIDManager
    {
        /// <summary>
        /// 对象被创建时
        /// </summary>
        public static event TezEventExtension.Action<uint> evtObjectGenerated;
        /// <summary>
        /// 对象被回收时
        /// </summary>
        public static event TezEventExtension.Action<uint> evtObjectRecycled;

        /// <summary>
        /// 错误UID
        /// </summary>
        public const uint cErrorUID = 0;

        static Queue<uint> IDPool = new Queue<uint>();
        static uint mIDGenerate = 0;

        public static uint totalCount => mIDGenerate;

        /// <summary>
        /// 激活的
        /// </summary>
        public static uint activedCount
        {
            get { return mIDGenerate - (uint)IDPool.Count; }
        }

        /// <summary>
        /// 被回收的UID数量
        /// </summary>
        public static uint freeCount
        {
            get { return (uint)IDPool.Count; }
        }

        static uint generateID()
        {
            if (IDPool.Count > 0)
            {
                return IDPool.Dequeue();
            }

            mIDGenerate++;
            evtObjectGenerated?.Invoke(mIDGenerate);
            return mIDGenerate;
        }

        static void recycleID(uint uid)
        {
            if (uid != cErrorUID)
            {
                IDPool.Enqueue(uid);
                evtObjectRecycled?.Invoke(uid);
            }
        }

        public static void generateUID(this ITezObjectUID uid)
        {
            uid.objectUID = generateID();
        }

        public static void recycleUID(this ITezObjectUID uid)
        {
            recycleID(uid.objectUID);
            uid.objectUID = cErrorUID;
        }
    }
}