using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 携带ID的String
    /// 用ID来代替字符串进行快速比较
    /// 所持有的字符串全都会被保存起来
    /// 
    /// 可以用来配合本地化系统快速查找本地化文本等
    /// </summary>
    public sealed class TezString<Belong>
        : IEquatable<TezString<Belong>>
        , ITezCloseable
    {
        #region Tool
        public static readonly TezString<Belong> empty = new TezString<Belong>();

        private static Queue<TezString<Belong>> sPool = new Queue<TezString<Belong>>();

        private static List<string> sStringList = new List<string>()
        {
            "$TezSys_Empty$"
        };
        private static Dictionary<string, int> sStringDict = new Dictionary<string, int>()
        {
            {"$TezSys_Empty$", 0}
        };

        /// <summary>
        /// 注册一个新的ID
        /// </summary>
        public static TezString<Belong> register(string value)
        {
            if (sPool.Count > 0)
            {
                var temp = sPool.Dequeue();
                temp.change(value);
                return temp;
            }

            return new TezString<Belong>(value, false);
        }

        /// <summary>
        /// 创建一个空ID
        /// </summary>
        /// <returns></returns>
        public static TezString<Belong> create()
        {
            if (sPool.Count > 0)
            {
                return sPool.Dequeue();
            }

            return new TezString<Belong>();
        }

        /// <summary>
        /// 注册一个String
        /// 返回此String的ID
        /// 试用与内建Sting
        /// </summary>
        public static int registerString(string value)
        {
            if (!sStringDict.TryGetValue(value, out int id))
            {
                id = sStringList.Count;
                sStringList.Add(value);
                sStringDict.Add(value, id);
            }

            return id;
        }

        public static bool isNullOrEmpty(TezString<Belong> myString)
        {
            return object.ReferenceEquals(myString, null) || myString.mID == 0;
        }

        public static bool getIDFromString(string str, out int id)
        {
            return sStringDict.TryGetValue(str, out id);
        }
        #endregion

        private int mID = -1;
        /// <summary>
        /// 序号
        /// </summary>
        public int ID => mID;

        /// <summary>
        /// 文本内容
        /// </summary>
        public string content => sStringList[mID];

        private TezString()
        {
            mID = 0;
        }

        private TezString(string str, bool create)
        {
            if (string.IsNullOrEmpty(str))
            {
                mID = 0;
            }
            else
            {
                if (!sStringDict.TryGetValue(str, out mID))
                {
                    if (create)
                    {
                        mID = sStringList.Count;
                        sStringList.Add(str);
                        sStringDict.Add(str, mID);
                    }
                }
            }
        }

        public TezString(TezString<Belong> other)
        {
            if (object.ReferenceEquals(other, null))
            {
                mID = 0;
            }
            else
            {
                mID = other.mID;
            }
        }

        /// <summary>
        /// 改变当前String的内容
        /// </summary>
        public bool change(string str)
        {
            if (sStringDict.TryGetValue(str, out mID))
            {
                return true;
            }

            mID = -1;
            return false;
        }

        /// <summary>
        /// 修改当前位置的String内容
        /// 而不是新加一个
        /// </summary>
        private void replace(string content)
        {
            if (mID == 0)
            {
                return;
            }

            sStringDict.Remove(sStringList[mID]);
            sStringDict.Add(content, mID);
            sStringList[mID] = content;
        }

        void ITezCloseable.closeThis()
        {
            mID = 0;
            sPool.Enqueue(this);
        }

        public override string ToString()
        {
            return $"[{mID}=><{this.content}>]";
        }

        public override int GetHashCode()
        {
            return mID;
        }

        public bool Equals(TezString<Belong> other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mID == other.mID;
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezString<Belong>)other);
        }

        //         public static implicit operator string(TezString<Belong> myString)
        //         {
        //             if (myString == null)
        //             {
        //                 return sStringList[0];
        //             }
        // 
        //             return sStringList[myString.mID];
        //         }

        /// <summary>
        /// 隐式转换
        /// 不会生成没有的String
        /// 
        /// 想创建新的请用Create
        /// </summary>
        //         public static implicit operator TezString<Belong>(string str)
        //         {
        //             return new TezString<Belong>(str, false);
        //         }

        /// <summary>
        /// 比较两个String的ID是否相同
        /// 如果为null则永远不会相同
        /// </summary>
        public static bool operator ==(TezString<Belong> a, TezString<Belong> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        /// <summary>
        /// 比较两个String的ID是否相同
        /// 如果为null则永远不会相同
        /// </summary>
        public static bool operator !=(TezString<Belong> a, TezString<Belong> b)
        {
            return !(a == b);
        }
    }
}