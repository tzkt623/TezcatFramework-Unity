using System.Collections.Generic;
using System.Text;
using tezcat.Framework.Core;


namespace tezcat.Framework.Game
{
    public class TezBonusToken
    {
        public string name;

        public int typeID;
        public int indexID;


        private TezBonusToken() { }

        private TezBonusToken(string name, int typeID, int indexID)
        {
            this.name = name;
            this.typeID = typeID;
            this.indexID = indexID;
        }

        #region Manager
        class Cell
        {
            public List<TezBonusToken> list = new List<TezBonusToken>(8);
            public Dictionary<string, TezBonusToken> dict = new Dictionary<string, TezBonusToken>();

            public void addToken(TezBonusToken token)
            {
                while (list.Count <= token.indexID)
                {
                    list.Add(null);
                }

                list[token.indexID] = token;
                dict.Add(token.name, token);
            }

            public void loadConfig(TezReader reader, int typeID)
            {
                foreach (var key in reader.getKeys())
                {
                    var token = dict[key];
                    token.typeID = typeID;
                    token.indexID = reader.readInt(key);
                    list[token.indexID] = token;
                }
            }
        }

        static List<Cell> mCellList = new List<Cell>();
        static Dictionary<string, Cell> mCellDict = new Dictionary<string, Cell>();

        /// <summary>
        /// 创建一个Token
        /// typeid是token的大类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeID"></param>
        /// <param name="indexID"></param>
        /// <returns></returns>
        public static TezBonusToken createToken(string name, int typeID, int indexID)
        {
            var type_id = typeID;
            var token = new TezBonusToken(name, type_id, indexID);
            while (mCellList.Count <= type_id)
            {
                mCellList.Add(new Cell());
            }
            mCellList[type_id].addToken(token);
            mCellDict.Add(name, mCellList[type_id]);
            return token;
        }

        public static int getTokenCapacity(int typeID)
        {
            return mCellList[typeID].list.Count;
        }

        public static void loadConfig(string path)
        {
            TezFileReader reader = new TezJsonReader();
            if (reader.load(path))
            {
                foreach (var key in reader.getKeys())
                {
                    reader.beginObject(key);

                    var cell = mCellDict[key];
                    var type_id = reader.readInt("TypeID");
                    mCellList[type_id] = cell;

                    reader.beginObject("InedxID");
                    mCellDict[key].loadConfig(reader, type_id);
                    reader.endObject("InedxID");

                    reader.endObject(key);
                }
            }
        }

        public static void generateBonusCShapFile(string configPath, string outPath)
        {
            TezFileReader reader = new TezJsonReader();
            if (reader.load(configPath))
            {
                StringBuilder builder = new StringBuilder();
                foreach (var key in reader.getKeys())
                {
                    reader.beginObject(key);



                    reader.endObject(key);
                }
            }
        }
        #endregion
    }
}