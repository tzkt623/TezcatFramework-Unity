using System.Collections.Generic;

namespace tezcat
{
    public class TezFactory<Type, Base>
    {
        public class Line
        {
            public string name { get; private set; }
            public Type type { get; private set; }

            TezEventBus.Function<Base> m_Function;

            public Line(Type type, string name, TezEventBus.Function<Base> function)
            {
                this.type = type;
                this.name = name;
                m_Function = function;
            }

            public Base create()
            {
                return m_Function();
            }
        }

        Dictionary<Type, Line> m_LineWithTypeDic = new Dictionary<Type, Line>();
        Dictionary<string, Line> m_LineWithNameDic = new Dictionary<string, Line>();

        public void register(Type type, string name, TezEventBus.Function<Base> function)
        {
            var line = new Line(type, name, function);
            m_LineWithTypeDic.Add(type, line);
            m_LineWithNameDic.Add(name, line);
        }

        public bool tryGetLine(Type type, out Line line)
        {
            return m_LineWithTypeDic.TryGetValue(type, out line);
        }

        public Base create(Type type)
        {
            return m_LineWithTypeDic[type].create();
        }

        public bool tryGetLine(string name, out Line line)
        {
            return m_LineWithNameDic.TryGetValue(name, out line);
        }

        public Base create(string name)
        {
            return m_LineWithNameDic[name].create();
        }

        public Type convertToType(string name)
        {
            return m_LineWithNameDic[name].type;
        }

        public string convertToName(Type type)
        {
            return m_LineWithTypeDic[type].name;
        }
    }
}

