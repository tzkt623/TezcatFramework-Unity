using System.Collections.Generic;

namespace tezcat
{
    public class TezStaticString
    {
        private static List<string> StringList = null;
        private static Dictionary<string, int> StringDic = null;
        public static TezStaticString empty { get; private set; }

        static TezStaticString()
        {
            TezStaticString.StringDic = new Dictionary<string, int>();
            TezStaticString.StringDic.Add(string.Empty, 0);
            TezStaticString.StringList = new List<string>();
            TezStaticString.StringList.Add(string.Empty);
            TezStaticString.empty = new TezStaticString();
        }

        private int m_ID = -1;

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
            if(tstring == null)
            {
                m_ID = 0;
            }
            else
            {
                m_ID = tstring.m_ID;
            }
        }

        public static implicit operator string(TezStaticString tstring)
        {
            if (tstring == null)
            {
                return string.Empty;
            }

            return StringList[tstring.m_ID];
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


        public static bool operator != (TezStaticString x, TezStaticString y)
        {
            /// (!true || !false) && (x)
            /// (!false || !false) && (x.m_ID != y.m_ID)

            bool flagX = object.ReferenceEquals(x, null);
            bool flagY = object.ReferenceEquals(y, null);

            return (!flagX || !flagY) && (flagX || flagY) || (x.m_ID != y.m_ID);
        }

        public static bool isNullOrEmpty(TezStaticString tstring)
        {
            return object.ReferenceEquals(tstring, null) || tstring.m_ID == 0;
        }

        public static int getIDFromString(string str)
        {
            int id = -1;
            if(!TezStaticString.StringDic.TryGetValue(str, out id))
            {
                id = TezStaticString.StringList.Count;
                TezStaticString.StringList.Add(str);
                TezStaticString.StringDic.Add(str, id);
            }

            return id;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            var temp = other as TezStaticString;
            if (temp != null)
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
    }
}