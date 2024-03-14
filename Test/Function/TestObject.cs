using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    class Wall : TezGameObject
    {
        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);
        }
    }

    #region Useable
    abstract class Useable : TezItemObject
    {

    }

    abstract class Potion : Useable
    {

    }

    class MagicPotion : Potion
    {
        public int magicAdd;

        protected override ITezItemObject copy()
        {
            return new MagicPotion();
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.magicAdd = reader.readInt("MagicAdd");
        }
    }

    class HealthPotion : Potion
    {
        public int healthAdd;

        protected override ITezItemObject copy()
        {
            return new HealthPotion();
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.healthAdd = reader.readInt("HealthAdd");
        }
    }
    #endregion

    #region Equipment
    abstract class Equipment : TezItemObject
    {

    }

    abstract class Weapon : Equipment
    {
        public int attack;

        protected override void onCopyDataFrome(ITezItemObject template)
        {
            base.onCopyDataFrome(template);
            this.attack = ((Weapon)template).attack;
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.attack = reader.readInt("Attack");
        }
    }

    class Gun : Weapon
    {
        protected override ITezItemObject copy()
        {
            return new Gun();
        }
    }

    class Axe : Weapon
    {
        protected override ITezItemObject copy()
        {
            return new Axe();
        }
    }

    class Missle : Weapon
    {
        public string name = null;
        public int step = 0;
        public TezLifeMonitor target = null;
        bool stop = false;

        protected override ITezItemObject copy()
        {
            return new Missle();
        }

        public void update()
        {
            if (stop)
            {
                return;
            }

            if (this.target == null)
            {
                this.findOtherTarget();
                return;
            }

            if (this.target.isValied)
            {
                this.moveToTarget();
            }
            else
            {
                this.target.close();
                this.target = null;
            }
        }

        public void setTarget(Ship ship)
        {
            this.target = new TezLifeMonitor();
            this.target.create(ship.lifeMonitor);
        }

        private void moveToTarget()
        {
            Console.WriteLine($"{name}: Move To Target......");
            step--;
            if (step == 0)
            {
                this.target.tryGetObject<Ship>(out var ship);
                ship.hull.value = 0;
                Console.WriteLine($"{name} hit target!!");
                this.target.close();
                this.target = null;
                this.stop = true;
            }
        }

        private void findOtherTarget()
        {
            Console.WriteLine($"{name}: Find Other Target......");
        }

        public void close()
        {
            this.target?.close();
        }
    }

    abstract class Armor : Equipment
    {
        public TezLitPropertyInt armorAdd { get; private set; } = new TezLitPropertyInt(MyPropertyConfig.ArmorAdd);

        protected override void onCopyDataFrome(ITezItemObject template)
        {
            base.onCopyDataFrome(template);
            this.armorAdd.innerValue = ((Armor)template).armorAdd.value;
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            armorAdd.innerValue = reader.readInt("ArmorAdd");
        }
    }

    class ArmorPlate : Armor
    {

    }

    class Helmet : Armor
    {
        protected override ITezItemObject copy()
        {
            return new Helmet();
        }
    }

    class Breastplate : Armor
    {
        protected override ITezItemObject copy()
        {
            return new Breastplate();
        }
    }

    class Leg : Armor
    {
        protected override ITezItemObject copy()
        {
            return new Leg();
        }
    }
    #endregion

    #region Unit
    abstract class Unit : TezItemObject
    {

    }

    class Character : Unit
    {
        public int health;
        public int armor;

        protected override ITezItemObject copy()
        {
            return new Character();
        }
    }

    class Ship 
        : Unit
        , ITezBonusSystemHolder
    {
        public TezLifeHolder lifeMonitor { get; } = new TezLifeHolder();

        TezBonusSystem mBonusSystem = new TezBonusSystem();
        public TezBonusSystem bonusSystem => mBonusSystem;

        //Property
        public TezLitPropertyInt hull { get; private set; } = new TezLitPropertyInt(MyPropertyConfig.Hull);
        public TezLitPropertyInt armor { get; private set; } = new TezLitPropertyInt(MyPropertyConfig.Armor);
        public TezLitPropertyInt shield { get; private set; } = new TezLitPropertyInt(MyPropertyConfig.Shield);
        public TezLitPropertyInt power { get; private set; } = new TezLitPropertyInt(MyPropertyConfig.Power);

        //Bonusable
        public TezBonusableInt hullCapacity { get; private set; } = new TezBonusableInt(MyPropertyConfig.HullCapacity);
        public TezBonusableInt armorCapacity { get; private set; } = new TezBonusableInt(MyPropertyConfig.ArmorCapacity);
        public TezBonusableInt shieldCapacity { get; private set; } = new TezBonusableInt(MyPropertyConfig.ShieldCapacity);
        public TezBonusableInt powerCapacity { get; private set; } = new TezBonusableInt(MyPropertyConfig.PowerCapacity);

        public Ship()
        {
            this.lifeMonitor.create(this);
            //default value
            this.hullCapacity.baseValue = 5;
            this.armorCapacity.baseValue = 34;
            this.shieldCapacity.baseValue = 8;
            this.powerCapacity.baseValue = 50;
        }

        protected override void preInit()
        {
            base.preInit();
            this.initBonusSystem();
        }

        protected override void onInit()
        {
            base.onInit();
            this.initValue();
        }

        protected override void onCopyDataFrome(ITezItemObject template)
        {
            base.onCopyDataFrome(template);
            Ship data = (Ship)template;
            this.hullCapacity.baseValue = data.hullCapacity.baseValue;
            this.armorCapacity.baseValue = data.armorCapacity.baseValue;
            this.shieldCapacity.baseValue = data.shieldCapacity.baseValue;
            this.powerCapacity.baseValue = data.powerCapacity.baseValue;
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.hullCapacity.baseValue = reader.readInt(MyPropertyConfig.HullCapacity.name);
            this.armorCapacity.baseValue = reader.readInt(MyPropertyConfig.ArmorCapacity.name);
            this.shieldCapacity.baseValue = reader.readInt(MyPropertyConfig.ShieldCapacity.name);
            this.powerCapacity.baseValue = reader.readInt(MyPropertyConfig.PowerCapacity.name);
        }

        private void initValue()
        {
            this.hull.innerValue = this.hullCapacity.value;
            this.armor.innerValue = this.armorCapacity.value;
            this.shield.innerValue = this.shieldCapacity.value;
            this.power.innerValue = this.powerCapacity.value;
        }

        public void initBonusSystem()
        {
            mBonusSystem.init(TezBonusTokenManager.getTokenCapacity((int)MyBonusTokens.TypeID.Ship));

            mBonusSystem.register<TezBonusModifierContainer>(this.hullCapacity, MyBonusTokens.BToken_ShipHull);
            mBonusSystem.register<TezBonusModifierContainer>(this.armorCapacity, MyBonusTokens.BToken_ShipArmor);
            mBonusSystem.register<TezBonusModifierContainer>(this.shieldCapacity, MyBonusTokens.BToken_ShipShield);
            mBonusSystem.register<TezBonusModifierContainer>(this.powerCapacity, MyBonusTokens.BToken_ShipPower);
        }

        public void update()
        {
            if (this.lifeMonitor.isValied)
            {
                if (this.hull.value == 0)
                {
                    Console.WriteLine("Ship Dead");
                    this.lifeMonitor.setInvalid();
                }
            }
        }

        protected override ITezItemObject copy()
        {
            return new Ship();
        }

        public void close()
        {
            this.hull.close();

            this.hullCapacity.close();
            this.armorCapacity.close();
            this.shieldCapacity.close();
            this.powerCapacity.close();

            mBonusSystem.close();
            this.lifeMonitor.close();

            this.hull = null;

            this.hullCapacity = null;
            this.armorCapacity = null;
            this.shieldCapacity = null;
            this.powerCapacity = null;

            mBonusSystem = null;
        }
    }
    #endregion

    class TestObject : TezBaseTest
    {
        public TestObject() : base("Objects")
        {

        }

        public override void init()
        {

        }

        public override void run()
        {
            //var item_info = TezcatFramework.mainDB.getItem(0, 0);
            var item_info = TezcatFramework.mainDB.getItem<Potion>(0);

            var potion1 = item_info.createObject<HealthPotion>();
            potion1.init();

            Console.WriteLine($"Potion1: {potion1.itemInfo.NID}, HealthAdd: {potion1.healthAdd}");

            var potion2 = potion1.duplicate<HealthPotion>();
            potion2.init();

            Console.WriteLine($"Potion2: {potion2.itemInfo.NID}, HealthAdd: {potion2.healthAdd}");

            Console.WriteLine($"Potion1==Potion2 : {potion1 == potion2}");

            var potion3 = potion1.remodify<HealthPotion>();
            potion3.init();
            Console.WriteLine($"Potion1==Potion3 : {potion1 == potion3}");

            potion1.close();
            potion2.close();
            potion3.close();

            item_info = TezcatFramework.mainDB.getItem<Armor>(0);
            var armor1 = item_info.createObject<Breastplate>();
            armor1.init();

            Console.WriteLine($"Armor: {armor1.itemInfo.NID}, Armor: {armor1.armorAdd}");

            armor1.close();

            var wall = new Wall();
            wall.close();
        }

        public override void close()
        {

        }
    }
}