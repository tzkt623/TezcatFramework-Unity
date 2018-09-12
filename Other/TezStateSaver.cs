using System.Collections.Generic;


namespace tezcat
{
    public abstract class TezAbstractState
    {
        public int index { get; set; } = -1;
    }

    public class TezStateT<Content>
    {

    }

    public class TezStateSaver
    {
        List<TezAbstractState> m_StateList = new List<TezAbstractState>();

        public void saveState(TezAbstractState state)
        {
            if(state.index < 0)
            {
                state.index = m_StateList.Count;
                m_StateList.Add(state);
            }
            else
            {

            }
        }

        public void loadState(int index)
        {

        }
    }
}