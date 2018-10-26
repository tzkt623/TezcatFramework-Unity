using System.Collections.Generic;

namespace tezcat.Framework.TypeTraits
{
    public class TezRTTIInfo
    {
        #region MetaData
        public abstract class MetaData
        {
            public enum Type
            {
                Bool,
                Int,
                Float,
                String,
                List,
                Dictionary,
                HashSet,
                Class
            }

            public abstract Type variableType { get; }
            public string variableName { get; private set; }

            public MetaData(string name)
            {
                this.variableName = name;
            }
        }

        public class MetaData_Bool : MetaData
        {
            public MetaData_Bool(string name) : base(name)
            {
            }

            public override Type variableType
            {
                get { return Type.Bool; }
            }
        }

        public class MetaData_Int : MetaData
        {
            public MetaData_Int(string name) : base(name)
            {
            }

            public override Type variableType
            {
                get { return Type.Int; }
            }
        }

        public class MetaData_Float : MetaData
        {
            public MetaData_Float(string name) : base(name)
            {
            }

            public override Type variableType
            {
                get { return Type.Float; }
            }
        }

        public class MetaData_String : MetaData
        {
            public MetaData_String(string name) : base(name)
            {
            }

            public override Type variableType
            {
                get { return Type.String; }
            }
        }

        public class MetaData_List : MetaData
        {
            public override Type variableType
            {
                get { return Type.List; }
            }

            public MetaData itemData { get; private set; }

            public MetaData_List(string name, MetaData item_data) : base(name)
            {
                this.itemData = item_data;
            }
        }

        public class MetaData_Dictionary : MetaData
        {
            public override Type variableType
            {
                get { return Type.Dictionary; }
            }

            public MetaData key { get; private set; }
            public MetaData value { get; private set; }

            public MetaData_Dictionary(string name, MetaData key, MetaData value) : base(name)
            {
                this.key = key;
                this.value = value;
            }
        }


        public class MetaData_HashSet : MetaData
        {
            public override Type variableType
            {
                get { return Type.HashSet; }
            }

            public MetaData itemData { get; private set; }

            public MetaData_HashSet(string name, MetaData item_data) : base(name)
            {
                this.itemData = item_data;
            }
        }

        public class MetaData_Class : MetaData
        {
            public string nameSpace { get; private set; }

            public MetaData_Class(string name_space, string name) : base(name)
            {
                this.nameSpace = name_space;
            }

            public override Type variableType
            {
                get { return Type.Class; }
            }
        }
        #endregion


        public string parentClassName { get; private set; } = null;
        public string parentNameSpace { get; private set; } = null;

        public string nameSpace { get; private set; } = null;
        public string className { get; private set; } = null;

        public List<MetaData> metaData { get; private set; } = new List<MetaData>();

        public TezRTTIInfo(string name_space, string class_name, string parent_name_space, string parent_class_name)
        {
            this.nameSpace = name_space;
            this.className = class_name;
            this.parentClassName = parent_class_name;
            this.parentNameSpace = parent_name_space;
        }

        public void addBool(string name)
        {
            this.metaData.Add(new MetaData_Bool(name));
        }

        public void addInt(string name)
        {
            this.metaData.Add(new MetaData_Int(name));
        }

        public void addFloat(string name)
        {
            this.metaData.Add(new MetaData_Float(name));
        }

        public void addString(string name)
        {
            this.metaData.Add(new MetaData_String(name));
        }

        public void addList(string name, MetaData item_data)
        {
            this.metaData.Add(new MetaData_List(name, item_data));
        }

        public void addDictionary(string name, MetaData key, MetaData value)
        {
            this.metaData.Add(new MetaData_Dictionary(name, key, value));
        }

        public void addClass(string name_space, string name)
        {
            this.metaData.Add(new MetaData_Class(name_space, name));
        }

        public void addMetaData(MetaData data)
        {
            this.metaData.Add(data);
        }
    }
}