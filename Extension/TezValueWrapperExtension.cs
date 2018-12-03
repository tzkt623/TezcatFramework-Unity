using tezcat.Framework.Core;
using tezcat.Framework.DataBase;

namespace tezcat.Framework.Extension
{
    public static class TezValueWrapperExtension
    {
        public static void writePropertyCollection(TezWriter writer, ITezPropertyCollection collection, string name)
        {
            writer.beginObject(name);
            collection.foreachProperty((TezValueWrapper wrapper) =>
            {
                switch (wrapper.valueType)
                {
                    case TezValueType.Int:
                        var iv = (TezValueWrapper<int>)wrapper;
                        writer.write(iv.name, iv.value);
                        break;
                    default:
                        break;
                }
            });
            writer.endObject(name);
        }

        public static void readPropertyCollection(TezReader reader, ITezPropertyCollection collection, string name)
        {
            reader.beginObject(name);
            var keys = reader.getKeys();
            foreach (var key in keys)
            {
                collection.register(new TezValueWrapper<int>(TezValueDescriptor.get(key))
                {
                    value = reader.readInt(key)
                });
            }
            reader.endObject(name);
        }

        public static void writeProperty(this TezWriter writer, TezValueWrapper vw)
        {
            switch (vw.valueType)
            {
                case TezValueType.Bool:
                    {
                        var result = (TezValueWrapper<bool>)vw;
                        writer.write(result.name, result.value);
                        break;
                    }
                case TezValueType.Int:
                    {
                        var result = (TezValueWrapper<int>)vw;
                        writer.write(result.name, result.value);
                        break;
                    }
                case TezValueType.Float:
                    {
                        var result = (TezValueWrapper<float>)vw;
                        writer.write(result.name, result.value);
                        break;
                    }
                case TezValueType.Double:
                    break;
                case TezValueType.String:
                    {
                        var result = (TezValueWrapper<string>)vw;
                        writer.write(result.name, result.value);
                        break;
                    }
                case TezValueType.Class:
                    break;
                case TezValueType.StaticString:
                    break;
                case TezValueType.Type:
                    break;
                case TezValueType.Unknown:
                    break;
                default:
                    break;
            }
        }

        public static void readProperty(this TezReader reader, TezValueWrapper vw)
        {
            switch (vw.valueType)
            {
                case TezValueType.Bool:
                    {
                        var result = (TezValueWrapper<bool>)vw;
                        result.value = reader.readBool(result.name);
                        break;
                    }
                case TezValueType.Int:
                    {
                        var result = (TezValueWrapper<int>)vw;
                        result.value = reader.readInt(result.name);
                        break;
                    }
                case TezValueType.Float:
                    {
                        var result = (TezValueWrapper<float>)vw;
                        result.value = reader.readFloat(result.name);
                        break;
                    }
                case TezValueType.Double:
                    break;
                case TezValueType.String:
                    {
                        var result = (TezValueWrapper<string>)vw;
                        result.value = reader.readString(result.name);
                        break;
                    }
                case TezValueType.Class:
                    break;
                case TezValueType.StaticString:
                    break;
                case TezValueType.Type:
                    break;
                case TezValueType.Unknown:
                    break;
                default:
                    break;
            }
        }
    }
}