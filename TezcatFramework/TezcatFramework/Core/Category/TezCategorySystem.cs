using System;
using System.Collections.Generic;
using System.Text;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    public static class TezCategorySystem
    {
        #region RootToken
        static List<ITezCategoryRootToken> m_MainTokenList = new List<ITezCategoryRootToken>();
        static Dictionary<string, ITezCategoryRootToken> m_MainTokenDic = new Dictionary<string, ITezCategoryRootToken>();

        public static void registerRootToken(ITezCategoryRootToken token)
        {
            while (token.toID >= m_MainTokenList.Count)
            {
                m_MainTokenList.Add(null);
            }

            m_MainTokenList[token.toID] = token;
            m_MainTokenDic.Add(token.toName, token);
        }

        public static ITezCategoryRootToken getRootToken(string name)
        {
            return m_MainTokenDic[name];
        }

        public static ITezCategoryRootToken getRootToken(int index)
        {
            return m_MainTokenList[index];
        }
        #endregion

        #region Category
        class Slot
        {
            List<TezCategory> m_List = new List<TezCategory>();
            Dictionary<string, TezCategory> m_Dic = new Dictionary<string, TezCategory>();
            public void generate(ITezCategoryFinalToken finalToken, out TezCategory category, TezEventExtension.Function<TezCategory> onGenerate)
            {
                if (!m_Dic.TryGetValue(finalToken.toName, out category))
                {
                    category = onGenerate();
                    m_Dic.Add(finalToken.toName, category);
                    while (finalToken.toID >= m_List.Count)
                    {
                        m_List.Add(null);
                    }
                    m_List[finalToken.toID] = category;
                }
            }

            public TezCategory getCategory(string finalName)
            {
                return m_Dic[finalName];
            }

            public TezCategory getCategory(int finalIndex)
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

        public static TezCategory getCategory(int rootID, int finalID)
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

        static Dictionary<string, ITezCategoryBaseToken> m_TokenDic = new Dictionary<string, ITezCategoryBaseToken>();
        public static int registerToken(ITezCategoryBaseToken baseToken)
        {
            var id = m_TokenDic.Count;
            m_TokenDic.Add(baseToken.toName, baseToken);
            return id;
        }

        static Dictionary<string, TezCategory> m_CategoryDic = new Dictionary<string, TezCategory>();
        public static TezCategory getCategory(string finalToken)
        {
            if (m_CategoryDic.TryGetValue(finalToken, out var category))
            {
                return category;
            }

            throw new Exception(string.Format("TezCategorySystem : Category With FinalToken[{0}] no exist!!", finalToken));
        }

        public static TezCategory getCategory(ITezCategoryFinalToken finalToken)
        {
            return getCategory(finalToken.toName);
        }

        /// <summary>
        /// 用FinalToken创建一个共用的Category
        /// </summary>
        private static TezCategory createCategory(ITezCategoryFinalToken finalToken)
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
            return category;
        }

        public static ITezCategoryBaseToken getToken(string tokenName)
        {
            if (m_TokenDic.TryGetValue(tokenName, out var baseToken))
            {
                return baseToken;
            }

            throw new Exception();
        }

        #endregion

        #region FinalToken
        static int UID = 0;
        static Dictionary<string, ITezCategoryFinalToken> m_FinalTokenDic = new Dictionary<string, ITezCategoryFinalToken>();
        public static int registerFinalToken(ITezCategoryFinalToken finalToken)
        {
            m_FinalTokenDic.Add(finalToken.toName, finalToken);
            m_CategoryDic.Add(finalToken.toName, createCategory(finalToken));
            return UID++;
        }

        public static ITezCategoryFinalToken getFinalToken(string name)
        {
            if (m_FinalTokenDic.TryGetValue(name, out var token))
            {
                return token;
            }

            throw new Exception();
        }
        #endregion

        #region CShap Code File Generator
        /// <summary>
        /// 
        /// Category Cshap File Generator
        /// 
        /// </summary>
        /// <param name="outPath"></param>
        /// <param name="reader"></param>
        public static void generateCodeFile(string outPath, TezReader reader)
        {
            List<string> final_list = new List<string>();
            StringBuilder builder = new StringBuilder();

            var root_class = "Root";
            var wrapper_class = reader.readString("WrapperClass");

            var name_space = reader.readString("Namespace");
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using tezcat.Framework.Core;");
            builder.AppendLine();
            builder.AppendLine();

            ///Namespace
            builder.AppendLine(string.Format("namespace {0}", name_space));
            builder.AppendLine("{");

            ///Config Class
            builder.AppendLine(string.Format("public static class {0}", wrapper_class));
            builder.AppendLine("{");

            #region 写类
            reader.beginObject(root_class);
            var children = writeRootClass(reader, builder, root_class);
            ///从Key中获得所有Class的名称
            foreach (var class_name in children)
            {
                ///如果是Object
                ///说明是Path
                if (reader.tryBeginObject(class_name))
                {
                    ///写入当前PathClass
                    ///并且得到下一级Class的Name
                    var new_children = writePathClass(reader, builder, class_name, root_class);
                    writeClasses(new_children, reader, builder, class_name, root_class, class_name, final_list);
                    reader.endObject(class_name);

                    ///调用自己写入下一个Class
                }
                ///如果不是Object
                ///说明是Final
                else
                {
                    reader.beginArray(class_name);
                    writeFinalClass(reader, builder, class_name, root_class, root_class, class_name);
                    reader.endArray(class_name);
                    final_list.Add(class_name);
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

            var writer = TezFilePath.createTextFile(outPath + "/" + wrapper_class + ".cs");
            writer.Write(builder.ToString());
            writer.Close();
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
                else
                {
                    reader.beginArray(class_name);
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
                builder.AppendLine(string.Format("{0}.registerID({1}.{2});", reader.readString(i), rootClass, rootMember));
            }
            builder.AppendLine("}");
            #endregion


            builder.AppendLine("}");
            builder.AppendLine();
        }

        #endregion
    }
}
