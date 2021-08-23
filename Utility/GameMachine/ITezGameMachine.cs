using System.Collections.Generic;
using tezcat.Framework.AI;

namespace tezcat.Framework.Utility
{
    public interface ITezGameMachine<TBlackborad, TState>
        where TBlackborad : TezBaseFSMBlackboard
        where TState : TezGameMachineState<TBlackborad, TState>
    {
        void push<State>() where State : TState, new();
        void pop<State>() where State : TState, new();
        void change<State>() where State : TState, new();
        bool isThis<State>() where State : TState, new();
        Stack<TState> dump();
    }
}