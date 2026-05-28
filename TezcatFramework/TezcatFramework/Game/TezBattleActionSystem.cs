
using System;
using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    /*
     * 当一个单位触发事件时,生成一个事件,并加入管理器进行执行
     * 事件会通知所有可能的触发者,并由触发者决定是否加入事件的触发目标列表
     * 
     * 
     * 每一个动作都可以拆解成执行前和执行后两个大阶段
     * 所有技能的触发,都处于这两个阶段中,并且每一个阶段都可以有多个子阶段
     * 比如:一个技能的执行前阶段,可能包含:伤害计算,暴击计算,闪避计算等子阶段
     * 
     */

    public class TezBattleActionSystem
    {
        public enum ActionTiming
        {
            Waiting,
            Running,
            Complete,
            Fail
        }

        public class ActionPhase
        {
            public ActionTiming timing;
            public event System.Func<ActionTiming> onAction;

            public ActionTiming execute()
            {
                return onAction.Invoke();
            }
        }

        /// <summary>
        /// 一个阶段执行完后,才会继续执行下一个阶段
        /// </summary>
        public class Action
        {
            public object source;
            public List<Action> triggerTarget;
            public ActionTiming timing;
            public List<ActionPhase> actionPhases;

            private int mRunIndex = 0;

            internal Controller controller => mController;
            Controller mController = null;

            public event System.Action onNotifyTriggerTarget;


            internal void setController(Controller controller)
            {
                mController = controller;
            }

            public void notifyTriggerTarget()
            {
                onNotifyTriggerTarget?.Invoke();
            }

            public ActionTiming execute()
            {
                var timing = actionPhases[mRunIndex].timing;
                switch (timing)
                {
                    case ActionTiming.Waiting:
                        break;
                    case ActionTiming.Running:
                        actionPhases[mRunIndex].execute();
                        return ActionTiming.Running;
                    case ActionTiming.Complete:
                        return ActionTiming.Complete;
                    case ActionTiming.Fail:
                        return ActionTiming.Fail;
                }

                return ActionTiming.Fail;
            }
        }

        internal class Controller
        {
            public Stack<Action> mStack = new Stack<Action>();

            internal void pushAction(Action action)
            {
                mStack.Push(action);
            }

            internal void popAction()
            {
                mStack.Pop();
            }

            internal void add(Action action)
            {
                action.setController(this);
                mStack.Push(action);
            }

            internal bool execute()
            {
                if (mStack.Count == 0)
                {
                    return true;
                }

                var action = mStack.Peek();
                switch (action.execute())
                {
                    case ActionTiming.Waiting:
                        break;
                    case ActionTiming.Running:
                        break;
                    case ActionTiming.Complete:
                        break;
                    case ActionTiming.Fail:
                        break;
                    default:
                        break;
                }

                return false;
            }

            private List<Controller> mControllers = new List<Controller>();

            public void update()
            {
                for (int i = mControllers.Count - 1; i >= 0; i--)
                {
                    if (mControllers[i].execute())
                    {
                        mControllers.RemoveAt(i);
                    }
                }
            }

            public Action start()
            {
                var action = new Action();
                var c = new Controller();
                c.add(action);
                mControllers.Add(c);
                return action;
            }

            public Action start(Action masterAction)
            {
                var action = new Action();
                masterAction.controller.add(action);
                return action;
            }
        }
    }
}