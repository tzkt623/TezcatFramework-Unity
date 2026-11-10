using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.TBSSystem
{
    /// <summary>
    /// 技能执行时间点
    /// </summary>
    public enum TezSkillExecuteTiming
    {
        /// <summary>
        /// 1.准备阶段
        /// 用于准备技能的数据
        /// 通知自身释放前置技能等
        /// </summary>
        Prepare = 0,

        /// <summary>
        /// 2.通知阶段
        /// 通知技能目标进行一些行为
        /// 比如抢先反击,被攻击前加buff等
        /// </summary>
        Notify = 1,

        /// <summary>
        /// 3.执行阶段
        /// </summary>
        Execute = 2,

        /// <summary>
        /// 4.执行结束
        /// 做一些清理
        /// 和后续通知
        /// </summary>
        End = 3,

        Count
    }

    public enum TezSkillNotifyTiming
    {
        /// <summary>
        /// 通知自己
        /// </summary>
        NotifySelf,

        /// <summary>
        /// 通知目标
        /// </summary>
        NotifyTarget,

        /// <summary>
        /// 通知环境
        /// </summary>
        NotifyEnvironment,

        /// <summary>
        /// 通知自定义1
        /// </summary>
        NotifyCustom001,

        Count,
    }

    public interface ITezSkillSystem
    {

    }

    public abstract class TezSkillAction : ITezCloseable
    {
        private TezSkillSystem mSystem = null;
        public TezSkillSystem system => mSystem;
        public event Action evtFinished;

        TezSkillExecuteTiming mSkillTiming = TezSkillExecuteTiming.Prepare;
        public TezSkillExecuteTiming skillTiming => mSkillTiming;

        public void init(TezSkillSystem skillSystem)
        {
            mSystem = skillSystem;
        }

        public TezStepGroup buildGroup()
        {
            var group = new TezStepGroup();

            group.addStep(this.stepPrepare);
            group.addStep(this.stepNotify);
            group.addStep(this.stepExecute);
            group.addStep(this.stepEnd);

            return group;
        }

        public void stepPrepare(TezStepGroup me)
        {
            this.runPhase(me, TezSkillExecuteTiming.Prepare);
            this.onPrepare(me);
        }

        public void stepNotify(TezStepGroup me)
        {
            this.runPhase(me, TezSkillExecuteTiming.Notify);
            this.onNotify(me);
        }

        public void stepExecute(TezStepGroup me)
        {
            this.runPhase(me, TezSkillExecuteTiming.Execute);
            this.onExecute(me);
        }

        public void stepEnd(TezStepGroup me)
        {
            this.runPhase(me, TezSkillExecuteTiming.End);
            this.onEnd(me);
        }

        protected abstract void onPrepare(TezStepGroup me);
        protected abstract void onNotify(TezStepGroup me);
        protected abstract void onExecute(TezStepGroup me);
        protected abstract void onEnd(TezStepGroup me);

        protected void runPhase(TezStepGroup me, TezSkillExecuteTiming timing)
        {
            mSkillTiming = timing;
        }

        public virtual void close()
        {
            this.evtFinished = null;
            mSystem = null;
        }
    }

    public interface ITezSkillData
    {
        TezTags tags { get; }
    }

    public class TezSkillData : ITezSkillData
    {
        public TezTags tags { get; } = new TezTags();
    }

    public class TezSkillContext
    {
        public bool isisCancelled = false;
        public object sourceUnit;
        public List<object> targetUnits = new List<object>();
        public object mainTargetUnit => targetUnits[0];
    }

    public class TezSkillTimingConfig
    {
        public TezSkillExecuteTiming executeTiming;
        public TezSkillNotifyTiming notifyTiming;
    }

    public class TezSkill
    {
        public string myName { get; set; }

        TezSkillSystem mSystem = null;
        public TezSkillSystem masterSystem => mSystem;
        List<TezSkillTimingConfig> mCareTimings = new List<TezSkillTimingConfig>(3);
        public List<TezSkillTimingConfig> careTimings => mCareTimings;

        public void onAdded(TezSkillSystem skillSystem)
        {
            if (mSystem != null)
            {
                throw new Exception("System Must Null");
            }

            mSystem = skillSystem;
        }

    }

    public class TezSkillSystem
    {
        class Lisener
        {
            List<HashSet<TezSkill>> mSkills = new List<HashSet<TezSkill>>((int)TezSkillNotifyTiming.Count);

            public bool addSkill(TezSkill skill, TezSkillNotifyTiming notifyTiming)
            {
                while(mSkills.Count <= (int)notifyTiming)
                {
                    mSkills.Add(null);
                }

                if (mSkills[(int)notifyTiming] == null)
                {
                    mSkills[(int)notifyTiming] = new HashSet<TezSkill>();
                }

                return mSkills[(int)notifyTiming].Add(skill);
            }

            public bool removeSkill(TezSkill skill, TezSkillNotifyTiming notifyTiming)
            {
                return mSkills[(int)notifyTiming].Remove(skill);
            }
        }

        TezStepSystem2 mStepSystem = null;


        Lisener[] mSkillLisener = new Lisener[(int)TezSkillExecuteTiming.Count];

        List<TezSkill> mSkills = new List<TezSkill>();

        public void setSystem(TezStepSystem2 system)
        {
            mStepSystem = system;
        }

        public void addAction(TezSkillAction action)
        {
            action.init(this);
            var group = action.buildGroup();
            group.evtFinished += onFinished;
            mStepSystem.runInNextFrame<TezStepSequenceExecutor>(action.buildGroup());
        }

        public void addSkill(TezSkill skill)
        {
            if(mSkills.Contains(skill))
            {
                throw new Exception("Repeat Add");
            }
            skill.onAdded(this);

            foreach (var item in skill.careTimings)
            {
                mSkillLisener[(int)item.executeTiming].addSkill(skill, item.notifyTiming);
            }

            mSkills.Add(skill);
        }

        public void remove(TezSkill skill)
        {

        }

        public TezStepGroup notify(TezSkillExecuteTiming timing)
        {
            TezStepGroup group = new TezStepGroup();
            switch (timing)
            {
                case TezSkillExecuteTiming.Prepare:

                    break;
                default:
                    break;
            }

            return group;
        }

        private void onFinished(TezStepGroupResult result)
        {
            switch (result)
            {
                case TezStepGroupResult.Success:
                    break;
                case TezStepGroupResult.Failure:
                default:
                    break;
            }
        }
    }


    public class SkillActionAttack : TezSkillAction
    {
        protected override void onEnd(TezStepGroup me)
        {
            throw new NotImplementedException();
        }

        protected override void onExecute(TezStepGroup me)
        {
            throw new NotImplementedException();
        }

        protected override void onNotify(TezStepGroup me)
        {
            throw new NotImplementedException();
        }

        protected override void onPrepare(TezStepGroup me)
        {
            me.wait<TezStepSequenceExecutor>(this.system.notify(this.skillTiming));
        }
    }
}