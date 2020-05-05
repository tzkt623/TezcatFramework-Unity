using tezcat.Framework.ECS;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// GameObject的比较器
    /// </summary>
    public interface ITezGameObjectComparer
    {
        /// <summary>
        /// 比较
        /// </summary>
        bool sameAs(TezGameObject other_game_object);
    }
}