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
        public override void deserialize(TezSaveController.Reader reader)
        {
            base.deserialize(reader);
        }
    }

    #region Useable
    class MagicPotionData : TezProtoObjectData/*继承ProtoData*/
    {
        public int magicAdd;

        protected override void onInit()
        {

        }

        protected override TezProtoObject createObjectInternal()
        {
            return new MagicPotion();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            return new MagicPotionData
            {
                magicAdd = this.magicAdd
            };
        }

        protected override void onDeserializeObjectData(TezSaveController.Reader reader)
        {
            this.magicAdd = reader.readInt("MagicAdd");
        }

        protected override void onSerializeObjectData(TezSaveController.Writer writer)
        {
            throw new NotImplementedException();
        }
    }

    class HealthPotionData : TezProtoObjectData
    {
        public TezBonusModifier healthAdd { get; private set; } = new TezBonusModifier(MyDescriptorConfig.HumanProperty.HealthCapacity);

        protected override void onInit()
        {

        }

        protected override TezProtoObject createObjectInternal()
        {
            return new HealthPotion();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            var data = new HealthPotionData();
            data.healthAdd.value = this.healthAdd.value;
            return data;
        }

        protected override void onDeserializeObjectData(TezSaveController.Reader reader)
        {
            this.healthAdd.value = reader.readInt("HealthAdd");
        }

        protected override void onSerializeObjectData(TezSaveController.Writer writer)
        {
            throw new NotImplementedException();
        }
    }

    abstract class Useable : TezProtoObject
    {

    }

    abstract class Potion : Useable
    {

    }

    class MagicPotion : Potion, ITezProtoObject<MagicPotionData>/*指明protodata类*/
    {
        public MagicPotionData protoData => (MagicPotionData)mProtoData;

        public override void initProtoData(TezProtoObjectData data)
        {
            base.initProtoData(data);
        }

        protected override TezProtoObjectData createProtoData()
        {
            return new MagicPotionData();
        }
    }

    class HealthPotion : Potion, ITezProtoObject<HealthPotionData>
    {
        //public TezBonusableInt healthAdd { get; private set; } = new TezBonusableInt(MyDescriptorConfig.Modifier.HealthAdd);

        public HealthPotionData protoData => (HealthPotionData)mProtoData;
        public TezBonusModifier healthAdd => this.protoData.healthAdd;

        protected override TezProtoObjectData createProtoData()
        {
            return new HealthPotionData();
        }
    }
    #endregion

    #region Equipment
    abstract class WeaponData : TezProtoObjectData
    {
        public int attack;

        protected override void onInit()
        {

        }

        protected override void onDeserializeObjectData(TezSaveController.Reader reader)
        {
            this.attack = reader.readInt("Attack");
        }

        protected override void onSerializeObjectData(TezSaveController.Writer writer)
        {
            throw new NotImplementedException();
        }
    }

    class GunData : WeaponData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new Gun();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            return new GunData
            {
                attack = this.attack
            };
        }
    }

    class AxeData : WeaponData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new Axe();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            return new AxeData
            {
                attack = this.attack
            };
        }
    }

    class MissleData : WeaponData
    {
        public string name = null;
        public int step = 0;
        protected override TezProtoObject createObjectInternal()
        {
            return new Missle();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            return new MissleData
            {
                attack = this.attack,
                name = this.name,
                step = 0
            };
        }
    }

    abstract class Equipment : TezProtoObject
    {

    }

    abstract class Weapon : Equipment
    {

    }

    class Gun : Weapon, ITezProtoObject<GunData>
    {
        public GunData protoData => (GunData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new GunData();
        }
    }

    class Axe : Weapon, ITezProtoObject<AxeData>
    {
        public AxeData protoData => (AxeData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new AxeData();
        }
    }

    class Missle : Weapon, ITezProtoObject<MissleData>
    {
        public string name = null;
        public int step = 0;
        public TezLifeMonitor target = null;
        bool stop = false;

        public MissleData protoData => (MissleData)mProtoData;

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
            this.target.create(ship.lifeHolder);
        }

        private void moveToTarget()
        {
            Console.WriteLine($"{name}: Move To Target......");
            step--;
            if (step == 0)
            {
                var ship = this.target.getObject<Ship>();
                ship.protoData.hull.value = 0;
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

        protected override TezProtoObjectData createProtoData()
        {
            return new MissleData();
        }
    }


    abstract class ArmorData : TezProtoObjectData
    {
        public TezValueInt armorAdd { get; private set; } = new TezValueInt(MyDescriptorConfig.Modifier.ArmorAdd);

        protected override void onInit()
        {

        }

        protected override void onDeserializeObjectData(TezSaveController.Reader reader)
        {
            this.armorAdd.value = reader.readInt("ArmorAdd");
        }

        protected override void onSerializeObjectData(TezSaveController.Writer writer)
        {
            throw new NotImplementedException();
        }
    }

    class ArmorPlateData : ArmorData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new ArmorPlate();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            var data = new ArmorPlateData();
            data.armorAdd.value = this.armorAdd.value;
            return data;
        }
    }

    abstract class Armor : Equipment
    {

    }

    class ArmorPlate : Armor, ITezProtoObject<ArmorPlateData>
    {
        public ArmorPlateData protoData => (ArmorPlateData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new ArmorPlateData();
        }
    }

    class HelmetData : ArmorData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new Helmet();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            var data = new HelmetData();
            data.armorAdd.value = this.armorAdd.value;
            return data;
        }
    }

    class BreastplateData : ArmorData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new Breastplate();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            var data = new BreastplateData();
            data.armorAdd.value = this.armorAdd.value;
            return data;
        }
    }

    class LegData : ArmorData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new Leg();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            var data = new LegData();
            data.armorAdd.value = this.armorAdd.value;
            return data;
        }
    }

    [TezPrototypeRegister("Helmet", 3)]
    class Helmet : Armor, ITezProtoObject<HelmetData>
    {
        public HelmetData protoData => (HelmetData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new HelmetData();
        }
    }

    [TezPrototypeRegister("Breastplate", 2)]
    class Breastplate : Armor, ITezProtoObject<BreastplateData>
    {
        public BreastplateData protoData =>(BreastplateData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new BreastplateData();
        }
    }

    [TezPrototypeRegister("Leg", 1)]
    class Leg : Armor, ITezProtoObject<LegData>
    {
        public LegData protoData =>(LegData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new LegData();
        }
    }
    #endregion

    #region Unit
    abstract class UnitData : TezProtoObjectData
    {
        protected override void onInit()
        {

        }
    }

    class CharacterData : UnitData
    {
        protected override TezProtoObject createObjectInternal()
        {
            return new Character();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            return new CharacterData();
        }

        protected override void onDeserializeObjectData(TezSaveController.Reader reader)
        {

        }

        protected override void onSerializeObjectData(TezSaveController.Writer writer)
        {
            throw new NotImplementedException();
        }
    }

    class ShipData : UnitData
    {
        public TezValueInt hull { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Hull);
        public TezValueInt armor { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Armor);
        public TezValueInt shield { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Shield);
        public TezValueInt power { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Power);

        public TezBonusableInt hullCapacity { get; private set; } = new TezBonusableInt(MyDescriptorConfig.ShipPorperty.HullCapacity);
        public TezBonusableInt armorCapacity { get; private set; } = new TezBonusableInt(MyDescriptorConfig.ShipPorperty.ArmorCapacity);
        public TezBonusableInt shieldCapacity { get; private set; } = new TezBonusableInt(MyDescriptorConfig.ShipPorperty.ShieldCapacity);
        public TezBonusableInt powerCapacity { get; private set; } = new TezBonusableInt(MyDescriptorConfig.ShipPorperty.PowerCapacity);

        protected override TezProtoObject createObjectInternal()
        {
            return new Ship();
        }

        protected override void onClose()
        {
            this.hull.close();
            this.armor.close();
            this.shield.close();
            this.power.close();

            this.hullCapacity.close();
            this.armorCapacity.close();
            this.shieldCapacity.close();
            this.powerCapacity.close();

            this.hull = null;
            this.armor = null;
            this.shield = null;
            this.power = null;

            this.hullCapacity = null;
            this.armorCapacity = null;
            this.shieldCapacity = null;
            this.powerCapacity = null;

            base.onClose();
        }

        protected override TezProtoObjectData copySelfWithOutItemInfo()
        {
            var data = new ShipData();
            data.hull.value = this.hull.value;
            data.armor.value = this.armor.value;
            data.shield.value = this.shield.value;
            data.power.value = this.power.value;
            data.hullCapacity.baseValue = this.hullCapacity.baseValue;
            data.armorCapacity.baseValue = this.armorCapacity.baseValue;
            data.shieldCapacity.baseValue = this.shieldCapacity.baseValue;
            data.powerCapacity.baseValue = this.powerCapacity.baseValue;
            return data;
        }

        protected override void onDeserializeObjectData(TezSaveController.Reader reader)
        {
            this.hullCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.HullCapacity.name);
            this.armorCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.ArmorCapacity.name);
            this.shieldCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.ShieldCapacity.name);
            this.powerCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.PowerCapacity.name);
        }

        protected override void onSerializeObjectData(TezSaveController.Writer writer)
        {
            throw new NotImplementedException();
        }
    }

    abstract class Unit : TezProtoObject
    {

    }

    class Character : Unit, ITezProtoObject<CharacterData>
    {
        public CharacterData protoData =>(CharacterData)mProtoData;

        protected override TezProtoObjectData createProtoData()
        {
            return new CharacterData();
        }
    }

    [TezPrototypeRegister("Ship", ItemClassIndexConfig.Ship)]
    class Ship
        : Unit
        , ITezProtoObject<ShipData>
        , ITezBonusSystemHolder
    {
        //Life
        public TezLifeHolder lifeHolder { get; } = new TezLifeHolder();

        //Property
        TezValueArray mValueArray = new TezValueArray();
        public TezValueArray valueArray => mValueArray;

        //Bonusable
        TezBonusSystem mBonusSystem = new TezBonusSystem();
        public TezBonusSystem bonusSystem => mBonusSystem;

        public ShipData protoData => (ShipData)mProtoData;

        public Ship()
        {
            this.lifeHolder.create(this);
            //default value
            this.protoData.hullCapacity.baseValue = 5;
            this.protoData.armorCapacity.baseValue = 34;
            this.protoData.shieldCapacity.baseValue = 8;
            this.protoData.powerCapacity.baseValue = 50;
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


        private void initValue()
        {
            mValueArray.init(MyDescriptorConfig.TypeID.ShipValue);
            mValueArray.set(this.protoData.hull);
            mValueArray.set(this.protoData.armor);
            mValueArray.set(this.protoData.shield);
            mValueArray.set(this.protoData.power);
        }

        public void initBonusSystem()
        {
            this.protoData.hullCapacity.createContainer<TezBonusModifierCache>();
            this.protoData.armorCapacity.createContainer<TezBonusModifierCache>();
            this.protoData.shieldCapacity.createContainer<TezBonusModifierList>();
            this.protoData.powerCapacity.createContainer<TezBonusModifierList>();

            mBonusSystem.init();
            mBonusSystem.set(this.protoData.hullCapacity);
            mBonusSystem.set(this.protoData.armorCapacity);
            mBonusSystem.set(this.protoData.shieldCapacity);
            mBonusSystem.set(this.protoData.powerCapacity);
        }

        private void setValue()
        {
            this.protoData.hull.innerValue = this.protoData.hullCapacity.value;
            this.protoData.armor.innerValue = this.protoData.armorCapacity.value;
            this.protoData.shield.innerValue = this.protoData.shieldCapacity.value;
            this.protoData.power.innerValue = this.protoData.powerCapacity.value;
        }

        public void update()
        {
            if (this.lifeHolder.isValied)
            {
                if (this.protoData.hull.value == 0)
                {
                    Console.WriteLine("Ship Dead");
                    this.lifeHolder.setInvalid();
                }
            }
        }

        protected override void onClose()
        {
            base.onClose();

            mBonusSystem.close();
            mValueArray.close();
            this.lifeHolder.close();

            mBonusSystem = null;
            mValueArray = null;
        }

        protected override TezProtoObjectData createProtoData()
        {
            return new ShipData();
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
            //var proto = TezcatFramework.protoDB.createObject<HealthPotion>(0);

            Console.WriteLine("Create HealthPotion From ProtoDB");
            var potion1 = TezcatFramework.protoDB.createObject<HealthPotion>(0);
            potion1.init();

            Console.WriteLine($"Potion1: {potion1.itemInfo.NID}, HealthAdd: {potion1.protoData.healthAdd.value}");

            Console.WriteLine("Create HealthPotion From Potion1");
            var potion2 = potion1.protoData.createObject<HealthPotion>();
            potion2.init();

            Console.WriteLine($"Potion2: {potion2.itemInfo.NID}, HealthAdd: {potion2.healthAdd.value}");
            Console.WriteLine();

            Console.WriteLine($"Potion1==Potion2 : {potion1 == potion2}");
            Console.WriteLine($"Potion1.isItemOf(Potion2) : {potion1.isItemOf(potion2)}");
            Console.WriteLine();

            Console.WriteLine("Potion1 MakeAnRuntimeProtoByThis");
            potion1.makeAnRuntimeProtoByThis();
            Console.WriteLine($"Potion1.isItemOf(Potion2) : {potion1.isItemOf(potion2)}");
            var potion3 = potion1.protoData.createObject<HealthPotion>();
            potion3.init();
            Console.WriteLine($"Potion1==Potion3 : {potion1 == potion3}");
            Console.WriteLine($"Potion1.isItemOf(Potion3) : {potion1.isItemOf(potion3)}");

            potion1.close();
            potion2.close();
            potion3.close();


            var armor1 = TezcatFramework.protoDB.createObject<Breastplate>(0);
            armor1.init();

            Console.WriteLine();
            Console.WriteLine($"Armor: {armor1.itemInfo.NID}, Armor: {armor1.protoData.armorAdd}");

            armor1.close();

            var wall = new Wall();
            wall.close();
        }

        protected override void onClose()
        {

        }
    }
}