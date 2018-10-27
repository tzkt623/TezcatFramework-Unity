using System.Collections.Generic;
using System.IO;

namespace tezcat.Framework.Utility
{

    public class TezCodeWriter
    {
        private List<string> m_Lines = new List<string>();

        private string m_Path;

        private int m_Indent;

        public TezCodeWriter(string path)
        {
            this.m_Path = path;
        }

        public void comment(string text)
        {
            this.m_Lines.Add("// " + text);
        }

        public void beginPartialClass(string class_name, string parent_name = null)
        {
            string text = "public partial class " + class_name;
            if (parent_name != null)
            {
                text = text + " : " + parent_name;
            }
            this.line(text);
            this.line("{");
            this.m_Indent++;
        }

        public void beginClass(string class_name, string parent_name = null)
        {
            string text = "public class " + class_name;
            if (parent_name != null)
            {
                text = text + " : " + parent_name;
            }
            this.line(text);
            this.line("{");
            this.m_Indent++;
        }

        public void endClass()
        {
            this.m_Indent--;
            this.line("}");
        }

        public void beginNameSpace(string name)
        {
            this.line("namespace " + name);
            this.line("{");
            this.m_Indent++;
        }

        public void endNameSpace()
        {
            this.m_Indent--;
            this.line("}");
        }

        public void beginArrayStructureInitialization(string name)
        {
            this.line("new " + name);
            this.line("{");
            this.m_Indent++;
        }

        public void endArrayStructureInitialization(bool last_item)
        {
            this.m_Indent--;
            if (!last_item)
            {
                this.line("},");
            }
            else
            {
                this.line("}");
            }
        }

        public void beginArrayArrayInitialization(string array_type, string array_name)
        {
            this.line(array_name + " = new " + array_type + "[]");
            this.line("{");
            this.m_Indent++;
        }

        public void endArrayArrayInitialization(bool last_item)
        {
            this.m_Indent--;
            if (last_item)
            {
                this.line("}");
            }
            else
            {
                this.line("},");
            }
        }

        public void beginConstructor(string name)
        {
            this.line("public " + name + "()");
            this.line("{");
            this.m_Indent++;
        }

        public void endConstructor()
        {
            this.m_Indent--;
            this.line("}");
        }

        public void beginArrayAssignment(string array_type, string array_name)
        {
            this.line(array_name + " = new " + array_type + "[]");
            this.line("{");
            this.m_Indent++;
        }

        public void endArrayAssignment()
        {
            this.m_Indent--;
            this.line("};");
        }

        public void fieldAssignment(string field_name, string value)
        {
            this.line(field_name + " = " + value + ";");
        }

        public void beginStructureDelegateFieldInitializer(string name)
        {
            this.line(name + "=delegate()");
            this.line("{");
            this.m_Indent++;
        }

        public void endStructureDelegateFieldInitializer()
        {
            this.m_Indent--;
            this.line("},");
        }

        public void beginIf(string condition)
        {
            this.line("if(" + condition + ")");
            this.line("{");
            this.m_Indent++;
        }

        public void beginElseIf(string condition)
        {
            this.m_Indent--;
            this.line("}");
            this.line("else if(" + condition + ")");
            this.line("{");
            this.m_Indent++;
        }

        public void endIf()
        {
            this.m_Indent--;
            this.line("}");
        }

        public void beginFunctionDeclaration(string name, string parameter, string return_type)
        {
            this.line(string.Concat(new string[]
            {
            "public ",
            return_type,
            " ",
            name,
            "(",
            parameter,
            ")"
            }));
            this.line("{");
            this.m_Indent++;
        }

        public void beginFunctionDeclaration(string name, string return_type)
        {
            this.line(string.Concat(new string[]
            {
            "public ",
            return_type,
            " ",
            name,
            "()"
            }));
            this.line("{");
            this.m_Indent++;
        }

        public void endFunctionDeclaration()
        {
            this.m_Indent--;
            this.line("}");
        }

        private void internalNamedParameter(string name, string value, bool last_parameter)
        {
            string str = string.Empty;
            if (!last_parameter)
            {
                str = ",";
            }
            this.line(name + ":" + value + str);
        }

        public void namedParameterBool(string name, bool value, bool last_parameter = false)
        {
            this.internalNamedParameter(name, value.ToString().ToLower(), last_parameter);
        }

        public void namedParameterInt(string name, int value, bool last_parameter = false)
        {
            this.internalNamedParameter(name, value.ToString(), last_parameter);
        }

        public void namedParameterFloat(string name, float value, bool last_parameter = false)
        {
            this.internalNamedParameter(name, value.ToString() + "f", last_parameter);
        }

        public void namedParameterString(string name, string value, bool last_parameter = false)
        {
            this.internalNamedParameter(name, value, last_parameter);
        }

        public void beginFunctionCall(string name)
        {
            this.line(name);
            this.line("(");
            this.m_Indent++;
        }

        public void endFunctionCall()
        {
            this.m_Indent--;
            this.line(");");
        }

        public void functionCall(string function_name, params string[] parameters)
        {
            string str = function_name + "(";
            for (int i = 0; i < parameters.Length; i++)
            {
                str += parameters[i];
                if (i != parameters.Length - 1)
                {
                    str += ", ";
                }
            }
            this.line(str + ");");
        }

        public void structureFieldInitializer(string field, string value)
        {
            this.line(field + " = " + value + ",");
        }

        public void structureArrayFieldInitializer(string field, string field_type, params string[] values)
        {
            string text = field + " = new " + field_type + "[]{ ";
            for (int i = 0; i < values.Length; i++)
            {
                text += values[i];
                if (i < values.Length - 1)
                {
                    text += ", ";
                }
            }
            text += " },";
            this.line(text);
        }

        public void line(string text = "")
        {
            for (int i = 0; i < this.m_Indent; i++)
            {
                text = "\t" + text;
            }
            this.m_Lines.Add(text);
        }

        public void flush()
        {
            File.WriteAllLines(this.m_Path, this.m_Lines.ToArray());
        }
    }
}