using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public static class ItemClassIndexConfig
    {
        public const int Ship = 0;
    }

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

        protected override TezItemObject copy()
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
        //public TezBonusableInt healthAdd { get; private set; } = new TezBonusableInt(MyDescriptorConfig.Modifier.HealthAdd);

        public TezBonusModifier healthAdd { get; private set; } = new TezBonusModifier(MyBonusConfig.Human.Health);

        protected override TezItemObject copy()
        {
            return new HealthPotion();
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.healthAdd.value = reader.readInt("HealthAdd");
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

        protected override void onCopyDataFrome(TezItemObject template)
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
        protected override TezItemObject copy()
        {
            return new Gun();
        }
    }

    class Axe : Weapon
    {
        protected override TezItemObject copy()
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

        protected override TezItemObject copy()
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

        protected override void onClose()
        {
            this.target?.close();
        }
    }

    abstract class Armor : Equipment
    {
        public TezValueInt armorAdd { get; private set; } = new TezValueInt(MyDescriptorConfig.Modifier.ArmorAdd);

        protected override void onCopyDataFrome(TezItemObject template)
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

    [TezPrototypeRegister("Helmet", 3)]
    class Helmet : Armor
    {
        protected override TezItemObject copy()
        {
            return new Helmet();
        }
    }

    [TezPrototypeRegister("Breastplate", 2)]
    class Breastplate : Armor
    {
        protected override TezItemObject copy()
        {
            return new Breastplate();
        }
    }

    [TezPrototypeRegister("Leg", 1)]
    class Leg : Armor
    {
        protected override TezItemObject copy()
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
        protected override TezItemObject copy()
        {
            return new Character();
        }
    }

    [TezPrototypeRegister("Ship", ItemClassIndexConfig.Ship)]
    class Ship
        : Unit
        , ITezBonusSystemHolder
    {
        //Life
        public TezLifeHolder lifeMonitor { get; } = new TezLifeHolder();

        //Property
        TezValueArray mLitPropertyArray = new TezValueArray();
        public TezValueArray litPropertyArray => mLitPropertyArray;
        public TezValueInt hull { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Hull);
        public TezValueInt armor { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Armor);
        public TezValueInt shield { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Shield);
        public TezValueInt power { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Power);

        //Bonusable
        TezBonusSystem mBonusSystem = new TezBonusSystem();
        public TezBonusSystem bonusSystem => mBonusSystem;
        TezBonusableValueArray mBonusableValueArray = new TezBonusableValueArray();
        public TezBonusableValueArray bonusableValueArray => mBonusableValueArray;


        public TezBonusableInt hullCapacity { get; private set; } = new TezBonusableInt(MyBonusConfig.Ship.Hull, MyDescriptorConfig.ShipPorperty.HullCapacity);
        public TezBonusableInt armorCapacity { get; private set; } = new TezBonusableInt(MyBonusConfig.Ship.Armor, MyDescriptorConfig.ShipPorperty.ArmorCapacity);
        public TezBonusableInt shieldCapacity { get; private set; } = new TezBonusableInt(MyBonusConfig.Ship.Shield, MyDescriptorConfig.ShipPorperty.ShieldCapacity);
        public TezBonusableInt powerCapacity { get; private set; } = new TezBonusableInt(MyBonusConfig.Ship.Power, MyDescriptorConfig.ShipPorperty.PowerCapacity);


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

        protected override void postInit()
        {
            base.postInit();
            this.setValue();
        }

        protected override void onCopyDataFrome(TezItemObject template)
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
            this.hullCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.HullCapacity.name);
            this.armorCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.ArmorCapacity.name);
            this.shieldCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.ShieldCapacity.name);
            this.powerCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.PowerCapacity.name);
        }

        private void initValue()
        {
            mLitPropertyArray.init(MyDescriptorConfig.TypeID.ShipValue);
            mLitPropertyArray.set(this.hull);
            mLitPropertyArray.set(this.armor);
            mLitPropertyArray.set(this.shield);
            mLitPropertyArray.set(this.power);
        }

        public void initBonusSystem()
        {
            this.hullCapacity.createContainer<TezBonusModifierCache>();
            this.armorCapacity.createContainer<TezBonusModifierCache>();
            this.shieldCapacity.createContainer<TezBonusModifierList>();
            this.powerCapacity.createContainer<TezBonusModifierList>();

            mBonusSystem.init(MyBonusConfig.TypeID.Ship);
            mBonusSystem.set(this.hullCapacity);
            mBonusSystem.set(this.armorCapacity);
            mBonusSystem.set(this.shieldCapacity);
            mBonusSystem.set(this.powerCapacity);

            mBonusableValueArray.init(MyDescriptorConfig.TypeID.ShipProperty);
            mBonusableValueArray.set(this.hullCapacity);
            mBonusableValueArray.set(this.armorCapacity);
            mBonusableValueArray.set(this.shieldCapacity);
            mBonusableValueArray.set(this.powerCapacity);
        }

        private void setValue()
        {
            this.hull.innerValue = this.hullCapacity.value;
            this.armor.innerValue = this.armorCapacity.value;
            this.shield.innerValue = this.shieldCapacity.value;
            this.power.innerValue = this.powerCapacity.value;
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

        protected override TezItemObject copy()
        {
            return new Ship();
        }

        protected override void onClose()
        {
            base.onClose();
            this.hull.close();
            this.armor.close();
            this.shield.close();
            this.power.close();

            this.hullCapacity.close();
            this.armorCapacity.close();
            this.shieldCapacity.close();
            this.powerCapacity.close();

            mBonusSystem.close();
            mBonusableValueArray.close();
            mLitPropertyArray.close();
            this.lifeMonitor.close();

            this.hull = null;
            this.armor = null;
            this.shield = null;
            this.power = null;

            this.hullCapacity = null;
            this.armorCapacity = null;
            this.shieldCapacity = null;
            this.powerCapacity = null;

            mBonusSystem = null;
            mBonusableValueArray = null;
            mLitPropertyArray = null;
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
            var proto = TezcatFramework.protoDB.getProto<HealthPotion>(0);

            var potion1 = proto.spawnObject<HealthPotion>();
            potion1.init();

            Console.WriteLine($"Potion1: {potion1.itemInfo.NID}, HealthAdd: {potion1.healthAdd.value}");

            var potion2 = potion1.spawnObject<HealthPotion>();
            potion2.init();

            Console.WriteLine($"Potion2: {potion2.itemInfo.NID}, HealthAdd: {potion2.healthAdd.value}");

            Console.WriteLine($"Potion1==Potion2 : {potion1 == potion2}");

            var potion3 = potion1.remodifyObject<HealthPotion>();
            potion3.init();
            Console.WriteLine($"Potion1==Potion3 : {potion1 == potion3}");

            potion1.close();
            potion2.close();
            potion3.close();

            proto = TezcatFramework.protoDB.getProto<Breastplate>(0);
            var armor1 = proto.spawnObject<Breastplate>();
            armor1.init();

            Console.WriteLine($"Armor: {armor1.itemInfo.NID}, Armor: {armor1.armorAdd}");

            armor1.close();

            var wall = new Wall();
            wall.close();
        }

        protected override void onClose()
        {

        }
    }
}