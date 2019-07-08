using tezcat.MyRPG;
using UnityEngine;

public class CMDTest : MonoBehaviour
{
    CMDController controller = new CMDController();
    TObj1 obj1 { get; set; }
    TObj2 obj2 { get; set; }

    class BattleObject
    {
        public CMDTest tester { get; set; }
        public string name { get; set; }

        public BattleObject()
        {
            this.name = this.GetType().Name;
        }

        public void attack(BattleObject defenser)
        {
            Debug.Log(name + " : 攻击阶段");

            var cmd = CMD.create(CMD.State.Running);
            cmd.setOnExecute((CMD me) =>
            {
                Debug.Log(name + " : 播放攻击动画");
                me.state = CMD.State.Complete;
            });

            cmd.setOnComplete((CMD me) =>
            {
                Debug.Log(name + " : 传送攻击数据");
                me.controller.newQueue();
                defenser.defense(this);
            });
            tester.controller.post(cmd);
        }

        public void defense(BattleObject attacker)
        {
            Debug.Log(name + " : 防御阶段");

            var cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                this.beforeEffect(attacker);
            });
            tester.controller.post(cmd);
        }

        protected virtual void onBeforeEffect(BattleObject attacker)
        {

        }

        public void beforeEffect(BattleObject attacker)
        {
            Debug.Log(name + " : 伤害发生前阶段");

            var cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                this.onBeforeEffect(attacker);
            });
            tester.controller.post(cmd);

            cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                var rate = Random.value;
                if (rate > 0.5f)
                {
                    Debug.Log(string.Format("{0} : {1} 攻击成功", name, attacker.name));
                    this.effect(attacker);
                }
                else
                {
                    Debug.Log(string.Format("{0} : {1} 攻击失败", name, attacker.name));
                }
            });
            tester.controller.post(cmd);
        }

        public void effect(BattleObject attacker)
        {
            Debug.Log(name + " : 伤害发生阶段");

            var cmd = CMD.create(CMD.State.Running);
            cmd.setOnExecute((CMD me) =>
            {
                Debug.Log(name + " : 播放掉血效果");
                me.state = CMD.State.Complete;
            });
            tester.controller.post(cmd);

            cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                this.afterEffect(attacker);
            });
            tester.controller.post(cmd);
        }

        public void afterEffect(BattleObject attacker)
        {
            Debug.Log(name + " : 伤害发生后阶段");
            var cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                this.onAfterEffect(attacker);
            });
            tester.controller.post(cmd);
        }

        public virtual void onAfterEffect(BattleObject attacker)
        {

        }
    }

    class TObj1 : BattleObject
    {
        protected override void onBeforeEffect(BattleObject attacker)
        {

        }

        public override void onAfterEffect(BattleObject attacker)
        {
            var cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                this.repair();
            });
            tester.controller.post(cmd);

            cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                var rate = Random.value;
                if (rate > 0.5f)
                {
                    Debug.Log(string.Format("{0} : 反击成功", name));
                    this.attackBack(attacker);
                }
                else
                {
                    Debug.Log(string.Format("{0} : 反击失败", name));
                }
            });
            tester.controller.post(cmd);
        }

        private void attackBack(BattleObject attacker)
        {
            Debug.Log(this.name + ": 反击");
            this.attack(attacker);
        }

        private void repair()
        {
            var cmd = CMD.create(CMD.State.Running);
            cmd.setOnExecute((CMD me) =>
            {
                Debug.Log(this.name + ": 播放修理效果");
                me.state = CMD.State.Complete;
            });

            cmd.setOnComplete((CMD me) =>
            {
                Debug.Log(this.name + " : 修理");
                me.controller.newQueue();
            });
            tester.controller.post(cmd);
        }
    }

    class TObj2 : BattleObject
    {
        protected override void onBeforeEffect(BattleObject attacker)
        {

        }

        public override void onAfterEffect(BattleObject attacker)
        {
            var cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                this.repair();
            });
            tester.controller.post(cmd);

            cmd = CMD.create();
            cmd.setOnComplete((CMD me) =>
            {
                me.controller.newQueue();
                var rate = Random.value;
                if (rate > 0.5f)
                {
                    Debug.Log(string.Format("{0} : 反击成功", name));
                    this.attackBack(attacker);
                }
                else
                {
                    Debug.Log(string.Format("{0} : 反击失败", name));
                }
            });
            tester.controller.post(cmd);
        }

        private void attackBack(BattleObject attacker)
        {
            Debug.Log(this.name + ": 反击");
            this.attack(attacker);
        }

        private void repair()
        {
            var cmd = CMD.create(CMD.State.Running);
            cmd.setOnExecute((CMD me) =>
            {
                Debug.Log(this.name + ": 播放修理效果");
                me.state = CMD.State.Complete;
            });

            cmd.setOnComplete((CMD me) =>
            {
                Debug.Log(this.name + " : 修理");
                me.controller.newQueue();
            });
            tester.controller.post(cmd);
        }
    }

    void Start()
    {
        this.obj1 = new TObj1() { tester = this };
        this.obj2 = new TObj2() { tester = this };
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F5))
        {
            obj1.attack(obj2);
        }

        this.controller.execute();
    }
}