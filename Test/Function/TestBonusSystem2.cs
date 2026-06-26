using System;
using tezcat.Framework.ArchetypeECS;
using tezcat.Framework.Core;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestBonusSystem2 : TezBaseTest
    {
        /// <summary>
        /// <see cref="Ship">See Ship Class Memeber </see>
        /// </summary>
        TezWorld.Entity mShip;

        public TestBonusSystem2() : base("New BonusSystem")
        {

        }

        public override void init()
        {
            var data = TezcatFramework.protoDB.createObjectData<ShipData>("Battleship", TezProtoObjectCreateMode.New);
            mShip = data.instantiateEntity();
        }

        private void showData()
        {
            var data = TezWorld.instance.getComponent<ComUnitData, ShipData>(mShip);
            Console.WriteLine($"Hull: {data.hull.value}/{data.hullCapacity.value}");
            Console.WriteLine($"Armor: {data.armor.value} /{data.armorCapacity.value}");
            Console.WriteLine($"Shield: {data.shield.value} /{data.shieldCapacity.value}");
            Console.WriteLine($"Power: {data.power.value}  /{data.powerCapacity.value}");
            Console.WriteLine("");
        }

        public override void run()
        {
            Console.WriteLine("====Before Modify====");
            this.showData();

            Console.WriteLine("====Add Modifier====");
            TezBonusModifier modifier1 = new TezBonusModifier()
            {
                owner = this,
                calculateRule = (byte)TezBonusModifierCalculateRule.Base_SumAdd,
                valueDescriptor = MyDescriptorConfig.ShipPorperty.HullCapacity,
                value = 10
            };

            TezBonusModifier modifier2 = new TezBonusModifier()
            {
                owner = this,
                calculateRule = (byte)TezBonusModifierCalculateRule.Base_SumAdd,
                valueDescriptor = MyDescriptorConfig.ShipPorperty.ArmorCapacity,
                value = 20
            };

            TezBonusModifier modifier3 = new TezBonusModifier()
            {
                owner = this,
                calculateRule = (byte)TezBonusModifierCalculateRule.Base_SumAdd,
                valueDescriptor = MyDescriptorConfig.ShipPorperty.ShieldCapacity,
                value = 30
            };

            TezBonusModifier modifier4 = new TezBonusModifier()
            {
                owner = this,
                calculateRule = (byte)TezBonusModifierCalculateRule.Base_SumAdd,
                valueDescriptor = MyDescriptorConfig.ShipPorperty.PowerCapacity,
                value = 40
            };

            Console.WriteLine(modifier1);
            Console.WriteLine(modifier2);
            Console.WriteLine(modifier3);
            Console.WriteLine(modifier4);

            var ship = TezWorld.instance.getComponent<ComUnit, Ship>(mShip);
            ship.bonusSystem.addModifier(modifier1);
            ship.bonusSystem.addModifier(modifier2);
            ship.bonusSystem.addModifier(modifier3);
            ship.bonusSystem.addModifier(modifier4);

            this.showData();

            Console.WriteLine("====Add Modifier====");
            TezBonusModifier modifier5 = new TezBonusModifier()
            {
                owner = this,
                calculateRule = (byte)TezBonusModifierCalculateRule.Base_PercentAdd,
                valueDescriptor = MyDescriptorConfig.ShipPorperty.HullCapacity,
                value = 2
            };

            TezBonusModifier modifier6 = new TezBonusModifier()
            {
                owner = this,
                calculateRule = (byte)TezBonusModifierCalculateRule.Base_PercentAdd,
                valueDescriptor = MyDescriptorConfig.ShipPorperty.PowerCapacity,
                value = 3
            };

            Console.WriteLine(modifier5);
            Console.WriteLine(modifier6);
            ship.bonusSystem.addModifier(modifier5);
            ship.bonusSystem.addModifier(modifier6);
            this.showData();


            Console.WriteLine("====Remove Modifier====");
            ship.bonusSystem.removeModifier(modifier1);
            ship.bonusSystem.removeModifier(modifier2);
            ship.bonusSystem.removeModifier(modifier3);
            ship.bonusSystem.removeModifier(modifier4);
            ship.bonusSystem.removeModifier(modifier5);
            ship.bonusSystem.removeModifier(modifier6);

            this.showData();
        }

        protected override void onClose()
        {
            TezWorld.instance.removeEntity(mShip);
        }
    }

    public class TestValueArrayManager : TezBaseTest
    {
        TezWorld.Entity mShip;

        public TestValueArrayManager() : base("ValueArray")
        {

        }

        public override void init()
        {
            var data = TezcatFramework.protoDB.createObjectData<ShipData>("Battleship", TezProtoObjectCreateMode.New);
            mShip = data.instantiateEntity();
        }

        public override void run()
        {
            var ship = TezWorld.instance.getComponent<ComUnit, Ship>(mShip);

            var hull_capacity = ship.bonusSystem.get<TezBonusInt>(MyDescriptorConfig.ShipPorperty.HullCapacity);
            Console.WriteLine($"{hull_capacity.name}: {hull_capacity.value}");

            var hull = ship.valueArray.get<TezValueInt>(MyDescriptorConfig.ShipValue.Hull);
            Console.WriteLine($"{hull.name}: {hull.value}");
        }

        protected override void onClose()
        {
            TezWorld.instance.removeEntity(mShip);
        }
    }
}