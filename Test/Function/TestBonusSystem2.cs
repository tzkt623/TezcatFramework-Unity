using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public static class MyBonusConfig
    {
        public static class TypeID
        {
            public static readonly int Ship = 0;
            public static readonly int Human = 1;
        }

        public static class Ship
        {
            public static TezBonusToken Hull = TezBonusToken.createToken("Hull", TypeID.Ship, 0);
            public static TezBonusToken Armor = TezBonusToken.createToken("Armor", TypeID.Ship, 1);
            public static TezBonusToken Shield = TezBonusToken.createToken("Shield", TypeID.Ship, 2);
            public static TezBonusToken Power = TezBonusToken.createToken("Power", TypeID.Ship, 3);
        }

        public static class Human
        {
            public static TezBonusToken Health = TezBonusToken.createToken("Health", TypeID.Human, 0);
        }
    }

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
            var prto = TezcatFramework.protoDB.getProto<Ship>("Battleship");
            mShip = prto.spawnObject<Ship>();
            mShip.init();
        }

        private void showData()
        {
            Console.WriteLine($"Hull: {mShip.hull.value}/{mShip.hullCapacity.value}");
            Console.WriteLine($"Armor: {mShip.armor.value}/{mShip.armorCapacity.value}");
            Console.WriteLine($"Shield: {mShip.shield.value}/{mShip.shieldCapacity.value}");
            Console.WriteLine($"Power: {mShip.power.value}/{mShip.powerCapacity.value}");
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
                modifyType = (byte)TezBonusModifierType.Base_SumAdd,
                bonusToken = MyBonusConfig.Ship.Hull,
                value = 10
            };

            TezBonusModifier modifier2 = new TezBonusModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_SumAdd,
                bonusToken = MyBonusConfig.Ship.Armor,
                value = 20
            };

            TezBonusModifier modifier3 = new TezBonusModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_SumAdd,
                bonusToken = MyBonusConfig.Ship.Shield,
                value = 30
            };

            TezBonusModifier modifier4 = new TezBonusModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_SumAdd,
                bonusToken = MyBonusConfig.Ship.Power,
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
                modifyType = (byte)TezBonusModifierType.Base_PercentAdd,
                bonusToken = MyBonusConfig.Ship.Hull,
                value = 2
            };

            TezBonusModifier modifier6 = new TezBonusModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_PercentAdd,
                bonusToken = MyBonusConfig.Ship.Power,
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
            var proto = TezcatFramework.protoDB.getProto<Ship>("Battleship");
            mShip = proto.spawnObject<Ship>();
            mShip.init();
        }

        public override void run()
        {
            var hull_capacity = mShip.bonusableValueArray.get<TezBonusableInt>(MyDescriptorConfig.ShipPorperty.HullCapacity);
            Console.WriteLine($"{hull_capacity.name}: {hull_capacity.value}");

            var hull = mShip.litPropertyArray.get<TezValueInt>(MyDescriptorConfig.ShipValue.Hull);
            Console.WriteLine($"{hull.name}: {hull.value}");
        }

        protected override void onClose()
        {
            mShip.close();
            mShip = null;
        }
    }
}