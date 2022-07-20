﻿using System.Collections.Generic;
using tezcat.Framework.AI;

namespace tezcat.Framework.Utility
{
    public interface ITezGameMachine<TBlackboard, TState>
        where TBlackboard : TezBaseFSMBlackboard
        where TState : TezGameMachineState<TBlackboard, TState>
    {
        TBlackboard blackboard { get; }
        void push<State>() where State : TState, new();
        void pop<State>() where State : TState, new();
        void change<State>() where State : TState, new();
        bool isThis<State>() where State : TState, new();
        Stack<TState> dump();
    }
}