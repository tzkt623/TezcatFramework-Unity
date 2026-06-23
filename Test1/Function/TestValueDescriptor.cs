using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    public static class MyDescriptorConfig
    {
        public static class TypeID
        {
            public static short BaseProperty { get; private set; }
            public static short Unit { get; private set; }
            public static short Ability { get; private set; }
            public static short Resist { get; private set; }

            public static short ShipProperty { get; private set; }
            public static short ShipValue { get; private set; }
            public static short Modifier { get; private set; }
            public static short HumanProperty { get; private set; }

            public static void init()
            {
                BaseProperty = TezValueDescriptor.generateTypeID("BaseProperty");
                Unit = TezValueDescriptor.generateTypeID("P_Unit");
                Ability = TezValueDescriptor.generateTypeID("P_Ability");
                Resist = TezValueDescriptor.generateTypeID("P_Resist");

                ShipProperty = TezValueDescriptor.generateTypeID("ShipProperty");
                ShipValue = TezValueDescriptor.generateTypeID("ShipValue");
                Modifier = TezValueDescriptor.generateTypeID("Modifier");
                HumanProperty = TezValueDescriptor.generateTypeID("HumanProperty");
            }
        }

        public class Ability
        {
            public static readonly ITezValueDescriptor CD = TezValueDescriptor.register(TypeID.BaseProperty, "P_CD");
            public static readonly ITezValueDescriptor Cost = TezValueDescriptor.register(TypeID.BaseProperty, "P_Cost");
        }

        public class Unit
        {
            public static readonly ITezValueDescriptor Health = TezValueDescriptor.register(TypeID.BaseProperty, "P_Health");
            public static readonly ITezValueDescriptor Defense = TezValueDescriptor.register(TypeID.BaseProperty, "P_Defense");
            public static readonly ITezValueDescriptor Attack = TezValueDescriptor.register(TypeID.BaseProperty, "P_Attack");
            public static readonly ITezValueDescriptor Experience = TezValueDescriptor.register(TypeID.BaseProperty, "P_Experience");
        }

        public class BaseProperty
        {
            public static readonly ITezValueDescriptor Health = TezValueDescriptor.register(TypeID.BaseProperty, "Health");
            public static readonly ITezValueDescriptor Defense = TezValueDescriptor.register(TypeID.BaseProperty, "Defense");
            public static readonly ITezValueDescriptor Attack = TezValueDescriptor.register(TypeID.BaseProperty, "Attack");
            public static readonly ITezValueDescriptor Experience = TezValueDescriptor.register(TypeID.BaseProperty, "Experience");
        }

        public class Resist
        {
            public static readonly ITezValueDescriptor ResistFire = TezValueDescriptor.register(TypeID.Resist, "P_ResistFire");
            public static readonly ITezValueDescriptor ResistIce = TezValueDescriptor.register(TypeID.Resist, "P_ResistIce");
            public static readonly ITezValueDescriptor ResistWind = TezValueDescriptor.register(TypeID.Resist, "P_ResistWind");
            public static readonly ITezValueDescriptor ResistLight = TezValueDescriptor.register(TypeID.Resist, "P_ResistLight");
        }

        public static class ShipPorperty
        {
            public static readonly ITezValueDescriptor HullCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "HullCapacity");
            public static readonly ITezValueDescriptor ArmorCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "ArmorCapacity");
            public static readonly ITezValueDescriptor ShieldCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "ShieldCapacity");
            public static readonly ITezValueDescriptor ShieldRecover = TezValueDescriptor.register(TypeID.ShipProperty, "ShieldRecover");
            public static readonly ITezValueDescriptor PowerCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "PowerCapacity");
            public static readonly ITezValueDescriptor PowerGenerate = TezValueDescriptor.register(TypeID.ShipProperty, "PowerGenerate");
            public static readonly ITezValueDescriptor SpaceCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "SpaceCapacity");
            public static readonly ITezValueDescriptor ElectroCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "ElectroCapacity");
            public static readonly ITezValueDescriptor HeatCapacity = TezValueDescriptor.register(TypeID.ShipProperty, "HeatCapacity");
            public static readonly ITezValueDescriptor HeatReduce = TezValueDescriptor.register(TypeID.ShipProperty, "HeatReduce");
            public static readonly ITezValueDescriptor JumpDistance = TezValueDescriptor.register(TypeID.ShipProperty, "JumpDistance");
        }

        public static class ShipValue
        {
            public static readonly ITezValueDescriptor Hull = TezValueDescriptor.register(TypeID.ShipValue, "Hull");
            public static readonly ITezValueDescriptor Armor = TezValueDescriptor.register(TypeID.ShipValue, "Armor");
            public static readonly ITezValueDescriptor Shield = TezValueDescriptor.register(TypeID.ShipValue, "Shield");
            public static readonly ITezValueDescriptor Power = TezValueDescriptor.register(TypeID.ShipValue, "Power");
            public static readonly ITezValueDescriptor Speed = TezValueDescriptor.register(TypeID.ShipValue, "Speed");
            public static readonly ITezValueDescriptor Electro = TezValueDescriptor.register(TypeID.ShipValue, "Electro");
            public static readonly ITezValueDescriptor Heat = TezValueDescriptor.register(TypeID.ShipValue, "Heat");
            public static readonly ITezValueDescriptor Space = TezValueDescriptor.register(TypeID.ShipValue, "Space");
        }

        public static class Modifier
        {
            public static readonly ITezValueDescriptor PowerAdd = TezValueDescriptor.register(TypeID.Modifier, "PowerAdd");
            public static readonly ITezValueDescriptor SpeedAdd = TezValueDescriptor.register(TypeID.Modifier, "SpeedAdd");
            public static readonly ITezValueDescriptor ArmorAdd = TezValueDescriptor.register(TypeID.Modifier, "ArmorAdd");
            public static readonly ITezValueDescriptor HealthAdd = TezValueDescriptor.register(TypeID.Modifier, "HealthAdd");
        }

        public static class HumanProperty
        {
            public static readonly ITezValueDescriptor HealthCapacity = TezValueDescriptor.register(TypeID.HumanProperty, "HealthCapacity");
        }

        public static void init()
        {
            TypeID.init();
        }
    }

    class TestValueDescriptor : TezBaseTest
    {
        public TestValueDescriptor() : base("ValueDescriptor")
        {

        }

        protected override void onClose()
        {

        }

        public override void init()
        {

        }

        public override void run()
        {
            TezValueDescriptor.foreachName((string name, short id) =>
            {
                Console.WriteLine("=======================");
                Console.WriteLine($"TypeInfo: {name}[{id}]");
            },

            (ITezValueDescriptor descriptor) =>
            {
                Console.WriteLine($"{descriptor.name}: TypeID[{descriptor.typeID}] IndexID[{descriptor.indexID}] ID[{descriptor.ID}]");
            });
        }
    }
}