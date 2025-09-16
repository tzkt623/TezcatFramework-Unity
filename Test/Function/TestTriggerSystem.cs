using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestTriggerSystem : TezBaseTest
    {
        enum Camp
        {
            Error,
            Player,
            Enmey
        }

        enum DamageType
        {
            NormalDamage,
            FireDamage,
            IceDamage,
            WindDamage,
            LightningDamage,
        }

        enum TimingPhase : byte
        {
            Empty = 0,

            BeforeExecute,
            AfterExecute,
        }

        enum TriggerPhase : byte
        {
            Empty = 0,
            PhaseBefore,
            PhaseAfter,
            PhaseUnitDead,
        }

        enum EffectPhase : byte
        {
            Empty = 0,

            NormalDamage,
            FireDamage,
            IceDamage,
            WindDamage,
            LightningDamage,
            Healing,

            UnitBorn,
            UnitDead,
        }

        enum Notifier : byte
        {
            AnyOne = 0,
            Self,
            NotSelf,
        }

        enum NotifyWhoPhase : byte
        {
            Empty = 0,

            AnyOne,
            Self,
            Friend,
            Rival
        }

        /// <summary>
        /// PhaseID
        /// 
        /// 触发时机:执行前和执行后
        /// 触发效果:各种效果
        /// 
        /// 注册的时候需要注册技能自己的触发
        /// registerSkill(PhaseID, CurrentSkill)
        /// 
        /// 通知触发的时候
        /// notifySkill(PhaseID, MasterSkill)
        /// 
        /// 
        /// 当玩家受到伤害后触发(指注册所有伤害类型)
        /// 当玩家受到火焰伤害后触发
        /// 
        /// Skill
        /// 主动型:不需要注册触发,只需要发出阶段通知就行了
        /// 
        /// 被动型:需要注册当前技能的触发条件
        /// 
        /// 混合型:既可以主动使用,也可以被动触发
        /// 
        /// 考虑触发条件的完善机制
        /// 也就是循环触发到什么时候中止
        /// 
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct TriggerPhaseID : ITezTriggerPhaseID
        {
            [FieldOffset(0)]
            ulong mID;

            [FieldOffset(0)]
            byte mEffectPhase;
            public EffectPhase effectPhase
            {
                set { mEffectPhase = (byte)value; }
            }

            [FieldOffset(1)]
            byte mNotifier;
            public Notifier notifier
            {
                set { mNotifier = (byte)value; }
            }

            [FieldOffset(2)]
            byte mTriggerPhase;
            public TriggerPhase triggerPhase
            {
                set { mTriggerPhase = (byte)value; }
            }

            [FieldOffset(3)]
            byte mActionPhase;
            public TimingPhase timingPhase
            {
                set { mActionPhase = (byte)value; }
            }

            public ulong ID => mID;

            public void setID(ulong ID)
            {
                mID = ID;
            }
        }

        static class Helper
        {
            public static void writeLine(Camp camp, string str)
            {
                switch (camp)
                {
                    case Camp.Error:
                        break;
                    case Camp.Player:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case Camp.Enmey:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        break;
                }

                Console.WriteLine(str);
            }
        }

        class Damage
        {
            public DamageType damageType;
            public int damageValue;
        }

        abstract class Skill
        {
            public class TriggerFunction
            {
                public TriggerPhaseID phaseID;
                public Action<TezTrigger<UserData>> funcCreate = null;
            }

            public Player owner;
            public string name;
            public bool actived = false;
            public EffectPhase effectPhase = EffectPhase.Empty;

            List<TriggerFunction> mTriggerListener = null;
            public IReadOnlyCollection<TriggerFunction> triggerListener => mTriggerListener;

            public void registerTrigger(TriggerPhaseID phaseID, Action<TezTrigger<UserData>> funcCreate)
            {
                if (mTriggerListener == null)
                {
                    mTriggerListener = new List<TriggerFunction>();
                }
                mTriggerListener.Add(new TriggerFunction() { phaseID = phaseID, funcCreate = funcCreate });
            }

            public Camp getRival(Camp camp)
            {
                switch (camp)
                {
                    case Camp.Player:
                        return Camp.Enmey;
                    case Camp.Enmey:
                        return Camp.Player;
                    default:
                        break;
                }

                return Camp.Error;
            }

            protected void trigger(TezTrigger<UserData> masterTrigger, Action<TezTrigger<UserData>> funcExecute)
            {
                if (this.actived)
                {
                    return;
                }

                this.actived = true;
                Helper.writeLine(this.owner.camp, $"{this.name} Prepare to Active");

                new TezTrigger<UserData>()
                {
                    userData = this.owner.userData
                }
                .addPhase((me) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: Active");
                    Helper.writeLine(this.owner.camp, $"{this.name}: BeforeAction1");
                    var phase_id = new TriggerPhaseID()
                    {
                        effectPhase = this.effectPhase,
                        timingPhase = TimingPhase.BeforeExecute,
                        triggerPhase = TriggerPhase.PhaseBefore,
                        notifier = Notifier.Self
                    };
                    me.phaseID = phase_id;

                    this.owner.notify(phase_id, me);
                })
                .addPhase((me) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: BeforeAction2");
                    var phase_id = new TriggerPhaseID()
                    {
                        effectPhase = this.effectPhase,
                        timingPhase = TimingPhase.BeforeExecute,
                        triggerPhase = TriggerPhase.PhaseBefore,
                        notifier = Notifier.NotSelf
                    };
                    me.phaseID = phase_id;
                    this.owner.gameManager.notify(this.getRival(this.owner.camp), phase_id, me);
                })
                .addPhase((me) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: Playing Animation......");
                    this.owner.playAnimation(() => { me.resume(); });
                })
                .addPhase(funcExecute)
                .addPhase((me) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: AfterAction1");
                    var phase_id = new TriggerPhaseID()
                    {
                        effectPhase = this.effectPhase,
                        timingPhase = TimingPhase.AfterExecute,
                        triggerPhase = TriggerPhase.PhaseAfter,
                        notifier = Notifier.NotSelf
                    };
                    me.phaseID = phase_id;
                    this.owner.gameManager.notify(this.getRival(this.owner.camp), phase_id, me);
                })
                .addPhase((me) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: AfterAction2");
                    var phase_id = new TriggerPhaseID()
                    {
                        effectPhase = this.effectPhase,
                        timingPhase = TimingPhase.AfterExecute,
                        triggerPhase = TriggerPhase.PhaseAfter,
                        notifier = Notifier.Self
                    };
                    me.phaseID = phase_id;
                    this.owner.notify(phase_id, me);
                })
                .setOnComplete((state) =>
                {
                    this.actived = false;
                    Helper.writeLine(this.owner.camp, $"{this.name}: Completed {state}");
                })
                .run(masterTrigger);
            }

            public virtual void trigger(TezTrigger<UserData> masterTrigger)
            {
                throw new NotImplementedException();
            }

            public virtual void execute()
            {
                throw new NotImplementedException();
            }
        }

        class SkillAttack : Skill
        {
            public Player target {  get; set; }

            public SkillAttack()
            {
                this.name = "Attack";
                this.effectPhase = EffectPhase.NormalDamage;
            }

            public override void execute()
            {
                Helper.writeLine(this.owner.camp, $"{this.owner.name} Use Skill {this.name}");
                this.trigger(null, (trigger) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: Execute......");
                    this.target.takeDamage(trigger, this.owner.damage);
                });
            }
        }

        class SkillAttackBack : Skill
        {
            public SkillAttackBack()
            {
                this.name = "AttackBack";

                //此技能只能被
                //  正常伤害
                //  执行后阶段
                //  执行后触发器
                //  非自己
                //的效果触发
                this.registerTrigger(new TriggerPhaseID()
                {
                    effectPhase = EffectPhase.NormalDamage,
                    timingPhase = TimingPhase.AfterExecute,
                    triggerPhase = TriggerPhase.PhaseAfter,
                    notifier = Notifier.NotSelf
                }
                , this.trigger);
            }

            public override void trigger(TezTrigger<UserData> masterTrigger)
            {
                this.trigger(masterTrigger, (trigger) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: Execute......");
                });
            }
        }

        class SkillFirstAttack : Skill
        {
            public SkillFirstAttack()
            {
                this.name = "FirstAttack";

                //此技能只能被
                //  正常伤害
                //  执行前阶段
                //  执行前触发器
                //  非自己
                //的效果触发
                this.registerTrigger(new TriggerPhaseID()
                {
                    effectPhase = EffectPhase.NormalDamage,
                    timingPhase = TimingPhase.BeforeExecute,
                    triggerPhase = TriggerPhase.PhaseBefore,
                    notifier = Notifier.NotSelf,
                }
                , this.trigger);

                this.registerTrigger(new TriggerPhaseID()
                {
                    effectPhase = EffectPhase.IceDamage,
                    timingPhase = TimingPhase.BeforeExecute,
                    triggerPhase = TriggerPhase.PhaseBefore,
                    notifier = Notifier.NotSelf,
                }
                , this.trigger);

                this.registerTrigger(new TriggerPhaseID()
                {
                    effectPhase = EffectPhase.FireDamage,
                    timingPhase = TimingPhase.BeforeExecute,
                    triggerPhase = TriggerPhase.PhaseBefore,
                    notifier = Notifier.NotSelf,
                }
                , this.trigger);

                this.registerTrigger(new TriggerPhaseID()
                {
                    effectPhase = EffectPhase.WindDamage,
                    timingPhase = TimingPhase.BeforeExecute,
                    triggerPhase = TriggerPhase.PhaseBefore,
                    notifier = Notifier.NotSelf,
                }
                , this.trigger);

                this.registerTrigger(new TriggerPhaseID()
                {
                    effectPhase = EffectPhase.LightningDamage,
                    timingPhase = TimingPhase.BeforeExecute,
                    triggerPhase = TriggerPhase.PhaseBefore,
                    notifier = Notifier.NotSelf,
                }
                , this.trigger);
            }

            public override void trigger(TezTrigger<UserData> masterTrigger)
            {
                this.trigger(masterTrigger, (trigger) =>
                {
                    Helper.writeLine(this.owner.camp, $"{this.name}: Execute......");
                    //masterTrigger.fail();
                });
            }
        }

        class SkillManager
        {
            Player mMaster = null;

            Dictionary<ulong, List<Skill.TriggerFunction>> mTriggerSkills = new Dictionary<ulong, List<Skill.TriggerFunction>>();
            Dictionary<string, Skill> mAllSkils = new Dictionary<string, Skill>();


            public void setMaster(Player master)
            {
                mMaster = master;
            }

            public void addSkill(Skill skill)
            {
                if (mAllSkils.ContainsKey(skill.name))
                {
                    return;
                }

                skill.owner = mMaster;
                mAllSkils.Add(skill.name, skill);
                if (skill.triggerListener != null)
                {
                    foreach (var item in skill.triggerListener)
                    {
                        if (mTriggerSkills.TryGetValue(item.phaseID.ID, out var list))
                        {
                            list.Add(item);
                        }
                        else
                        {
                            list = new List<Skill.TriggerFunction>
                            {
                                item
                            };
                            mTriggerSkills.Add(item.phaseID.ID, list);
                        }
                    }
                }
            }

            public void notify(TriggerPhaseID phaseID, TezTrigger<UserData> masterTrigger)
            {
                if (mTriggerSkills.TryGetValue(phaseID.ID, out var list))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].funcCreate(masterTrigger);
                    }
                }
            }

            public Skill getSkill(string v)
            {
                if (mAllSkils.TryGetValue(v, out var skill))
                {
                    return skill;
                }

                return null;
            }
        }

        class Player
        {
            public Camp camp { get; set; }
            public GameManager gameManager { get; set; }
            public string name { get; set; }
            public UserData userData { get; set; }
            public int health { get; set; }
            public int damage { get; set; }

            SkillManager mSkillManager = new SkillManager();
            public SkillManager skillManager => mSkillManager;

            public Player()
            {
                mSkillManager.setMaster(this);
                this.userData = new UserData()
                {
                    self = this
                };
            }

            public void addSkill(Skill skill)
            {
                mSkillManager.addSkill(skill);
            }

            public void notify(TriggerPhaseID phaseID, TezTrigger<UserData> masterTrigger)
            {
                Helper.writeLine(this.camp, $"{this.name}: Notify Skill");
                mSkillManager.notify(phaseID, masterTrigger);
            }

            public void playAnimation(Action function)
            {
                function();
            }

            public void doDamage(Player target)
            {
                Helper.writeLine(this.camp, $"Damage Target {this.name}=>{target.name}");
            }

            public void attack(Player target)
            {
                SkillAttack skill = (SkillAttack)mSkillManager.getSkill("Attack");
                skill.target = target;
                skill.execute();
            }

            public void takeDamage(TezTrigger<UserData> masterTrigger, int damage)
            {
                this.health -= damage;
                if (this.health < 0)
                {
                    mSkillManager.notify(new TriggerPhaseID()
                    {
                        effectPhase = EffectPhase.UnitDead,
                        timingPhase = TimingPhase.AfterExecute,
                        triggerPhase = TriggerPhase.PhaseUnitDead,
                        notifier = Notifier.Self,
                    },
                    masterTrigger);
                }
                else
                {
                    mSkillManager.notify(new TriggerPhaseID()
                    {
                        effectPhase = EffectPhase.NormalDamage,
                        timingPhase = TimingPhase.AfterExecute,
                        triggerPhase = TriggerPhase.PhaseAfter,
                        notifier = Notifier.Self,
                    },
                    masterTrigger);
                }
            }
        }

        class UserData
        {
            public Player target;
            public Player self;
        }

        class GameManager
        {
            List<Player> mPlayerList = new List<Player>();
            List<Player> mEnemyList = new List<Player>();

            private void notifyPlayer(TriggerPhaseID phaseID, TezTrigger<UserData> masterTrigger)
            {
                for (int i = 0; i < mPlayerList.Count; i++)
                {
                    mPlayerList[i].notify(phaseID, masterTrigger);
                }
            }

            private void notifyEnemy(TriggerPhaseID phaseID, TezTrigger<UserData> masterTrigger)
            {
                for (int i = 0; i < mEnemyList.Count; i++)
                {
                    mEnemyList[i].notify(phaseID, masterTrigger);
                }
            }

            public void notify(Camp camp, TriggerPhaseID phaseID, TezTrigger<UserData> masterTrigger)
            {
                switch (camp)
                {
                    case Camp.Player:
                        this.notifyPlayer(phaseID, masterTrigger);
                        break;
                    case Camp.Enmey:
                        this.notifyEnemy(phaseID, masterTrigger);
                        break;
                    default:
                        break;
                }
            }

            public void add(Player player)
            {
                switch (player.camp)
                {
                    case Camp.Player:
                        this.addPlayer(player);
                        break;
                    case Camp.Enmey:
                        this.addEnemy(player);
                        break;
                    default:
                        break;
                }
            }

            private void addPlayer(Player player)
            {
                player.gameManager = this;
                mPlayerList.Add(player);
            }

            private void addEnemy(Player player)
            {
                player.gameManager = this;
                mEnemyList.Add(player);
            }
        }

        GameManager mGameManager = new GameManager();
        Player player = null;
        Player enemy = null;

        public TestTriggerSystem() : base("TriggerSystem")
        {

        }

        public override void init()
        {
            this.player = new Player()
            {
                name = "Player",
                camp = Camp.Player,
                health = 3,
                damage = 5
            };

            this.player.addSkill(new SkillAttack());
            this.player.addSkill(new SkillAttackBack());
            mGameManager.add(this.player);


            this.enemy = new Player()
            {
                name = "Enemy",
                camp = Camp.Enmey,
                health = 10,
                damage = 6
            };

            this.enemy.addSkill(new SkillAttack());
            this.enemy.addSkill(new SkillFirstAttack());
            mGameManager.add(this.enemy);
        }

        public override void run()
        {
            Action action = () => { this.player.attack(this.enemy); };

            Console.WriteLine("Input Any Key except Escape to next step");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                if (action != null)
                {
                    action();
                    action = null;
                }

                TezTriggerSystem.update();
            }
        }

        protected override void onClose()
        {

        }
    }
}
