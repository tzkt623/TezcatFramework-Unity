using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 携带ID的String
    /// 用ID来代替字符串进行快速比较
    /// </summary>
    public class TezIDString : ITezCloseable
    {
        private static List<string> StringList = null;
        private static Dictionary<string, int> StringDic = null;
        private const string ErrorString = "@Error";

        static TezIDString()
        {
            TezIDString.StringDic = new Dictionary<string, int>();
            TezIDString.StringDic.Add(ErrorString, 0);
            TezIDString.StringList = new List<string>();
            TezIDString.StringList.Add(ErrorString);
        }

        private int m_ID = -1;

        public bool isNullOrEmpty
        {
            get { return m_ID == 0; }
        }

        public TezIDString()
        {
            m_ID = 0;
        }

        public TezIDString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                m_ID = 0;
            }
            else
            {
                if (!TezIDString.StringDic.TryGetValue(str, out m_ID))
                {
                    m_ID = TezIDString.StringList.Count;
                    TezIDString.StringList.Add(str);
                    TezIDString.StringDic.Add(str, m_ID);
                }
            }
        }

        public TezIDString(TezIDString tstring)
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
            int id = -1;
            if (StringDic.TryGetValue(content, out id))
            {
                m_ID = id;
            }
            else
            {
                if (m_ID == 0)
                {
                    m_ID = TezIDString.StringList.Count;
                    TezIDString.StringList.Add(content);
                    TezIDString.StringDic.Add(content, m_ID);
                }
                else
                {
                    TezIDString.StringDic.Remove(StringList[m_ID]);
                    TezIDString.StringDic.Add(content, m_ID);
                    TezIDString.StringList[m_ID] = content;
                }
            }
        }

        public string convertToString()
        {
            return TezIDString.StringList[m_ID];
        }

        public TezIDString convertFormString(string str)
        {
            return new TezIDString(str);
        }

        public void reset()
        {
            m_ID = 0;
        }

        public void close()
        {
            m_ID = -1;
        }

        public override int GetHashCode()
        {
            return m_ID;
        }

        public override bool Equals(object other)
        {
            var temp = other as TezIDString;
            if (temp != null)
            {
                return temp.m_ID == m_ID;
            }

            return false;
        }

        public static int getIDFromString(string str)
        {
            int id = -1;
            if (!TezIDString.StringDic.TryGetValue(str, out id))
            {
                id = TezIDString.StringList.Count;
                TezIDString.StringList.Add(str);
                TezIDString.StringDic.Add(str, id);
            }

            return id;
        }

        public static implicit operator string(TezIDString tstring)
        {
            if (tstring == null)
            {
                return TezIDString.StringList[0];
            }

            return TezIDString.StringList[tstring.m_ID];
        }

        public static implicit operator TezIDString(string str)
        {
            return new TezIDString(str);
        }

        public static bool operator ==(TezIDString x, TezIDString y)
        {
            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (flagx && flagy) || (!flagx && !flagy && (x.m_ID == y.m_ID));
        }

        public static bool operator !=(TezIDString x, TezIDString y)
        {
            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (!flagx || !flagy) && (flagx || flagy || (x.m_ID != y.m_ID));
        }
    }
}