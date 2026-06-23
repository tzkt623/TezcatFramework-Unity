using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    public interface ITezGameMachine<TBlackboard, TState>
        where TBlackboard : TezBaseFSMBlackboard
        where TState : TezGameMachineState<TBlackboard, TState>
    {
        TBlackboard blackboard { get; }
        State push<State>() where State : TState, new();
        void pop<State>() where State : TState, new();
        void change<State>() where State : TState, new();
        bool isThis<State>() where State : TState, new();
        Stack<TState> dump();
    }
}