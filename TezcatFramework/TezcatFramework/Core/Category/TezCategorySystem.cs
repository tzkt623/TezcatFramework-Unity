using System;
using System.Collections.Generic;
using System.Text;

namespace tezcat.Framework.Core
{
    public static class TezCategorySystem
    {
        #region RootToken
        static List<ITezCategoryRootToken> mRootTokenList = new List<ITezCategoryRootToken>();
        static Dictionary<string, ITezCategoryRootToken> mRootTokenDict = new Dictionary<string, ITezCategoryRootToken>();

        public static void registerRootToken(ITezCategoryRootToken token)
        {
            while (token.intValue >= mRootTokenList.Count)
            {
                mRootTokenList.Add(null);
            }

            mRootTokenList[token.intValue] = token;
            mRootTokenDict.Add(token.name, token);
        }

        public static ITezCategoryRootToken getRootToken(string name)
        {
            return mRootTokenDict[name];
        }

        public static ITezCategoryRootToken getRootToken(int index)
        {
            return mRootTokenList[index];
        }
        #endregion

        #region FinalToken
        static List<ITezCategoryFinalToken> mFinalTokenList = new List<ITezCategoryFinalToken>();
        static Dictionary<string, ITezCategoryFinalToken> mFinalTokenDict = new Dictionary<string, ITezCategoryFinalToken>();
        public static int registerFinalToken(ITezCategoryFinalToken finalToken)
        {
            int uid = mFinalTokenList.Count;
            mFinalTokenDict.Add(finalToken.name, finalToken);
            mFinalTokenList.Add(finalToken);
            return uid;
        }

        public static ITezCategoryFinalToken getFinalToken(string name)
        {
            if (mFinalTokenDict.TryGetValue(name, out var token))
            {
                return token;
            }

            throw new Exception();
        }
        #endregion

        #region Category
        /*
        class Slot
        {
            List<TezCategory> m_List = new List<TezCategory>();
            Dictionary<string, TezCategory> mDict = new Dictionary<string, TezCategory>();
            public void generate(ITezCategoryFinalToken finalToken, out TezCategory category, TezEventExtension.Function<TezCategory> onGenerate)
            {
                if (!mDict.TryGetValue(finalToken.toName, out category))
                {
                    category = onGenerate();
                    mDict.Add(finalToken.toName, category);
                    while (finalToken.toID >= m_List.Count)
                    {
                        m_List.Add(null);
                    }
                    m_List[finalToken.toID] = category;
                }
            }

            public TezCategory getCategory(string finalName)
            {
                return mDict[finalName];
            }

            public TezCategory getCategory(init finalIndex)
            {
                return m_List[finalIndex];
            }
        }

        static List<Slot> m_SlotList = new List<Slot>();
        static Dictionary<string, Slot> m_SlotDic = new Dictionary<string, Slot>();

        public static TezCategory getCategory(string rootName, string finalName)
        {
            if (m_SlotDic.TryGetValue(rootName, out Slot slot))
            {
                return slot.getCategory(finalName);
            }

            throw new Exception();
        }

        public static TezCategory getCategory(init rootID, init finalID)
        {
            return m_SlotList[rootID].getCategory(finalID);
        }

        public static void generate(ITezCategoryRootToken rootToken, ITezCategoryFinalToken finalToken, out TezCategory category, TezEventExtension.Function<TezCategory> onGenerate)
        {
            if (!m_SlotDic.TryGetValue(rootToken.toName, out Slot slot))
            {
                slot = new Slot();
                while (rootToken.toID >= m_SlotList.Count)
                {
                    m_SlotList.Add(null);
                }
                m_SlotList[rootToken.toID] = slot;
                m_SlotDic.Add(rootToken.toName, slot);
            }

            slot.generate(finalToken, out category, onGenerate);
        }
        */

        static Dictionary<string, ITezCategoryBaseToken> mTokenDict = new Dictionary<string, ITezCategoryBaseToken>();
        static List<ITezCategoryBaseToken> mTokenList = new List<ITezCategoryBaseToken>();
        public static int registerToken(ITezCategoryBaseToken baseToken)
        {
            var id = mTokenList.Count;
            mTokenList.Add(baseToken);
            mTokenDict.Add(baseToken.name, baseToken);
            return id;
        }

        public static ITezCategoryBaseToken getToken(string tokenName)
        {
            if (mTokenDict.TryGetValue(tokenName, out var baseToken))
            {
                return baseToken;
            }

            throw new Exception();
        }

        public static ITezCategoryBaseToken getToken(int index)
        {
            return mTokenList[index];
        }

        static Dictionary<string, TezCategory> mCategoryDict = new Dictionary<string, TezCategory>();
        static List<TezCategory> mCategoryList = new List<TezCategory>();
        public static TezCategory getCategory(string finalToken)
        {
            if (mCategoryDict.TryGetValue(finalToken, out var category))
            {
                return category;
            }

            throw new Exception(string.Format("TezCategorySystem : Category With FinalToken[{0}] no exist!!", finalToken));
        }

        public static TezCategory getCategory(ITezCategoryFinalToken finalToken)
        {
            return mCategoryList[finalToken.globalID];
        }

        /// <summary>
        /// 用FinalToken创建一个共用的Category
        /// </summary>
        public static void createCategory(ITezCategoryFinalToken finalToken)
        {
            Stack<ITezCategoryBaseToken> stack = new Stack<ITezCategoryBaseToken>();
            ITezCategoryBaseToken temp_token = finalToken;

            while (temp_token != null)
            {
                stack.Push(temp_token);
                temp_token = temp_token.parent;
            }

            TezCategory category = new TezCategory();
            category.setToken(stack.ToArray());
            mCategoryDict.Add(finalToken.name, category);

            while (mCategoryList.Count <= finalToken.globalID)
            {
                mCategoryList.Add(null);
            }
            mCategoryList[finalToken.globalID] = category;

            stack.Clear();
        }

        public static void registerTypeIDFrom(TezReader reader)
        {
            foreach (var item in mCategoryList)
            {
                item.finalToken.setTypeID(reader.readInt(item.finalToken.parent.name));
            }
        }
        #endregion

        #region CShap Code File Generator
        class Token
        {
            public enum Type
            {
                Path,
                Final,
                Member
            }

            public string name;
            public Type type;
        }


        private static void parseToken(TezReader reader, string rootClass)
        {
            reader.beginObject(rootClass);

            ICollection<string> keys = reader.getKeys();
            ///从Key中获得所有Class的名称
            foreach (var token_name in keys)
            {
                ///如果是Object
                ///说明是Path
                if (reader.tryBeginObject(token_name))
                {
                    new Token()
                    {
                        type = Token.Type.Path,
                        name = token_name
                    };
                    reader.endObject(token_name);
                }
                ///如果不是Object
                ///说明是Final
                else if (reader.tryBeginArray(token_name))
                {
                    new Token()
                    {
                        type = Token.Type.Final,
                        name = token_name
                    };
                    reader.endArray(token_name);
                }
                else
                {
                    new Token()
                    {
                        type = Token.Type.Member,
                        name = token_name
                    };
                }
            }

            reader.endObject(rootClass);
        }


        /// <summary>
        /// 
        /// Category Cshap File Generator
        /// 
        /// </summary>
        /// <param name="outPath"></param>
        /// <param name="reader"></param>
        public static void generateCShapFile(string outPath, TezReader reader)
        {
            #region 生成.cs文件
            List<string> final_list = new List<string>();
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using tezcat.Framework.Database;");
            builder.AppendLine();
            builder.AppendLine();

            ///Namespace
            var name_space = reader.readString("Namespace");
            builder.AppendLine(string.Format("namespace {0}", name_space));
            builder.AppendLine("{");

            ///Config Class
            var wrapper_class = reader.readString("WrapperClass");
            builder.AppendLine(string.Format("public static class {0}", wrapper_class));
            builder.AppendLine("{");

            #region 写类
            var root_class = "Root";
            reader.beginObject(root_class);
            var children = writeRootClass(reader, builder, root_class);

            ///从Key中获得所有Class的名称
            foreach (var token_name in children)
            {
                ///处理Object
                ///Object一定是path类型
                if (reader.tryBeginObject(token_name))
                {
                    ///写入当前PathClass
                    ///并且得到下一级Class的Name
                    var new_children = writePathClass(reader, builder, token_name, root_class);
                    writeClasses(new_children, reader, builder, token_name, root_class, token_name, final_list);
                    reader.endObject(token_name);
                }
                ///如果是Array
                ///说明有的可能是Final
                ///有的可能是Path
                else if (reader.tryBeginArray(token_name))
                {
                    writeFinalClass(reader, builder, token_name, root_class, root_class, token_name);
                    reader.endArray(token_name);
                    final_list.Add(token_name);
                }
            }
            reader.endObject(root_class);
            #endregion

            #region 初始化函数
            builder.AppendLine("public static void init()");
            builder.AppendLine("{");
            for (int i = 0; i < final_list.Count; i++)
            {
                builder.AppendLine(string.Format("{0}.init();", final_list[i]));
            }
            builder.AppendLine("}");
            #endregion

            ///Config Class
            builder.AppendLine("}");

            ///Namespace
            builder.AppendLine("}");

            var writer = TezFilePath.createTextFile(outPath);
            writer.Write(builder.ToString());
            writer.Flush();
            writer.Close();
            #endregion
        }

        public static void generateItemConfigFile(string outPath, TezReader reader)
        {
            TezJsonWriter writer = new TezJsonWriter();

            reader.beginObject("Root");
            int id = 0;
            foreachCategoryFile(ref id, reader, writer);
            reader.endObject("Root");

            writer.save(outPath);
            writer.close();
        }

        private static void foreachCategoryFile(ref int id, TezReader reader, TezWriter writer)
        {
            foreach (var key in reader.getKeys())
            {
                if (reader.tryBeginObject(key))
                {
                    foreachCategoryFile(ref id, reader, writer);
                    reader.endObject(key);
                }
                else
                {
                    writer.write(key, id++);
                }
            }
        }

        private static void writeClasses(ICollection<string> children, TezReader reader, StringBuilder builder, string parentClass, string rootClass, string rootMemeber, List<string> finalList)
        {
            ///从Key中获得所有Class的名称
            foreach (var class_name in children)
            {
                ///如果是Object
                ///说明是Path
                if (reader.tryBeginObject(class_name))
                {
                    ///写入当前PathClass
                    ///并且得到下一级Class的Name
                    var new_children = writePathClass(reader, builder, class_name, parentClass);
                    ///调用自己写入下一个Class
                    writeClasses(new_children, reader, builder, class_name, rootClass, rootMemeber, finalList);
                    reader.endObject(class_name);
                }
                ///如果不是Object
                ///说明是Final
                else if (reader.tryBeginArray(class_name))
                {
                    writeFinalClass(reader, builder, class_name, parentClass, rootClass, rootMemeber);
                    reader.endArray(class_name);
                    finalList.Add(class_name);
                }
            }
        }

        private static ICollection<string> writeRootClass(TezReader reader, StringBuilder builder, string className)
        {
            builder.AppendLine(string.Format("public class {0} : TezCategoryRootToken<{0}, {0}.Category>", className));
            builder.AppendLine("{");

            #region 枚举变量
            builder.AppendLine("public enum Category");
            builder.AppendLine("{");
            var keys = reader.getKeys();
            foreach (var key in keys)
            {
                builder.AppendLine(key + ",");
            }
            builder.AppendLine("}");
            #endregion

            #region 构造函数
            builder.AppendLine(string.Format("private {0}(Category value) : base(value)", className));
            builder.AppendLine("{");
            builder.AppendLine("}");
            #endregion

            #region 生成变量
            foreach (var key in keys)
            {
                builder.AppendLine(string.Format("public static readonly {0} {1} = new {0}(Category.{1});", className, key));
            }
            #endregion

            builder.AppendLine("}");
            builder.AppendLine();
            return keys;
        }

        private static ICollection<string> writePathClass(TezReader reader, StringBuilder builder, string className, string parentClass)
        {
            builder.AppendLine(string.Format("public class {0} : TezCategoryPathToken<{0}, {0}.Category>", className));
            builder.AppendLine("{");

            #region 枚举变量
            builder.AppendLine("public enum Category");
            builder.AppendLine("{");
            var keys = reader.getKeys();
            foreach (var key in keys)
            {
                builder.AppendLine(key + ",");
            }
            builder.AppendLine("}");
            #endregion

            #region 构造函数
            builder.AppendLine(string.Format("private {0}(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)", className));
            builder.AppendLine("{");
            builder.AppendLine("}");
            #endregion

            #region 生成变量
            foreach (var member_name in keys)
            {
                builder.AppendLine(string.Format("public static readonly {0} {1} = new {0}(Category.{1}, {2}.{0});", className, member_name, parentClass));
            }
            #endregion

            builder.AppendLine("}");
            builder.AppendLine();
            return keys;
        }

        private static ICollection<string> writePathOrFinalClass(TezReader reader, StringBuilder builder, string className, string parentClass)
        {
            builder.AppendLine(string.Format("public class {0} : TezCategoryPathToken<{0}, {0}.Category>", className));
            builder.AppendLine("{");

            #region 枚举变量
            builder.AppendLine("public enum Category");
            builder.AppendLine("{");
            var keys = reader.getKeys();
            foreach (var key in keys)
            {
                builder.AppendLine(key + ",");
            }
            builder.AppendLine("}");
            #endregion

            #region 构造函数
            builder.AppendLine(string.Format("private {0}(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)", className));
            builder.AppendLine("{");
            builder.AppendLine("}");
            #endregion

            #region 生成变量
            foreach (var member_name in keys)
            {
                builder.AppendLine(string.Format("public static readonly {0} {1} = new {0}(Category.{1}, {2}.{0});", className, member_name, parentClass));
            }
            #endregion

            builder.AppendLine("}");
            builder.AppendLine();
            return keys;
        }

        private static void writeFinalClass(TezReader reader, StringBuilder builder, string className, string parentClass, string rootClass, string rootMember)
        {
            builder.AppendLine(string.Format("public class {0} : TezCategoryFinalToken<{0}, {0}.Category>", className));
            builder.AppendLine("{");

            #region 枚举变量
            builder.AppendLine("public enum Category");
            builder.AppendLine("{");
            for (int i = 0; i < reader.count; i++)
            {
                builder.AppendLine(reader.readString(i) + ",");
            }
            builder.AppendLine("}");
            #endregion

            #region 构造函数
            builder.AppendLine(string.Format("private {0}(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)", className));
            builder.AppendLine("{");
            builder.AppendLine("}");
            #endregion

            #region 声明变量
            for (int i = 0; i < reader.count; i++)
            {
                builder.AppendLine(string.Format("public static readonly {0} {1} = new {0}(Category.{1}, {2}.{0});", className, reader.readString(i), parentClass));
            }
            #endregion

            #region 初始化变量
            builder.AppendLine("public static void init()");
            builder.AppendLine("{");
            for (int i = 0; i < reader.count; i++)
            {
                builder.AppendLine(string.Format("{0}.registerID();", reader.readString(i)));
            }
            builder.AppendLine("}");
            #endregion


            builder.AppendLine("}");
            builder.AppendLine();
        }

        #endregion
    }
}
