using tezcat.Framework.Event;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

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
            public static readonly uint Idle = TezGlobalState.createOrGet("Idle");
            public static readonly uint Fun1 = TezGlobalState.createOrGet("Fun1");
        }

        public class GameMachineBlackboard : TezGameMachineBlackborad
        {
            public override void close()
            {
            }

            public override void init()
            {
            }
        }

        public abstract class GameMachineState : TezGameMachineState<GameMachineBlackboard, GameMachineState>
        {
            public bool hoverUI => EventSystem.current.IsPointerOverGameObject();
            public abstract string name { get; }

            public override void execute(GameMachineBlackboard blackboard)
            {

            }
        }

        public class GameMachineState_Idle : GameMachineState
        {
            public override string name => "Idle";

            public override void close()
            {

            }

            public override void enter(GameMachineBlackboard blackboard)
            {
                TezGlobalState.add(State.Idle);
            }

            public override void exit(GameMachineBlackboard blackboard)
            {
                TezGlobalState.remove(State.Idle);
            }

            public override void execute(GameMachineBlackboard blackboard)
            {

            }

            public override bool onEvent(ITezEventData eventData)
            {
                return false;
            }
        }

        public class GameMachineState_Fun1 : GameMachineState
        {
            public override string name => "Fun1";
            int m_MeshLayerMask = -1;

            public override void close()
            {

            }

            public override void enter(GameMachineBlackboard blackboard)
            {
                if (m_MeshLayerMask == -1)
                {
                    m_MeshLayerMask = 1 << LayerMask.NameToLayer("Fun1");
                }
                TezGlobalState.add(State.Fun1);
            }

            public override void exit(GameMachineBlackboard blackboard)
            {
                TezGlobalState.remove(State.Fun1);
            }

            public override bool onEvent(ITezEventData eventData)
            {
                return false;
            }

            public override void execute(GameMachineBlackboard blackboard)
            {
                if (this.hoverUI)
                {
                    return;
                }

                if (Input.GetMouseButtonUp(1))
                {
                    this.gameMachine.pop<GameMachineState_Fun1>();
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Camera.main.farClipPlane, m_MeshLayerMask))
                {
                    Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
                }
            }
        }

        public class GameMachine : TezGameMachine<GameMachineBlackboard, GameMachineState>
        {

        }


        GameMachine mGameMachine = new GameMachine();

        public TestGameMachine() : base("GameMachine")
        {
            mGameMachine.setBlackboard(new GameMachineBlackboard());
        }

        public override void run()
        {
            mGameMachine.push<GameMachineState_Idle>();
            mGameMachine.execute();
            mGameMachine.pop<GameMachineState_Idle>();

            mGameMachine.push<GameMachineState_Fun1>();
            mGameMachine.execute();
        }
    }
}

