using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    public abstract class TezCommandSequence
    {
        int mIndex = 0;
        List<TezCommand> mCommandList = new List<TezCommand>();

        public void add(TezCommand command)
        {
            command.sequence = this;
            command.ID = mCommandList.Count;
            mCommandList.Add(command);
        }

        public void remove(TezCommand command)
        {
            var last = mCommandList[mCommandList.Count - 1];
        }

        public void nextCommand()
        {
            mCommandList[mIndex].onEnter();
        }

        public void commandExit(TezCommand command)
        {
            mIndex = command.ID + 1;
            if(mIndex >= mCommandList.Count)
            {
                mIndex = 0;
                this.onSequenceEnd();
            }
            else
            {
                mCommandList[mIndex].onEnter();
            }
        }

        protected abstract void onSequenceBegin();
        protected abstract void onSequenceEnd();
    }
}

