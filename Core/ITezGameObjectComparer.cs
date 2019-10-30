using tezcat.Framework.ECS;

namespace tezcat.Framework.Core
{
    public interface ITezGameObjectComparer
    {
        bool sameAs(TezGameObject other_game_object);
    }
}