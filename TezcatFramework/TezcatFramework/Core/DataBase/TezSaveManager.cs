using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Database
{
    public class TezSaveManager : ITezService
    {
        List<ITezSerializable> mList = new List<ITezSerializable>();

        TezWriter mWriter = new TezJsonWriter();
        TezFileReader mReader = new TezJsonReader();

        public int readCount
        {
            get
            {
                return this.mReader.count;
            }
        }

        public TezSaveManager()
        {

        }

        public T createObject<T>(string CID) where T : class
        {
            return TezcatFramework.classFactory.create<T>(CID);
        }

        public ITezSerializable get(int index)
        {
            if (mList.Count < index)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return mList[index];
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
            //             if (!mDict.TryGetValue(serializable.RUID, out id))
            //             {
            //                 id = m_List.Count;
            //                 mDict.Add(serializable.RUID, id);
            //                 m_List.Add(serializable);
            //             }

            return id;
        }

        private void saveDB()
        {
            this.beginWriteArray("DB");
            for (int i = 0; i < mList.Count; i++)
            {
                this.beginWriteObject(i);
                this.write("DBID", i);
                this.endWriteObject(i);
            }
            this.endWriteArray("DB");
        }

        public void save(string fullPath)
        {
            this.saveDB();

            this.mWriter.save(fullPath);
            this.mWriter.close();
            this.mWriter = null;
        }

        public void loadDB(string path)
        {
            TezJsonReader reader = new TezJsonReader();
            if (reader.load(path))
            {
                int count = reader.count;
                for (int i = 0; i < count; i++)
                {
                    reader.beginObject(i);
                    var CID = reader.readString(TezReadOnlyString.ClassID);
                    var obj = TezcatFramework.classFactory.create<TezDataComponent>(CID);
                    if (obj != null)
                    {
                        obj.initNew();

//                        mDict.Add(obj.GUID, m_List.Count);
//                        m_List.Add(obj);
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
            mList.Clear();
        }

        public void close()
        {

        }

        public void begin(int index)
        {
            mReader.beginObject(index);
        }

        public void saveItem(TezDataComponent my_object)
        {
//            var ruid = my_object.GUID;
        }

        public void begin(string name)
        {
            mReader.beginObject(name);
        }

        public void end(int index)
        {
            mReader.endObject(index);
        }

        public void end(string name)
        {
            mReader.endObject(name);
        }

        #region write
        public void newSave()
        {
            this.mWriter = new TezJsonWriter();
        }

        public void beginWriteArray(string name)
        {
            mWriter.beginArray(name);
        }

        public void endWriteArray(string name)
        {
            mWriter.endArray(name);
        }

        public void beginWriteArray(int index)
        {
            mWriter.beginArray(index);
        }

        public void endWriteArray(int index)
        {
            mWriter.endArray(index);
        }

        public void beginWriteObject(string name)
        {
            mWriter.beginObject(name);
        }

        public void endWriteObject(string name)
        {
            mWriter.endObject(name);
        }

        public void beginWriteObject(int index)
        {
            mWriter.beginObject(index);
        }

        public void endWriteObject(int index)
        {
            mWriter.endObject(index);
        }

        public void write(string name, bool value)
        {
            mWriter.write(name, value);
        }

        public void write(string name, int value)
        {
            mWriter.write(name, value);
        }

        public void write(string name, float value)
        {
            mWriter.write(name, value);
        }

        public void write(string name, string value)
        {
            mWriter.write(name, value);
        }

        public void write(bool value)
        {
            mWriter.write(value);
        }

        public void write(int value)
        {
            mWriter.write(value);
        }

        public void write(float value)
        {
            mWriter.write(value);
        }

        public void write(string value)
        {
            mWriter.write(value);
        }

        public void writeProperty(TezValueWrapper vw)
        {
            mWriter.writeProperty(vw);
        }
        #endregion

        #region Read
        public void newLoad(string path)
        {
            mReader = new TezJsonReader();
            mReader.load(path);
        }

        public void beginReadObject(string name)
        {
            this.mReader.beginObject(name);
        }

        public void endReadObject(string name)
        {
            this.mReader.endObject(name);
        }

        public void beginReadObject(int index)
        {
            this.mReader.beginObject(index);
        }

        public void endReadObject(int index)
        {
            this.mReader.endObject(index);
        }

        public void beginReadArray(string name)
        {
            this.mReader.beginArray(name);
        }

        public void endReadArray(string name)
        {
            this.mReader.endArray(name);
        }

        public void beginReadArray(int index)
        {
            this.mReader.beginArray(index);
        }

        public void endReadArray(int index)
        {
            this.mReader.endArray(index);
        }

        public bool readBool(string name)
        {
            return this.mReader.readBool(name);
        }

        public int readInt(string name)
        {
            return this.mReader.readInt(name);
        }

        public float readFloat(string name)
        {
            return this.mReader.readFloat(name);
        }

        public string readString(string name)
        {
            return this.mReader.readString(name);
        }

        public bool readBool(int index)
        {
            return this.mReader.readBool(index);
        }

        public int readInt(int index)
        {
            return this.mReader.readInt(index);
        }

        public float readFloat(int index)
        {
            return this.mReader.readFloat(index);
        }

        public string readString(int index)
        {
            return this.mReader.readString(index);
        }

        public void readProperty(TezValueWrapper vw)
        {
            this.mReader.readProperty(vw);
        }
        #endregion
    }
}