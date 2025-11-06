using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace tezcat.Framework.Game
{
    public class TezEnum
    {
        /*
         * 设计方向
         * 
         * 快速查询,记录层级关系,与int可以相互转换
         * 
         * 伤害-物理-斩击  1101
         * 伤害-物理-刺击  1102
         * 伤害-物理-钝击  1103
         * 伤害-魔法-火焰  1201
         * 伤害-魔法-冰霜  1202
         * 伤害-魔法-雷电  1203
         */

        [StructLayout(LayoutKind.Explicit)]
        struct IDInfo
        {
            [FieldOffset(0)]
            public int ID;
            [FieldOffset(0)]
            public ushort subtypeID;
            [FieldOffset(2)]
            public short rootID;
        }

        IDInfo mIDInfo = new IDInfo();
        int mInnerIndex = 0;
        string mName = null;
        int mGlobalID = -1;
        ushort mMarkValue = 0;

        TezEnum mParent = null;
        List<TezEnum> mChildren = null;

        public string Name
        {
            get { return mName; }
        }

        public int GlobalID
        {
            get { return mGlobalID; }
        }

        public ushort markValue
        {
            get { return mMarkValue; }
        }

        public short rootID
        {
            get { return mIDInfo.rootID; }
        }

        public ushort subtypeID
        {
            get { return mIDInfo.subtypeID; }
        }

        public int innerIndex
        {
            get { return mInnerIndex; }
        }

        public int UID
        {
            get { return mIDInfo.ID; }
        }

        public bool hasParent
        {
            get { return mParent != sDefault; }
        }

        public TezEnum parent => mParent;


        private TezEnum(short highID, ushort lowID)
        {
            mIDInfo.rootID = highID;
            mIDInfo.subtypeID = lowID;
        }

        public TezEnum addChildNode(string name, ushort markValue)
        {
            if (mChildren == null)
            {
                mChildren = new List<TezEnum>();
            }

            if (sCheckNameDict.ContainsKey(name))
            {
                throw new System.Exception($"TezEnum::registerRoot Duplicate Enum Name:{name}");
            }

            var true_base_value = (ushort)(mMarkValue + markValue);
            var e = new TezEnum(this.rootID, true_base_value)
            {
                mName = name,
                mMarkValue = true_base_value,
                mGlobalID = sGlobalEnum.Count,
                mInnerIndex = mChildren.Count,
                mParent = this
            };

            if (sCheckIDDict.ContainsKey(e.UID))
            {
                throw new System.Exception($"TezEnum::registerRoot Duplicate Enum {name} UID:{e.UID} RID:{e.rootID} TID:{e.subtypeID}");
            }

            sGlobalEnum.Add(e);
            mChildren.Add(e);
            sCheckIDDict.Add(e.UID, e);
            sCheckNameDict.Add(e.Name, e);
            return e;
        }

        public TezEnum addChildLeafNode(string name)
        {
            if (mChildren == null)
            {
                mChildren = new List<TezEnum>();
            }

            if (sCheckNameDict.ContainsKey(name))
            {
                throw new System.Exception($"TezEnum::registerRoot Duplicate Enum Name:{name}");
            }

            var e = new TezEnum(this.rootID, (ushort)(this.markValue + mChildren.Count + 1))
            {
                mName = name,
                mMarkValue = mMarkValue,
                mGlobalID = sGlobalEnum.Count,
                mInnerIndex = mChildren.Count,
                mParent = this
            };

            if (sCheckIDDict.ContainsKey(e.UID))
            {
                throw new System.Exception($"TezEnum::registerRoot Duplicate Enum ID:{sRootEnum.Count}");
            }

            sGlobalEnum.Add(e);
            mChildren.Add(e);
            sCheckIDDict.Add(e.UID, e);
            sCheckNameDict.Add(e.Name, e);
            return e;
        }

        private void close()
        {
            this.mName = null;
            if (mChildren != null)
            {
                foreach (var e in mChildren)
                {
                    e.close();
                }

                mChildren.Clear();
                mChildren = null;
            }
        }

        static List<TezEnum> sGlobalEnum = new List<TezEnum>();
        static List<TezEnum> sRootEnum = new List<TezEnum>();
        static Dictionary<int, TezEnum> sCheckIDDict = new Dictionary<int, TezEnum>();
        static Dictionary<string, TezEnum> sCheckNameDict = new Dictionary<string, TezEnum>();
        static TezEnum sDefault = new TezEnum();

        public static IReadOnlyList<TezEnum> allEnum => sGlobalEnum;

        public static TezEnum get(string name)
        {
            return sCheckNameDict[name];
        }

        public static TezEnum get(int globalID)
        {
            return sGlobalEnum[globalID];
        }

        public static TezEnum registerRoot(string name, ushort baseValue)
        {
            if (sCheckNameDict.ContainsKey(name))
            {
                throw new System.Exception($"TezEnum::registerRoot Duplicate Enum Name:{name}");
            }

            var e = new TezEnum((short)sRootEnum.Count, 0)
            {
                mName = name,
                mGlobalID = sGlobalEnum.Count,
                mInnerIndex = sRootEnum.Count,
                mParent = sDefault
            };

            if (sCheckIDDict.ContainsKey(e.UID))
            {
                throw new System.Exception($"TezEnum::registerRoot Duplicate Enum ID:{sRootEnum.Count}");
            }

            sGlobalEnum.Add(e);
            sRootEnum.Add(e);
            sCheckIDDict.Add(e.UID, e);
            sCheckNameDict.Add(e.Name, e);
            return e;
        }

        public static TezEnum registerChildNode(string name, TezEnum parent, ushort markValue)
        {
            return parent.addChildNode(name, markValue);
        }

        public static TezEnum registerLeafNode(string name, TezEnum parent)
        {
            return parent.addChildLeafNode(name);
        }

        public static void clearTempCache()
        {
            sCheckIDDict.Clear();
            sCheckIDDict = null;
        }

        public static void clear()
        {
            foreach (var e in sGlobalEnum)
            {
                e.close();
            }

            sGlobalEnum.Clear();
            sRootEnum.Clear();
        }

        private TezEnum()
        {

        }
    }
}