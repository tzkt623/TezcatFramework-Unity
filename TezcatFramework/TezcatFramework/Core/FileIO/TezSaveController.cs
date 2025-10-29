using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;

namespace tezcat.Framework.Core
{
    public class TezSaveController
    {
        public class Reader : ITezCloseable
        {
            public enum DataType : int
            {
                Error,
                Bool,
                Int,
                Long,
                Float,
                String,
                Object,
                Array
            }

            public interface IData : ITezCloseable
            {
                IData parent { get; set; }
                DataType dataType { get; }
                void add(string name, IData data);
                IData get(string name);
                IData get(int index);
                int count { get; }

                string name { get; set; }
                int index { get; set; }
            }

            class DataObject : Dictionary<string, IData>, IData
            {
                public DataType dataType => DataType.Object;
                IData IData.parent { get; set; } = null;
                string IData.name { get; set; } = null;
                int IData.index { get; set; } = -1;
                int IData.count => this.Count;

                void IData.add(string name, IData data)
                {
                    data.parent = this;
                    data.name = name;
                    this.Add(name, data);
                }

                public void close()
                {
                    foreach (var item in this)
                    {
                        item.Value.close();
                    }

                    this.Clear();
                }

                IData IData.get(string name)
                {
                    return this[name];
                }

                IData IData.get(int index)
                {
                    throw new NotImplementedException($"DataObject error get index {index}");
                }
            }

            class DataArray : List<IData>, IData
            {
                public DataType dataType => DataType.Array;
                IData IData.parent { get; set; } = null;
                string IData.name { get; set; } = null;
                int IData.index { get; set; } = -1;
                int IData.count => this.Count;

                void IData.add(string name, IData data)
                {
                    data.parent = this;
                    data.index = this.Count;
                    this.Add(data);
                }

                public void close()
                {
                    foreach (var item in this)
                    {
                        item.close();
                    }

                    this.Clear();
                }

                IData IData.get(string name)
                {
                    throw new NotImplementedException($"DataArray error get string {name}");
                }

                IData IData.get(int index)
                {
                    return this[index];
                }
            }

            public class DataT<T> : IData
            {
                public DataType type = DataType.Error;
                public T value;

                DataType IData.dataType => type;
                IData IData.parent { get; set; } = null;
                string IData.name { get; set; } = null;
                int IData.index { get; set; } = -1;
                int IData.count => 0;

                void IData.add(string name, IData data)
                {
                    throw new NotImplementedException();
                }

                IData IData.get(string name)
                {
                    throw new NotImplementedException();
                }

                IData IData.get(int index)
                {
                    throw new NotImplementedException();
                }

                public void close()
                {
                    this.value = default;
                }
            }

            class Helper
            {
                public IData current;

                public void push(string name, IData data)
                {
                    this.current.add(name, data);
                    this.current = data;
                }

                public void pop()
                {
                    this.current = (IData)this.current.parent;
                }

                public void setData(string name, IData data)
                {
                    this.current.add(name, data);
                }

                public void close()
                {
                    this.current = null;
                }
            }

            IData mRoot = null;
            IData mCurrent = null;
            string mCurrentName = null;
            int mCurrentIndex = -1;

            public int count => mCurrent.count;
            public IReadOnlyDictionary<string, IData> dict => (IReadOnlyDictionary<string, IData>)mCurrent;
            public IReadOnlyList<IData> array => (IReadOnlyList<IData>)mCurrent;

            public void beginReadObject()
            {
                mCurrent = mRoot;
                this.checkType(DataType.Object);
                mCurrentIndex = -1;
                mCurrentName = null;
            }

            public void beginReadArray()
            {
                mCurrent = mRoot;
                this.checkType(DataType.Array);
                mCurrentIndex = -1;
                mCurrentName = null;
            }

            public void endReadArray()
            {
                if (mCurrent.parent != null)
                {
                    throw new Exception();
                }
                mCurrent = null;
            }

            public void endReadObject()
            {
                if (mCurrent.parent != null)
                {
                    throw new Exception();
                }
                mCurrent = null;
            }

            private void setCurrent(string name)
            {
                mCurrentName = name;
                mCurrent = mCurrent.get(name);
            }

            private void setCurrent(int index)
            {
                mCurrentIndex = index;
                mCurrent = mCurrent.get(index);
            }

            private void resetCurrent(string name)
            {
                mCurrent = (IData)mCurrent.parent;
                mCurrentName = mCurrent.name;
            }

            private void resetCurrent(int index)
            {
                mCurrent = (IData)mCurrent.parent;
                mCurrentName = mCurrent.name;
            }

            public void enterObject(string name)
            {
                this.setCurrent(name);
                this.checkType(DataType.Object);
            }

            public void enterObject(int index)
            {
                this.setCurrent(index);
                this.checkType(DataType.Object);
            }

            public void enterArray(string name)
            {
                this.setCurrent(name);
                this.checkType(DataType.Array);
            }

            public void enterArray(int index)
            {
                this.setCurrent(index);
                this.checkType(DataType.Array);
            }

            public void exitObject(string name)
            {
                this.checkName(name);
                this.resetCurrent(name);
            }

            public void exitObject(int index)
            {
                this.checkIndex(index);
                this.resetCurrent(index);
            }

            public void exitArray(string name)
            {
                this.checkName(name);
                this.resetCurrent(name);
            }

            public void exitArray(int index)
            {
                this.checkIndex(index);
                this.resetCurrent(index);
            }

            private void checkType(DataType type)
            {
                if (mCurrent.dataType != type)
                {
                    throw new Exception($"Current data type is not {type}, but {mCurrent.dataType}");
                }
            }

            private void checkIndex(int index)
            {
                if (mCurrentIndex != index)
                {
                    throw new Exception($"Current data index is not {index}, but {mCurrentIndex}");
                }
            }

            private void checkName(string name)
            {
                if (mCurrentName != name)
                {
                    throw new Exception($"Current data name is not {name}, but {mCurrentName}");
                }
            }

            public void setIndex(int index)
            {
                mCurrent = mCurrent.get(index);
            }

            public T readValue<T>(string name)
            {
                return ((DataT<T>)mCurrent.get(name)).value;
            }

            public bool tryEnterObject(string name)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    this.checkType(DataType.Object);
                    mCurrentName = name;
                    mCurrent = data;
                    return true;
                }

                return false;
            }

            public bool tryEnterArray(string name)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    this.checkType(DataType.Array);
                    mCurrentName = name;
                    mCurrent = data;
                    return true;
                }

                return false;
            }

            public bool tryRead<T>(string name, out T value)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    value = ((DataT<T>)data).value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool tryRead(string name, out bool value)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    value = ((DataT<bool>)data).value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool tryRead(string name, out int value)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    value = ((DataT<int>)data).value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool tryRead(string name, out long value)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    value = ((DataT<long>)data).value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool tryRead(string name, out float value)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    value = ((DataT<float>)data).value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool tryRead(string name, out string value)
            {
                if (((DataObject)mCurrent).TryGetValue(name, out var data))
                {
                    value = ((DataT<string>)data).value;
                    return true;
                }

                value = default;
                return false;
            }

            public bool readBool(string name)
            {
                return ((DataT<bool>)mCurrent.get(name)).value;
            }

            public int readInt(string name)
            {
                return ((DataT<int>)mCurrent.get(name)).value;
            }

            public long readLong(string name)
            {
                return ((DataT<long>)mCurrent.get(name)).value;
            }

            public float readFloat(string name)
            {
                return ((DataT<float>)mCurrent.get(name)).value;
            }

            public string readString(string name)
            {
                return ((DataT<string>)mCurrent.get(name)).value;
            }

            public T readValue<T>(int index)
            {
                return ((DataT<T>)mCurrent.get(index)).value;
            }

            public bool readBool(int index)
            {
                return ((DataT<bool>)mCurrent.get(index)).value;
            }

            public int readInt(int index)
            {
                return ((DataT<int>)mCurrent.get(index)).value;
            }

            public long readLong(int index)
            {
                return ((DataT<long>)mCurrent.get(index)).value;
            }

            public float readFloat(int index)
            {
                return ((DataT<float>)mCurrent.get(index)).value;
            }

            public string readString(int index)
            {
                return ((DataT<string>)mCurrent.get(index)).value;
            }

            private void createRoot(Helper helper, JsonReader reader)
            {
                reader.Read();
                switch (reader.Token)
                {
                    case JsonToken.ObjectStart:
                        mRoot = new DataObject();
                        helper.current = mRoot;
                        break;
                    case JsonToken.ArrayStart:
                        mRoot = new DataArray();
                        helper.current = mRoot;
                        break;
                    default:
                        throw new Exception();
                }
            }

            public bool load(string filePath)
            {
                if(mRoot != null)
                {
                    throw new Exception("Root is already created, please close it first.");
                }

                var content = File.ReadAllText(filePath, Encoding.UTF8);
                if (content != null)
                {
                    var helper = new Helper();

                    JsonReader jsonReader = new JsonReader(content);
                    this.createRoot(helper, jsonReader);
                    string name = null;

                    while (jsonReader.Read())
                    {
                        switch (jsonReader.Token)
                        {
                            case JsonToken.None:
                                break;
                            case JsonToken.ObjectStart:
                                helper.push(name, new DataObject());
                                break;
                            case JsonToken.PropertyName:
                                name = (string)jsonReader.Value;
                                break;
                            case JsonToken.ObjectEnd:
                                helper.pop();
                                break;
                            case JsonToken.ArrayStart:
                                helper.push(name, new DataArray());
                                break;
                            case JsonToken.ArrayEnd:
                                helper.pop();
                                break;
                            case JsonToken.Int:
                                helper.setData(name, new DataT<int>()
                                {
                                    type = DataType.Int,
                                    value = (int)jsonReader.Value
                                });
                                break;
                            case JsonToken.Long:
                                helper.setData(name, new DataT<long>()
                                {
                                    type = DataType.Long,
                                    value = (long)jsonReader.Value
                                });
                                break;
                            case JsonToken.Double:
                                helper.setData(name, new DataT<float>()
                                {
                                    type = DataType.Float,
                                    value = (float)jsonReader.Value
                                });
                                break;
                            case JsonToken.String:
                                helper.setData(name, new DataT<string>()
                                {
                                    type = DataType.String,
                                    value = (string)jsonReader.Value
                                });
                                break;
                            case JsonToken.Boolean:
                                helper.setData(name, new DataT<bool>()
                                {
                                    type = DataType.String,
                                    value = (bool)jsonReader.Value
                                });
                                break;
                            case JsonToken.Null:
                                break;
                            default:
                                break;
                        }
                    }

                    helper.close();

                    mCurrent = mRoot;
                    return true;
                }

                return false;
            }

            public void close()
            {
                mRoot.close();
                //mRoot.close();
                mRoot = null;
            }
        }

        public class Writer : ITezCloseable
        {
            StringBuilder mStringBuilder = null;
            JsonWriter mWriter = null;
            string mPath;
            Stack<string> mNameStack = new Stack<string>();
            Stack<int> mIndexStack = new Stack<int>();

            private void beginSave(string path)
            {
                mPath = path;
                mStringBuilder = new StringBuilder();
                mWriter = new JsonWriter(mStringBuilder);
            }

            private void endSave()
            {
                File.WriteAllText(mPath, mStringBuilder.ToString());
                mStringBuilder.Clear();
                mPath = null;
            }

            public void beginWriteObject(string path)
            {
                this.beginSave(path);
                mWriter.WriteObjectStart();
            }

            public void beginWriteArray(string path)
            {
                this.beginSave(path);
                mWriter.WriteArrayStart();
            }

            public void endWriteObject()
            {
                mWriter.WriteObjectEnd();
                this.endSave();
            }

            public void endWriteArray()
            {
                mWriter.WriteArrayEnd();
                this.endSave();
            }

            public void enterObject(string name)
            {
                mNameStack.Push(name);
                mWriter.WritePropertyName(name);
                mWriter.WriteObjectStart();
            }

            public void enterArray(string name)
            {
                mNameStack.Push(name);
                mWriter.WritePropertyName(name);
                mWriter.WriteArrayStart();
            }

            public void enterObject(int index)
            {
                mIndexStack.Push(index);
                mWriter.WriteObjectStart();
            }

            public void enterArray(int index)
            {
                mIndexStack.Push(index);
                mWriter.WriteArrayStart();
            }

            public void exitObject(string name)
            {
                if (name != mNameStack.Peek())
                {
                    throw new Exception($"Current object name is not {name}, but {mNameStack.Peek()}");
                }

                mNameStack.Pop();
                mWriter.WriteObjectEnd();
            }

            public void exitArray(string name)
            {
                if (name != mNameStack.Peek())
                {
                    throw new Exception($"Current object name is not {name}, but {mNameStack.Peek()}");
                }

                mNameStack.Pop();
                mWriter.WriteArrayEnd();
            }

            public void exitObject(int index)
            {
                if (index != mIndexStack.Peek())
                {
                    throw new Exception($"Current object index is not {index}, but {mIndexStack.Peek()}");
                }

                mIndexStack.Pop();
                mWriter.WriteObjectEnd();
            }

            public void exitArray(int index)
            {
                if (index != mIndexStack.Peek())
                {
                    throw new Exception($"Current object index is not {index}, but {mIndexStack.Peek()}");
                }

                mIndexStack.Pop();
                mWriter.WriteArrayEnd();
            }



            public void write(string name, bool value)
            {
                mWriter.WritePropertyName(name);
                mWriter.Write(value);
            }

            public void write(string name, int value)
            {
                mWriter.WritePropertyName(name);
                mWriter.Write(value);
            }

            public void write(string name, long value)
            {
                mWriter.WritePropertyName(name);
                mWriter.Write(value);
            }

            public void write(string name, float value)
            {
                mWriter.WritePropertyName(name);
                mWriter.Write(value);
            }

            public void write(string name, String value)
            {
                mWriter.WritePropertyName(name);
                mWriter.Write(value);
            }

            public void write(bool value)
            {
                mWriter.Write(value);
            }

            public void write(int value)
            {
                mWriter.Write(value);
            }

            public void write(long value)
            {
                mWriter.Write(value);
            }

            public void write(float value)
            {
                mWriter.Write(value);
            }

            public void write(String value)
            {
                mWriter.Write(value);
            }

            public void close()
            {
                mWriter = null;
                mStringBuilder = null;
            }
        }
    }
}
