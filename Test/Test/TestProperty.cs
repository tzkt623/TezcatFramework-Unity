using tezcat.Framework.BonusSystem;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    class MyPropertyConfig
    {
        public static readonly ITezValueDescriptor HullCapacity = TezValueDescriptor<MyPropertyConfig>.register("HullCapacity");
        public static readonly ITezValueDescriptor Hull = TezValueDescriptor<MyPropertyConfig>.register("Hull");
        public static readonly ITezValueDescriptor ArmorCapacity = TezValueDescriptor<MyPropertyConfig>.register("ArmorCapacity");
        public static readonly ITezValueDescriptor Armor = TezValueDescriptor<MyPropertyConfig>.register("Armor");
        public static readonly ITezValueDescriptor ArmorAdd = TezValueDescriptor<MyPropertyConfig>.register("ArmorAdd");
        public static readonly ITezValueDescriptor ShieldCapacity = TezValueDescriptor<MyPropertyConfig>.register("ShieldCapacity");
        public static readonly ITezValueDescriptor Shield = TezValueDescriptor<MyPropertyConfig>.register("Shield");
        public static readonly ITezValueDescriptor ShieldRecover = TezValueDescriptor<MyPropertyConfig>.register("ShieldRecover");
        public static readonly ITezValueDescriptor PowerCapacity = TezValueDescriptor<MyPropertyConfig>.register("PowerCapacity");
        public static readonly ITezValueDescriptor PowerGenerate = TezValueDescriptor<MyPropertyConfig>.register("PowerGenerate");
        public static readonly ITezValueDescriptor PowerAdd = TezValueDescriptor<MyPropertyConfig>.register("PowerAdd");
        public static readonly ITezValueDescriptor Power = TezValueDescriptor<MyPropertyConfig>.register("Power");
        public static readonly ITezValueDescriptor SpaceCapacity = TezValueDescriptor<MyPropertyConfig>.register("SpaceCapacity");
        public static readonly ITezValueDescriptor Speed = TezValueDescriptor<MyPropertyConfig>.register("Speed");
        public static readonly ITezValueDescriptor SpeedAdd = TezValueDescriptor<MyPropertyConfig>.register("SpeedAdd");
        public static readonly ITezValueDescriptor ElectroCapacity = TezValueDescriptor<MyPropertyConfig>.register("ElectroCapacity");
        public static readonly ITezValueDescriptor Electro = TezValueDescriptor<MyPropertyConfig>.register("Electro");
        public static readonly ITezValueDescriptor HeatCapacity = TezValueDescriptor<MyPropertyConfig>.register("HeatCapacity");
        public static readonly ITezValueDescriptor Heat = TezValueDescriptor<MyPropertyConfig>.register("Heat");
        public static readonly ITezValueDescriptor Space = TezValueDescriptor<MyPropertyConfig>.register("Space");
        public static readonly ITezValueDescriptor HeatReduce = TezValueDescriptor<MyPropertyConfig>.register("HeatReduce");
        public static readonly ITezValueDescriptor JumpDistance = TezValueDescriptor<MyPropertyConfig>.register("JumpDistance");
    }

    class MyValueModifierAssembleConfig : TezValueModifierAssembleConfig
    {
        public const byte my1 = BuildInHold + 1;
        public const byte my2 = BuildInHold + 2;
        public const byte my3 = BuildInHold + 3;
    }

    class MyIntModifierCache : TezBaseValueModifierCache<int>
    {
        protected float m_SumBase = 0;
        protected float m_SumTotal = 0;
        protected float m_MultBase = 0;
        protected float m_MultTotal = 0;

        public override int calculate(ITezProperty<int> property)
        {
            float result = (property.baseValue + m_SumBase) + property.baseValue * m_MultBase;
            result = result + m_SumTotal + result * (1 + m_MultTotal);
            return (int)System.Math.Round(result);
        }

        protected override void onModifierChanged(ITezValueModifier valueModifier, float oldValue)
        {
            var config = valueModifier.modifierConfig;
            switch (config.assemble)
            {
                case TezValueModifierAssembleConfig.SumBase:
                    m_SumBase = m_SumBase - oldValue + valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.SumTotal:
                    m_SumTotal = m_SumTotal - oldValue + valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.MultBase:
                    m_MultBase = m_MultBase - oldValue + valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.MultTotal:
                    m_MultTotal = m_MultTotal - oldValue + valueModifier.value;
                    break;
                case MyValueModifierAssembleConfig.my1:
                    break;
                default:
                    break;
            }
        }

        protected override void onModifierAdded(ITezValueModifier valueModifier)
        {
            var config = valueModifier.modifierConfig;
            switch (config.assemble)
            {
                case TezValueModifierAssembleConfig.SumBase:
                    m_SumBase += valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.SumTotal:
                    m_SumTotal += valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.MultBase:
                    m_MultBase += valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.MultTotal:
                    m_MultTotal += valueModifier.value;
                    break;
                case MyValueModifierAssembleConfig.my1:
                    break;
                default:
                    break;
            }
        }

        protected override void onModifierRemoved(ITezValueModifier valueModifier)
        {
            var config = valueModifier.modifierConfig;
            switch (config.assemble)
            {
                case TezValueModifierAssembleConfig.SumBase:
                    m_SumBase -= valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.SumTotal:
                    m_SumTotal -= valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.MultBase:
                    m_MultBase -= valueModifier.value;
                    break;
                case TezValueModifierAssembleConfig.MultTotal:
                    m_MultTotal -= valueModifier.value;
                    break;
                case MyValueModifierAssembleConfig.my1:
                    break;
                default:
                    break;
            }
        }
    }

    class MyIntModifierCacheMinMax : MyIntModifierCache
    {
        int m_Min;
        int m_Max;

        public void setMinMax(int min, int max)
        {
            m_Min = min;
            m_Max = max;
            this.dirty = true;
        }

        public override int calculate(ITezProperty<int> property)
        {
            float result = (property.baseValue + m_SumBase) * (1 + m_MultTotal);

            var temp = (int)System.Math.Round(result);
            if (temp > m_Max)
            {
                temp = m_Max;
            }
            else if (temp < m_Min)
            {
                temp = m_Min;
            }

            return temp;
        }
    }

    class MyPropertyInt : TezProperty<int>
    {
        public MyPropertyInt() : base(null, new MyIntModifierCache())
        {

        }
    }

    class MyModifier : TezValueModifier
    {

    }

    class MyAgentModifier : TezAgentValueModifier
    {
        public MyAgentModifier(ITezProperty property) : base(property)
        {

        }
    }

    class MyPropertyManager : TezPropertyManager<TezPropertyListContainer>
    {

    }

    class MyArmorPlate
    {
        public MyPropertyManager propertyManager = new MyPropertyManager();
        public TezBonusAgent agent = new TezBonusAgent();
        TezBonusPath bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.ArmorPlate);

        MyPropertyInt armorCapacity = null;

        public void init()
        {
            this.agent.setPath(this.bonusPath);
            this.agent.setListener(this.onAddBonusObject, this.onRemoveBonusObject);

            this.armorCapacity = this.propertyManager.getOrCreate<MyPropertyInt>(MyPropertyConfig.ArmorCapacity);
        }

        private void onRemoveBonusObject(ITezBonusObject arg)
        {
            this.propertyManager.removeModifier((MyModifier)arg);
        }

        private void onAddBonusObject(ITezBonusObject arg)
        {
            this.propertyManager.addModifier((MyModifier)arg);
        }
    }


    class MyUnit2
    {
        public MyBountyTree myTree = new MyBountyTree();

        MyArmorPlate myArmorPlate = new MyArmorPlate();

        public void init()
        {
            myArmorPlate.init();
            myTree.registerAgent(myArmorPlate.agent);
        }
    }

    class TestProperty
    {
        MyUnit2 myUnit2 = new MyUnit2();
        MyPropertyInt extraArmorPlate = new MyPropertyInt();


        public void test()
        {
            this.extraArmorPlate = new MyPropertyInt()
            {
                descriptor = MyPropertyConfig.ArmorCapacity
            };

            MyModifier myModifier = new MyModifier()
            {
                modifierConfig = new TezValueModifierConfig()
                {
                    assemble = MyValueModifierAssembleConfig.SumBase,
                    target = MyPropertyConfig.ArmorCapacity
                },
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.ArmorPlate),
                source = this
            };

            MyAgentModifier myAgentModifier = new MyAgentModifier(this.extraArmorPlate)
            {
                modifierConfig = new TezValueModifierConfig()
                {
                    assemble = MyValueModifierAssembleConfig.SumBase,
                    target = MyPropertyConfig.ArmorCapacity
                },
                bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.ArmorPlate),
                source = this
            };

            myUnit2.init();
            myUnit2.myTree.addBonusObject(myModifier);
            myUnit2.myTree.addBonusObject(myAgentModifier);
        }
    }
}
