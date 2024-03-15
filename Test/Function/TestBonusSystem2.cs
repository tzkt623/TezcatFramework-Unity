using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class MyBonusTokens
    {
        public enum TypeID : int
        {
            Ship = 0,
        }

        public static TezBonusToken BToken_ShipHull = TezBonusTokenManager.createGlobalToken("Hull", TypeID.Ship, 0);
        public static TezBonusToken BToken_ShipArmor = TezBonusTokenManager.createGlobalToken("Armor", TypeID.Ship, 1);
        public static TezBonusToken BToken_ShipShield = TezBonusTokenManager.createGlobalToken("Shield", TypeID.Ship, 2);
        public static TezBonusToken BToken_ShipPower = TezBonusTokenManager.createGlobalToken("Power", TypeID.Ship, 3);

        public static void init()
        {

        }
    }

    public class MyReadModifierEntry : ITezBonusReadModifierEntry
    {
        public float value => 2.5f;
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
            MyBonusTokens.init();

            var info = TezcatFramework.mainDB.getItem<Unit>("Battleship");
            mShip = info.createObject<Ship>();
            mShip.init();
        }

        public override void run()
        {
            Console.WriteLine("Before Modify");
            Console.WriteLine($"Hull : {mShip.hull.value}/{mShip.hullCapacity.value}");
            Console.WriteLine($"Armor : {mShip.armor.value}/{mShip.armorCapacity.value}");
            Console.WriteLine($"Shield : {mShip.shield.value}/{mShip.shieldCapacity.value}");
            Console.WriteLine($"Power : {mShip.power.value}/{mShip.powerCapacity.value}");

            Console.WriteLine("");
            Console.WriteLine("Value Modifier");
            TezBonusValueModifier modifier = new TezBonusValueModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_SumAdd,
                bonusToken = MyBonusTokens.BToken_ShipHull,
                value = 5
            };

            mShip.bonusSystem.addModifier(modifier);

            Console.WriteLine($"Hull : {mShip.hull.value}/{mShip.hullCapacity.value}");
            Console.WriteLine($"Armor : {mShip.armor.value}/{mShip.armorCapacity.value}");
            Console.WriteLine($"Shield : {mShip.shield.value}/{mShip.shieldCapacity.value}");
            Console.WriteLine($"Power : {mShip.power.value}/{mShip.powerCapacity.value}");

            Console.WriteLine("");
            Console.WriteLine("ReadOnly Modifier");
            TezBonusReadModifier readModifier = new TezBonusReadModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_SumAdd,
                bonusToken = MyBonusTokens.BToken_ShipArmor,
                entry = mShip.hullCapacity
            };

            mShip.bonusSystem.addModifier(readModifier);

            mShip.armorCapacity.baseValue = 5;
            mShip.hullCapacity.baseValue = 20;
            //mShip.armorCapacity.manualUpdate();
            Console.WriteLine($"Armor : {mShip.armor.value}/{mShip.armorCapacity.value}");

            Console.WriteLine("");
            Console.WriteLine("ReadOnly Modifier");
            readModifier = new TezBonusReadModifier()
            {
                owner = this,
                modifyType = (byte)TezBonusModifierType.Base_PercentAdd,
                bonusToken = MyBonusTokens.BToken_ShipArmor,
                entry = new MyReadModifierEntry()
            };

            mShip.bonusSystem.addModifier(readModifier);
            Console.WriteLine($"Armor : {mShip.armor.value}/{mShip.armorCapacity.value}");
        }

        public override void close()
        {
            mShip.close();
            mShip = null;
        }
    }
}