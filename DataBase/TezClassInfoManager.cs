﻿using System.Collections.Generic;
namespace tezcat.DataBase
{
    public static class TezClassInfoManager
    {
        const string _namespace = "namespace";
        const string _class = "class";
        const string _parent_class = "parent_class";
        const string _parent_namespace = "parent_namespace";
        const string _metadata = "metadata";
        const string _variable_type = "variable_type";
        const string _variable_name = "variable_name";
        const string _list_item = "list_item";
        const string _set_item = "set_item";
        const string _key = "key";
        const string _value = "value";

        static Dictionary<string, Dictionary<string, TezClassInfo>> m_NameSpace = new Dictionary<string, Dictionary<string, TezClassInfo>>();

        public static void foreachInfo(TezEventBus.Action<string, TezClassInfo> action)
        {
            foreach (var pair in m_NameSpace)
            {
                var ns = pair.Key;
                var dic = pair.Value;
                foreach (var p in dic)
                {
                    action(pair.Key, p.Value);
                }
            }
        }

        public static TezClassInfo addClassInfo(string name_space, string class_name, string parent_name_space, string parent_class_name)
        {
            Dictionary<string, TezClassInfo> class_info = null;
            if(!m_NameSpace.TryGetValue(name_space, out class_info))
            {
                class_info = new Dictionary<string, TezClassInfo>();
                m_NameSpace.Add(name_space, class_info);
            }     

            var info = new TezClassInfo(name_space, class_name, parent_name_space, parent_class_name);
            class_info.Add(class_name, info);
            return info;
        }

        public static TezClassInfo getClassInfo(string name_space, string class_name)
        {
            return m_NameSpace[name_space][class_name];
        }

        static TezClassInfo.MetaData readMetaData(TezJsonReader reader)
        {
            var type = reader.getString(_variable_type);
            var name = reader.getString(_variable_name);
            TezClassInfo.MetaData meta = null;
            switch (type)
            {
                case "bool":
                    meta = new TezClassInfo.MetaData_Bool(name);
                    break;
                case "int":
                    meta = new TezClassInfo.MetaData_Int(name);
                    break;
                case "float":
                    meta = new TezClassInfo.MetaData_Float(name);
                    break;
                case "string":
                    meta = new TezClassInfo.MetaData_String(name);
                    break;
                case "list":
                    reader.enter(_list_item);
                    meta = new TezClassInfo.MetaData_List(name, readMetaData(reader));
                    reader.exit();
                    break;
                case "dictionary":
                    ///key
                    reader.enter(_key);
                    var key = readMetaData(reader);
                    reader.exit();
                    ///value
                    reader.enter("value");
                    var value = readMetaData(reader);
                    reader.exit();
                    ///
                    meta = new TezClassInfo.MetaData_Dictionary(name, key, value);
                    break;
                case "hashset":
                    reader.enter(_set_item);
                    meta = new TezClassInfo.MetaData_HashSet(name, readMetaData(reader));
                    reader.exit();
                    break;
                case "class":
                    meta = new TezClassInfo.MetaData_Class(reader.getString(_namespace), name);
                    break;
                default:
                    break;
            }

            return meta;
        }

        public static void loadClassInfoFile(string path)
        {
            TezJsonReader reader = new TezJsonReader();
            reader.load(path);

            var count = reader.count();
            for (int i = 0; i < count; i++)
            {
                reader.enter(i);
                var info = addClassInfo(
                    reader.getString(_namespace), reader.getString(_class),
                    reader.getString(_parent_namespace), reader.getString(_parent_class));

                reader.enter(_metadata);
                var mdcount = reader.count();
                for (int j = 0; j < mdcount; j++)
                {
                    reader.enter(j);
                    info.addMetaData(readMetaData(reader));
                    reader.exit();
                }
                reader.exit();

                reader.exit();
            }
        }

        public static void saveClassInfoFile(string path)
        {

        }
    }
}