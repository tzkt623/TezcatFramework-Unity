using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    public static class MyDescriptorConfig
    {
        public static class TypeID
        {
            public static readonly short ShipProperty = TezValueDescriptor.generateTypeID("ShipProperty");
            public static readonly short ShipValue = TezValueDescriptor.generateTypeID("ShipValue");
            public static readonly short Modifier = TezValueDescriptor.generateTypeID("Modifier");
            public static readonly short HumanProperty = TezValueDescriptor.generateTypeID("HumanProperty");
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

        public static void init() { }
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
