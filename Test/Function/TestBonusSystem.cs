using System;
using tezcat.Framework.BonusSystem;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
#if false
    /// <summary>
    /// 第一类加成写法
    /// </summary>
    [Obsolete]
    public class MyPathes
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly TezBonusToken Equipment = TezBonusTokenCreator<MyPathes>.createToken("Equipment", TezBonusTokenType.Root, null);

        public static readonly TezBonusToken Turrent = TezBonusTokenCreator<MyPathes>.createToken("Turrent", TezBonusTokenType.Path, Equipment);
        public static readonly TezBonusToken LaserTurrent = TezBonusTokenCreator<MyPathes>.createToken("LaserTurrent", TezBonusTokenType.Leaf, Turrent);

        public static readonly TezBonusToken Armor = TezBonusTokenCreator<MyPathes>.createToken("Armor", TezBonusTokenType.Path, Equipment);
        public static readonly TezBonusToken ArmorPlate = TezBonusTokenCreator<MyPathes>.createToken("ArmorPlate", TezBonusTokenType.Leaf, Armor);
        public static readonly TezBonusToken ArmorRepairer = TezBonusTokenCreator<MyPathes>.createToken("ArmorRepairer", TezBonusTokenType.Leaf, Armor);

        /// <summary>
        /// 
        /// </summary>
        public static readonly TezBonusToken Unit = TezBonusTokenCreator<MyPathes>.createToken("Unit", TezBonusTokenType.Root, null);
        public static readonly TezBonusToken Ship = TezBonusTokenCreator<MyPathes>.createToken("Ship", TezBonusTokenType.Leaf, Unit);

        /// <summary>
        /// 
        /// </summary>
        public static readonly TezBonusToken Battle = TezBonusTokenCreator<MyPathes>.createToken("Battle", TezBonusTokenType.Root, null);

        public static readonly TezBonusToken Melee = TezBonusTokenCreator<MyPathes>.createToken("Melee", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Slash = TezBonusTokenCreator<MyPathes>.createToken("Slash", TezBonusTokenType.Leaf, Melee);

        public static readonly TezBonusToken Range = TezBonusTokenCreator<MyPathes>.createToken("Range", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Shoot = TezBonusTokenCreator<MyPathes>.createToken("Shoot", TezBonusTokenType.Leaf, Range);
    }

    /// <summary>
    /// 第二类加成写法
    /// </summary>
    [Obsolete]
    public class MySkillPathes
    {
        public static readonly TezBonusToken Battle = TezBonusTokenCreator<MySkillPathes>.createToken("Battle", TezBonusTokenType.Root, null);

        public static readonly TezBonusToken Melee = TezBonusTokenCreator<MySkillPathes>.createToken("Melee", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Slash = TezBonusTokenCreator<MySkillPathes>.createToken("Slash", TezBonusTokenType.Leaf, Melee);

        public static readonly TezBonusToken Range = TezBonusTokenCreator<MySkillPathes>.createToken("Range", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Shoot = TezBonusTokenCreator<MySkillPathes>.createToken("Shoot", TezBonusTokenType.Leaf, Range);
    }

    [Obsolete]
    public class MyBountyLeaf : TezBonusTreeLeafNode
    {
        public MyBountyLeaf(int id, ITezBonusTree system) : base(id, system)
        {

        }
    }

    [Obsolete]
    public class MyBountyPath : TezBonusTreePathNode
    {
        public MyBountyPath(int id, ITezBonusTree tree) : base(id, tree)
        {

        }
    }

    [Obsolete]
    public class MyBountyTree : TezBonusTree<TezBonusTreeListContainer>
    {
        protected override TezBonusTreeNode onCreateNode(ITezBonusToken token)
        {
            switch (token.tokenType)
            {
                case TezBonusTokenType.Root:
                case TezBonusTokenType.Path:
                    return new MyBountyPath(token.tokenID, this);
                case TezBonusTokenType.Leaf:
                    return new MyBountyLeaf(token.tokenID, this);
                default:
                    throw new Exception();
            }
        }
    }

    [Obsolete]
    public class MyIntBonus : ITezBonusCarrier
    {
        public string name;
        public TezBonusPath bonusPath { get; set; }
        public int value = 5;
    }

    [Obsolete]
    public class MyPropertyBonus : TezValueModifier
    {

    }

    [Obsolete]
    public class MyLaserTurrent : ITezBonusAgentEntry
    {
        public TezBonusAgent bonusAgent { get; } = new TezBonusAgent();

        public int attack = 10;
        public int power = 10;
        public int cooldown = 10;

        public void init()
        {
            bonusAgent.setPath(TezBonusTokenCreator<MyPathes>.getPath(MyPathes.LaserTurrent));
            bonusAgent.setListener(((ITezBonusAgentEntry)this).onAddBonusObject, ((ITezBonusAgentEntry)this).onRemoveBonusObject);
        }

        public void log()
        {
            Console.WriteLine($"Attack: {attack}");
            Console.WriteLine($"Power: {power}");
            Console.WriteLine($"Cooldown: {cooldown}");
        }

        public void close()
        {
            this.bonusAgent.close();
        }

        void ITezBonusAgentEntry.onAddBonusObject(ITezBonusCarrier obj)
        {
            MyIntBonus mobj = (MyIntBonus)obj;
            if (mobj.name == "attack")
            {
                this.attack += mobj.value;
            }

            if (mobj.name == "power")
            {
                this.power += mobj.value;
            }

            if (mobj.name == "cooldown")
            {
                this.cooldown += mobj.value;
            }
        }

        void ITezBonusAgentEntry.onRemoveBonusObject(ITezBonusCarrier obj)
        {
            MyIntBonus mobj = (MyIntBonus)obj;
            if (mobj.name == "attack")
            {
                this.attack -= mobj.value;
            }

            if (mobj.name == "power")
            {
                this.power -= mobj.value;
            }

            if (mobj.name == "cooldown")
            {
                this.cooldown -= mobj.value;
            }
        }
    }

    [Obsolete]
    public class MyShip : ITezBonusAgentEntry
    {
        MyBountyTree tree = new MyBountyTree();
        MyLaserTurrent laserTurrent = new MyLaserTurrent();

        public MyPropertyManager propertyManager = new MyPropertyManager();
        MyPropertyInt hullCapacity = null;
        MyPropertyInt armorCapacity = null;
        MyPropertyInt shieldCapacity = null;

        public TezBonusAgent bonusAgent { get; } = new TezBonusAgent();

        public void init()
        {
            this.laserTurrent.init();

            this.bonusAgent.setPath(TezBonusTokenCreator<MyPathes>.getPath(MyPathes.Ship));
            this.bonusAgent.setListener(((ITezBonusAgentEntry)this).onAddBonusObject, ((ITezBonusAgentEntry)this).onRemoveBonusObject);

            tree.registerAgent(this.bonusAgent);
            tree.registerAgent(this.laserTurrent.bonusAgent);

            this.hullCapacity = this.propertyManager.getOrCreate<MyPropertyInt>(MyPropertyConfig.HullCapacity);
            this.hullCapacity.baseValue = 100;

            this.armorCapacity = this.propertyManager.getOrCreate<MyPropertyInt>(MyPropertyConfig.ArmorCapacity);
            this.armorCapacity.baseValue = 300;

            this.shieldCapacity = this.propertyManager.getOrCreate<MyPropertyInt>(MyPropertyConfig.ShieldCapacity);
            this.shieldCapacity.baseValue = 50;
        }

        public void log()
        {
            Console.WriteLine($"{this.hullCapacity.name}: {this.hullCapacity.value}");
            Console.WriteLine($"{this.armorCapacity.name}: {this.armorCapacity.value}");
            Console.WriteLine($"{this.shieldCapacity.name}: {this.shieldCapacity.value}");
        }

        public void addBonus()
        {
            tree.addBonusObject(new MyIntBonus()
            {
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.LaserTurrent),
                name = "attack",
                value = 5
            });

            tree.addBonusObject(new MyIntBonus()
            {
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.LaserTurrent),
                name = "power",
                value = -2
            });

            tree.addBonusObject(new MyIntBonus()
            {
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.LaserTurrent),
                name = "cooldown",
                value = -1
            });

            laserTurrent.log();
        }

        public void addModifierBonus()
        {
            tree.addBonusObject(new MyPropertyBonus()
            {
                modifierConfig = new TezValueModifierConfig()
                {
                    assemble = TezValueModifierAssembleConfig.SumBase,
                    target = MyPropertyConfig.HullCapacity
                },
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.Ship),
                source = this,
                value = 30
            });

            tree.addBonusObject(new MyPropertyBonus()
            {
                modifierConfig = new TezValueModifierConfig()
                {
                    assemble = TezValueModifierAssembleConfig.SumBase,
                    target = MyPropertyConfig.ArmorCapacity
                },
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.Ship),
                source = this,
                value = -50
            });

            tree.addBonusObject(new MyPropertyBonus()
            {
                modifierConfig = new TezValueModifierConfig()
                {
                    assemble = TezValueModifierAssembleConfig.SumBase,
                    target = MyPropertyConfig.ShieldCapacity
                },
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.Ship),
                source = this,
                value = 10
            });

            this.log();
        }

        public void close()
        {
            this.tree.close();
            this.bonusAgent.close();
            this.laserTurrent.close();
            this.propertyManager.close();
        }

        void ITezBonusAgentEntry.onRemoveBonusObject(ITezBonusCarrier carrier)
        {
            this.propertyManager.removeModifier((ITezValueModifier)carrier);
        }

        void ITezBonusAgentEntry.onAddBonusObject(ITezBonusCarrier carrier)
        {
            this.propertyManager.addModifier((ITezValueModifier)carrier);
        }
    }

    [Obsolete]
    class TestBonusSystem : TezBaseTest
    {
        MyShip myUnit = null;

        public TestBonusSystem() : base("BonusSystem")
        {
        }

        protected override void onClose()
        {
            myUnit.close();
            myUnit = null;
        }

        public override void init()
        {
            myUnit = new MyShip();
            myUnit.init();
        }

        public override void run()
        {
            myUnit.addBonus();
            myUnit.addModifierBonus();

        }
    }
#endif
}