﻿using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    public static class MyPropertyConfig
    {
        public static class TypeID
        {
            public static short ShipProperty = TezValueDescriptor.generateTypeID("ShipProperty");
            public static short ShipValue = TezValueDescriptor.generateTypeID("ShipValue");
            public static short Modifier = TezValueDescriptor.generateTypeID("Modifier");
        }

        /// <summary>
        /// Property
        /// </summary>
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


        /// <summary>
        /// Value
        /// </summary>
        public static readonly ITezValueDescriptor Hull = TezValueDescriptor.register(TypeID.ShipValue, "Hull");
        public static readonly ITezValueDescriptor Armor = TezValueDescriptor.register(TypeID.ShipValue, "Armor");
        public static readonly ITezValueDescriptor Shield = TezValueDescriptor.register(TypeID.ShipValue, "Shield");
        public static readonly ITezValueDescriptor Power = TezValueDescriptor.register(TypeID.ShipValue, "Power");
        public static readonly ITezValueDescriptor Speed = TezValueDescriptor.register(TypeID.ShipValue, "Speed");
        public static readonly ITezValueDescriptor Electro = TezValueDescriptor.register(TypeID.ShipValue, "Electro");
        public static readonly ITezValueDescriptor Heat = TezValueDescriptor.register(TypeID.ShipValue, "Heat");
        public static readonly ITezValueDescriptor Space = TezValueDescriptor.register(TypeID.ShipValue, "Space");

        /// <summary>
        /// Modifier
        /// </summary>
        public static readonly ITezValueDescriptor PowerAdd = TezValueDescriptor.register(TypeID.Modifier, "PowerAdd");
        public static readonly ITezValueDescriptor SpeedAdd = TezValueDescriptor.register(TypeID.Modifier, "SpeedAdd");
        public static readonly ITezValueDescriptor ArmorAdd = TezValueDescriptor.register(TypeID.Modifier, "ArmorAdd");
    }

    class TestValueDescriptor : TezBaseTest
    {
        public TestValueDescriptor() : base("ValueDescriptor")
        {

        }

        public override void close()
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