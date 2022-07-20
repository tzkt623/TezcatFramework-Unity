using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.ECS
{
    /// <summary>
    /// 请给每一个component按如下写一个注册流程
    /// 
    /// <para>class Component</para>
    /// <para>{</para>
    /// <para>声明一个静态变量用于标识Component的静态ID</para>
    /// <para>public static int SComUID;</para>
    /// <para>重写</para>
    /// <para>int comUID { get; } = SComUID</para>
    /// <para>}</para>
    /// 
    /// </summary>
    public interface ITezComponent : ITezCloseable
    {
        TezEntity entity { get; }
        int comUID { get; }
        void onAdd(TezEntity entity);
        void onRemove(TezEntity entity);
        void onOtherComponentAdded(ITezComponent component, int comID);
        void onOtherComponentRemoved(ITezComponent component, int comID);
    }

    public interface ITezObjectWithItem
    {
        void initWithData(ITezSerializableItem item);
    }
}