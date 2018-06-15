using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace tezcat.Game
{
    public abstract class TezCommandSequence
    {
        int m_Index = 0;
        List<TezCommand> m_CommandList = new List<TezCommand>();

        public void add(TezCommand command)
        {
            command.sequence = this;
            command.ID = m_CommandList.Count;
            m_CommandList.Add(command);
        }

        public void remove(TezCommand command)
        {
            var last = m_CommandList[m_CommandList.Count - 1];
        }

        public void nextCommand()
        {
            m_CommandList[m_Index].onEnter();
        }

        public void commandExit(TezCommand command)
        {
            m_Index = command.ID + 1;
            if(m_Index >= m_CommandList.Count)
            {
                m_Index = 0;
                this.onSequenceEnd();
            }
            else
            {
                m_CommandList[m_Index].onEnter();
            }
        }

        protected abstract void onSequenceBegin();
        protected abstract void onSequenceEnd();
    }
}

