using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public abstract class TezBaseFSMState<TBlackboard>
        : ITezCloseable
        where TBlackboard : TezBaseFSMBlackboard
    {
        public abstract void enter(TBlackboard blackboard);
        public abstract void exit(TBlackboard blackboard);

        public abstract void pause(TBlackboard blackboard);
        public abstract void resume(TBlackboard blackboard);

        public abstract void execute(TBlackboard blackboard);

        public abstract bool onEvent(ITezEventData eventData);

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }
}