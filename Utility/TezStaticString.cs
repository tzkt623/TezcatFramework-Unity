using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezStaticString : ITezCloseable
    {
        int ID { get; }
        string content { get; }
    }

    /// <summary>
    /// 携带ID的String
    /// 用ID来代替字符串进行快速比较
    /// 所持有的字符串全都会被保存起来
    /// 
    /// 通常用来配合本地化系统快速查找本地化文本
    /// </summary>
    public sealed class TezStaticString<Belong>
        : ITezStaticString
        , IEquatable<TezStaticString<Belong>>
    {
        public static readonly TezStaticString<Belong> empty = new TezStaticString<Belong>();

        private static List<string> StringList = new List<string>();
        private static Dictionary<string, int> StringDic = new Dictionary<string, int>();


        private int m_ID = -1;

        /// <summary>
        /// 序号
        /// </summary>
        public int ID => m_ID;
        /// <summary>
        /// 文本内容
        /// </summary>
        public string content => TezStaticString<Belong>.StringList[m_ID];

        /// <summary>
        /// 创建一个新的IDString
        /// </summary>
        public static TezStaticString<Belong> create(string value)
        {
            return new TezStaticString<Belong>(value, true);
        }

        /// <summary>
        /// 注册一个String
        /// 返回此String的ID
        /// </summary>
        public static int registerString(string value)
        {
            if (!TezStaticString<Belong>.StringDic.TryGetValue(value, out int id))
            {
                id = TezStaticString<Belong>.StringList.Count;
                TezStaticString<Belong>.StringList.Add(value);
                TezStaticString<Belong>.StringDic.Add(value, id);
            }

            return id;
        }

        public TezStaticString()
        {
            m_ID = 0;
        }

        private TezStaticString(string str, bool create)
        {
            if (string.IsNullOrEmpty(str))
            {
                m_ID = 0;
            }
            else
            {
                if (!TezStaticString<Belong>.StringDic.TryGetValue(str, out m_ID))
                {
                    if (create)
                    {
                        m_ID = TezStaticString<Belong>.StringList.Count;
                        TezStaticString<Belong>.StringList.Add(str);
                        TezStaticString<Belong>.StringDic.Add(str, m_ID);
                    }
                }
            }
        }

        public TezStaticString(TezStaticString<Belong> tstring)
        {
            if (tstring == null)
            {
                m_ID = 0;
            }
            else
            {
                m_ID = tstring.m_ID;
            }
        }

        /// <summary>
        /// 修改当前位置的String内容
        /// 而不是新加一个
        /// </summary>
        public void replace(string content)
        {
            if (m_ID == 0)
            {
                return;
            }

            TezStaticString<Belong>.StringDic.Remove(StringList[m_ID]);
            TezStaticString<Belong>.StringDic.Add(content, m_ID);
            TezStaticString<Belong>.StringList[m_ID] = content;
        }

        public void close()
        {
            m_ID = 0;
        }

        public override string ToString()
        {
            return string.Format("[{0}<{1}>]", m_ID, this.content);
        }

        public override int GetHashCode()
        {
            return m_ID;
        }

        public bool Equals(TezStaticString<Belong> other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return m_ID == other.m_ID;
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezStaticString<Belong>)other);
        }

        public static bool isNullOrEmpty(TezStaticString<Belong> idString)
        {
            if (object.ReferenceEquals(idString, null))
            {
                return true;
            }

            return idString.m_ID == 0;
        }

        public static int getIDFromString(string str)
        {
            if (TezStaticString<Belong>.StringDic.TryGetValue(str, out int id))
            {
                return id;
            }

            throw new Exception();
        }

        public static implicit operator string(TezStaticString<Belong> idstring)
        {
            if (idstring == null)
            {
                return TezStaticString<Belong>.StringList[0];
            }

            return TezStaticString<Belong>.StringList[idstring.m_ID];
        }

        /// <summary>
        /// 隐式转换
        /// 不会生成没有的String
        /// 
        /// 想创建新的请用Create
        /// </summary>
        public static implicit operator TezStaticString<Belong>(string str)
        {
            return new TezStaticString<Belong>(str, false);
        }

        /// <summary>
        /// 比较两个String的ID是否相同
        /// 如果为null则永远不会相同
        /// </summary>
        public static bool operator ==(TezStaticString<Belong> a, TezStaticString<Belong> b)
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
        public static bool operator !=(TezStaticString<Belong> a, TezStaticString<Belong> b)
        {
            return !(a == b);
        }
    }
}