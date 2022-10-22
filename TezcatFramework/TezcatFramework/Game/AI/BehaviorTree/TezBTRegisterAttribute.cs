using System;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// BTTree属性定义类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TezBTRegisterAttribute : Attribute
    {
        public string name;
    }
}
