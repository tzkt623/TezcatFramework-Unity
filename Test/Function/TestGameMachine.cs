﻿using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    /// <summary>
    /// <para>
    /// 取代原来的inputsystem
    /// 并且将游戏的整体state与inputsystem绑定在一起
    /// 用于设计游戏的状态转换以及在不同状态下的输入
    /// </para>
    /// 
    /// <para>
    /// 利用TezGlobalState
    /// 可以做到更多中组合状态的判断
    /// 以及输入控制
    /// </para>
    /// </summary>
    public class TestGameMachine : TezBaseTest
    {
        public class State
        {
            public static readonly TezGameState<TestGameMachine>.Mask Idle = TezGameState<TestGameMachine>.createOrGet("Idle");
            public static readonly TezGameState<TestGameMachine>.Mask Fun1 = TezGameState<TestGameMachine>.createOrGet("Fun1");
        }

        public class GameMachineBlackboard : TezGameMachineBlackborad
        {
            protected override void onClose()
            {

            }

            public override void init()
            {
            }
        }

        public abstract class GameMachineState : TezGameMachineState<GameMachineBlackboard, GameMachineState>
        {
            public abstract string name { get; }

            public override void execute(GameMachineBlackboard blackboard)
            {

            }
        }

        public class GameMachineState_Idle : GameMachineState
        {
            public override string name => "Idle";

            protected override void onClose()
            {

            }

            public override void enter(GameMachineBlackboard blackboard)
            {
                TezGameState<TestGameMachine>.add(State.Idle);
                Console.WriteLine($"{this.GetType().Name} enter");
            }

            public override void exit(GameMachineBlackboard blackboard)
            {
                TezGameState<TestGameMachine>.remove(State.Idle);
                Console.WriteLine($"{this.GetType().Name} exit");
            }

            public override void pause(GameMachineBlackboard blackboard)
            {
                Console.WriteLine($"{this.GetType().Name} pause");
            }

            public override void resume(GameMachineBlackboard blackboard)
            {
                Console.WriteLine($"{this.GetType().Name} resume");
            }

            public override void execute(GameMachineBlackboard blackboard)
            {
                Console.WriteLine($"{this.GetType().Name} running......");
            }

            public override bool onEvent(ITezEventData eventData)
            {
                return false;
            }
        }

        public class GameMachineState_Fun1 : GameMachineState
        {
            public override string name => "Fun1";

            protected override void onClose()
            {

            }

            public override void enter(GameMachineBlackboard blackboard)
            {
                TezGameState<TestGameMachine>.add(State.Fun1);
                Console.WriteLine($"{this.GetType().Name} enter");
            }

            public override void exit(GameMachineBlackboard blackboard)
            {
                TezGameState<TestGameMachine>.remove(State.Fun1);
                Console.WriteLine($"{this.GetType().Name} exit");
            }

            public override void pause(GameMachineBlackboard blackboard)
            {
                Console.WriteLine($"{this.GetType().Name} exit");
            }

            public override void resume(GameMachineBlackboard blackboard)
            {
                Console.WriteLine($"{this.GetType().Name} exit");
            }

            public override bool onEvent(ITezEventData eventData)
            {
                return false;
            }

            public override void execute(GameMachineBlackboard blackboard)
            {
                Console.WriteLine($"{this.GetType().Name} running......");
            }
        }

        public class GameMachine : TezGameMachine<GameMachineBlackboard, GameMachineState>
        {

        }


        GameMachine mGameMachine = null;
        GameMachineBlackboard mBlackboard = null;

        public TestGameMachine() : base("GameMachine")
        {
        }

        public override void run()
        {
            mGameMachine.execute();

            mGameMachine.push<GameMachineState_Fun1>();
            mGameMachine.execute();
            mGameMachine.pop<GameMachineState_Fun1>();

            mGameMachine.execute();
        }

        public override void init()
        {
            mBlackboard = new GameMachineBlackboard();
            mGameMachine = new GameMachine();
            mGameMachine.setBlackboard(mBlackboard);

            mGameMachine.push<GameMachineState_Idle>();
        }

        protected override void onClose()
        {
            mGameMachine.close();
            mBlackboard.close();
        }
    }
}