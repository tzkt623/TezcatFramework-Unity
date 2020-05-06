using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.String
{
    public class TezStaticString : TezObject
    {
        private static List<string> StringList = null;
        private static Dictionary<string, int> StringDic = null;
        private const string ErrorString = "@Error";

        static TezStaticString()
        {
            TezStaticString.StringDic = new Dictionary<string, int>();
            TezStaticString.StringDic.Add(ErrorString, 0);
            TezStaticString.StringList = new List<string>();
            TezStaticString.StringList.Add(ErrorString);
        }

        private int m_ID = -1;

        public bool isNullOrEmpty
        {
            get { return m_ID == 0; }
        }

        public TezStaticString()
        {
            m_ID = 0;
        }

        public TezStaticString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                m_ID = 0;
            }
            else
            {
                int id = -1;
                if (TezStaticString.StringDic.TryGetValue(str, out id))
                {
                    m_ID = id;
                }
                else
                {
                    m_ID = TezStaticString.StringList.Count;
                    TezStaticString.StringList.Add(str);
                    TezStaticString.StringDic.Add(str, m_ID);
                }
            }
        }

        public TezStaticString(TezStaticString tstring)
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
            if(StringDic.TryGetValue(content, out id))
            {
                m_ID = id;
            }
            else
            {
                if (m_ID == 0)
                {
                    m_ID = TezStaticString.StringList.Count;
                    TezStaticString.StringList.Add(content);
                    TezStaticString.StringDic.Add(content, m_ID);
                }
                else
                {
                    TezStaticString.StringDic.Remove(StringList[m_ID]);
                    TezStaticString.StringDic.Add(content, m_ID);
                    TezStaticString.StringList[m_ID] = content;
                }
            }
        }

        public override bool Equals(object other)
        {
            var temp = other as TezStaticString;
            if (temp)
            {
                return temp.m_ID == m_ID;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return m_ID;
        }

        public string convertToString()
        {
            return TezStaticString.StringList[m_ID];
        }

        public TezStaticString convertFormString(string str)
        {
            return new TezStaticString(str);
        }

        public void reset()
        {
            m_ID = 0;
        }

        public override void close(bool self_close = true)
        {
            m_ID = -1;
        }

        public static int getIDFromString(string str)
        {
            int id = -1;
            if (!TezStaticString.StringDic.TryGetValue(str, out id))
            {
                id = TezStaticString.StringList.Count;
                TezStaticString.StringList.Add(str);
                TezStaticString.StringDic.Add(str, id);
            }

            return id;
        }

        public static implicit operator string(TezStaticString tstring)
        {
            if (tstring == null)
            {
                return TezStaticString.StringList[0];
            }

            return TezStaticString.StringList[tstring.m_ID];
        }

        public static implicit operator TezStaticString(string str)
        {
            return new TezStaticString(str);
        }

        public static bool operator ==(TezStaticString x, TezStaticString y)
        {
            bool flagX = object.ReferenceEquals(x, null);
            bool flagY = object.ReferenceEquals(y, null);

            return (flagX && flagY) || (!flagX && !flagY) && (x.m_ID == y.m_ID);
        }

        public static bool operator !=(TezStaticString x, TezStaticString y)
        {
            /// (!true || !false) && (x)
            /// (!false || !false) && (x.m_ID != y.m_ID)

            bool flagX = object.ReferenceEquals(x, null);
            bool flagY = object.ReferenceEquals(y, null);

            return (!flagX || !flagY) && (flagX || flagY) || (x.m_ID != y.m_ID);
        }
    }
}