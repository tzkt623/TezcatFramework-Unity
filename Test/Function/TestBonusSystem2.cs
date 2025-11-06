using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestBonusSystem2 : TezBaseTest
    {
        /// <summary>
        /// <see cref="Ship">See Ship Class Memeber </see>
        /// </summary>
        Ship mShip = null;

        public TestBonusSystem2() : base("New BonusSystem")
        {

        }

        public override void init()
        {
            mShip = TezcatFramework.protoDB.createObject<ShipData, Ship>("Battleship");
            mShip.init();
        }

        private void showData()
        {
            Console.WriteLine($"Hull: {mShip.protoData.hull.value}/{mShip.protoData.hullCapacity.value}");
            Console.WriteLine($"Armor: {mShip.protoData.armor.value} /{mShip.protoData.armorCapacity.value}");
            Console.WriteLine($"Shield: {mShip.protoData.shield.value} /{mShip.protoData.shieldCapacity.value}");
            Console.WriteLine($"Power: {mShip.protoData.power.value}  /{mShip.protoData.powerCapacity.value}");
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
            mShip.bonusSystem.addModifier(modifier1);
            mShip.bonusSystem.addModifier(modifier2);
            mShip.bonusSystem.addModifier(modifier3);
            mShip.bonusSystem.addModifier(modifier4);

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
            mShip.bonusSystem.addModifier(modifier5);
            mShip.bonusSystem.addModifier(modifier6);
            this.showData();


            Console.WriteLine("====Remove Modifier====");
            mShip.bonusSystem.removeModifier(modifier1);
            mShip.bonusSystem.removeModifier(modifier2);
            mShip.bonusSystem.removeModifier(modifier3);
            mShip.bonusSystem.removeModifier(modifier4);
            mShip.bonusSystem.removeModifier(modifier5);
            mShip.bonusSystem.removeModifier(modifier6);

            this.showData();
        }

        protected override void onClose()
        {
            mShip.close();
            mShip = null;
        }
    }

    public class TestValueArrayManager : TezBaseTest
    {
        Ship mShip = null;

        public TestValueArrayManager() : base("ValueArray")
        {

        }

        public override void init()
        {
            mShip = TezcatFramework.protoDB.createObject<ShipData, Ship>("Battleship");
            mShip.init();
        }

        public override void run()
        {
            var hull_capacity = mShip.bonusSystem.get<TezBonusInt>(MyDescriptorConfig.ShipPorperty.HullCapacity);
            Console.WriteLine($"{hull_capacity.name}: {hull_capacity.value}");

            var hull = mShip.valueArray.get<TezValueInt>(MyDescriptorConfig.ShipValue.Hull);
            Console.WriteLine($"{hull.name}: {hull.value}");
        }

        protected override void onClose()
        {
            mShip.close();
            mShip = null;
        }
    }
}