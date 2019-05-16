using System;
using System.Collections.Generic;
using tezcat.Framework.DataBase;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezSaveManager : ITezService
    {
        Dictionary<TezRID, int> m_Dic = new Dictionary<TezRID, int>();
        List<ITezSerializable> m_List = new List<ITezSerializable>();

        TezWriter m_Writer = new TezJsonWriter();
        TezReader m_Reader = new TezJsonReader();

        public int readCount
        {
            get
            {
                return this.m_Reader.count;
            }
        }

        TezClassFactory m_Factory = null;

        public TezSaveManager()
        {
            m_Factory = TezService.get<TezClassFactory>();
        }

        public T createObject<T>(string CID) where T : class
        {
            return m_Factory.create<T>(CID);
        }

        public T createObject<T>() where T : class
        {
            return m_Factory.create<T>();
        }

        public ITezSerializable get(int index)
        {
            if (m_List.Count < index)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return m_List[index];
        }

        public T get<T>(int index) where T : ITezSerializable
        {
            return (T)this.get(index);
        }

        /// <summary>
        /// 缓存数据
        /// 如果已经缓存了 则返回缓存ID
        /// </summary>
        /// <param name="serializable"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int cache(ITezSerializable serializable)
        {
            int id = -1;
            //             if (!m_Dic.TryGetValue(serializable.RUID, out id))
            //             {
            //                 id = m_List.Count;
            //                 m_Dic.Add(serializable.RUID, id);
            //                 m_List.Add(serializable);
            //             }

            return id;
        }

        private void saveDB()
        {
            this.beginWriteArray("DB");
            for (int i = 0; i < m_List.Count; i++)
            {
                this.beginWriteObject(i);
                this.write("DBID", i);
                this.endWriteObject(i);
            }
            this.endWriteArray("DB");
        }

        public void save(string full_path)
        {
            this.saveDB();

            this.m_Writer.save(full_path);
            this.m_Writer.close();
            this.m_Writer = null;
        }

        public void loadDB(string path)
        {
            TezJsonReader reader = new TezJsonReader();
            if (reader.load(path))
            {
                var factory = TezService.get<TezClassFactory>();
                int count = reader.count;
                for (int i = 0; i < count; i++)
                {
                    reader.beginObject(i);
                    var CID = reader.readString(TezReadOnlyString.CID);
                    var obj = factory.create<TezGameObject>(CID);
                    if (obj != null)
                    {
                        obj.initNew();

//                        m_Dic.Add(obj.GUID, m_List.Count);
                        m_List.Add(obj);
                    }
                    else
                    {
                        throw new Exception(string.Format("Save Data Broken\nPosition:{0}\nCID:{1}", i, CID));
                    }
                    reader.endObject(i);
                }
            }
        }

        public void reset()
        {
            m_List.Clear();
            m_Dic.Clear();
        }

        public void close()
        {
            m_Factory = null;
        }

        public void begin(int index)
        {
            m_Reader.beginObject(index);
        }

        public void saveItem(TezGameObject my_object)
        {
//            var ruid = my_object.GUID;
        }

        public void begin(string name)
        {
            m_Reader.beginObject(name);
        }

        public void end(int index)
        {
            m_Reader.endObject(index);
        }

        public void end(string name)
        {
            m_Reader.endObject(name);
        }

        #region write
        public void newSave()
        {
            this.m_Writer = new TezJsonWriter();
        }

        public void beginWriteArray(string name)
        {
            m_Writer.beginArray(name);
        }

        public void endWriteArray(string name)
        {
            m_Writer.endArray(name);
        }

        public void beginWriteArray(int index)
        {
            m_Writer.beginArray(index);
        }

        public void endWriteArray(int index)
        {
            m_Writer.endArray(index);
        }

        public void beginWriteObject(string name)
        {
            m_Writer.beginObject(name);
        }

        public void endWriteObject(string name)
        {
            m_Writer.endObject(name);
        }

        public void beginWriteObject(int index)
        {
            m_Writer.beginObject(index);
        }

        public void endWriteObject(int index)
        {
            m_Writer.endObject(index);
        }

        public void write(string name, bool value)
        {
            m_Writer.write(name, value);
        }

        public void write(string name, int value)
        {
            m_Writer.write(name, value);
        }

        public void write(string name, float value)
        {
            m_Writer.write(name, value);
        }

        public void write(string name, string value)
        {
            m_Writer.write(name, value);
        }

        public void write(bool value)
        {
            m_Writer.write(value);
        }

        public void write(int value)
        {
            m_Writer.write(value);
        }

        public void write(float value)
        {
            m_Writer.write(value);
        }

        public void write(string value)
        {
            m_Writer.write(value);
        }

        public void writeProperty(TezValueWrapper vw)
        {
            m_Writer.writeProperty(vw);
        }
        #endregion

        #region Read
        public void newLoad(string path)
        {
            this.m_Reader = new TezJsonReader();
            m_Reader.load(path);
        }

        public void beginReadObject(string name)
        {
            this.m_Reader.beginObject(name);
        }

        public void endReadObject(string name)
        {
            this.m_Reader.endObject(name);
        }

        public void beginReadObject(int index)
        {
            this.m_Reader.beginObject(index);
        }

        public void endReadObject(int index)
        {
            this.m_Reader.endObject(index);
        }

        public void beginReadArray(string name)
        {
            this.m_Reader.beginArray(name);
        }

        public void endReadArray(string name)
        {
            this.m_Reader.endArray(name);
        }

        public void beginReadArray(int index)
        {
            this.m_Reader.beginArray(index);
        }

        public void endReadArray(int index)
        {
            this.m_Reader.endArray(index);
        }

        public bool readBool(string name)
        {
            return this.m_Reader.readBool(name);
        }

        public int readInt(string name)
        {
            return this.m_Reader.readInt(name);
        }

        public float readFloat(string name)
        {
            return this.m_Reader.readFloat(name);
        }

        public string readString(string name)
        {
            return this.m_Reader.readString(name);
        }

        public bool readBool(int index)
        {
            return this.m_Reader.readBool(index);
        }

        public int readInt(int index)
        {
            return this.m_Reader.readInt(index);
        }

        public float readFloat(int index)
        {
            return this.m_Reader.readFloat(index);
        }

        public string readString(int index)
        {
            return this.m_Reader.readString(index);
        }

        public void readProperty(TezValueWrapper vw)
        {
            this.m_Reader.readProperty(vw);
        }
        #endregion
    }
}