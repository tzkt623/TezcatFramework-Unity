using System;

namespace tezcat.Framework.TypeTraits
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TezFactoryClassAttribute : Attribute
    {
        public string className { get; private set; }

        public TezFactoryClassAttribute(string name)
        {
            this.className = name;
        }
    }
}