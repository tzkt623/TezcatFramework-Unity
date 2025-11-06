using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public static class ItemClassIndexConfig
    {
        public const int Ship = 0;
    }

    class Wall : TezGameObject
    {

    }

    #region Useable
    class MagicPotionData : TezProtoObjectData/*继承ProtoData*/
    {
        public int magicAdd;

        protected override void onInit()
        {

        }

        protected override TezProtoObject createObjectInternal(int index)
        {
            return new MagicPotion();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new MagicPotionData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            MagicPotionData data = (MagicPotionData)other;
            this.magicAdd = data.magicAdd;
        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.magicAdd = reader.readInt("MagicAdd");
        }


    }

    class HealthPotionData : TezProtoObjectData
    {
        public TezBonusModifier healthAdd { get; private set; } = new TezBonusModifier(MyDescriptorConfig.HumanProperty.HealthCapacity);

        protected override void onInit()
        {

        }

        protected override TezProtoObject createObjectInternal(int index)
        {
            return new HealthPotion();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new HealthPotionData();
        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.healthAdd.value = reader.readInt("HealthAdd");
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            HealthPotionData data = (HealthPotionData)other;
            this.healthAdd.value = data.healthAdd.value;
        }

    }

    abstract class Useable : TezProtoObject
    {

    }

    abstract class Potion : Useable
    {

    }

    class MagicPotion : Potion, ITezProtoObjectDataGetter<MagicPotionData>/*指明protodata类*/
    {
        public MagicPotionData protoData => (MagicPotionData)mProtoData;
    }

    class HealthPotion : Potion, ITezProtoObjectDataGetter<HealthPotionData>
    {
        public HealthPotionData protoData => (HealthPotionData)mProtoData;
        public TezBonusModifier healthAdd => this.protoData.healthAdd;
    }
    #endregion

    #region Equipment
    abstract class WeaponData : TezProtoObjectData
    {
        public int attack;

        protected override void onInit()
        {

        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.attack = reader.readInt("Attack");
        }


    }

    class GunData : WeaponData
    {
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Gun();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new GunData
            {
                attack = this.attack
            };
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            GunData otherData = (GunData)other;
            this.attack = otherData.attack;
        }
    }

    class AxeData : WeaponData
    {
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Axe();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new AxeData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            AxeData data = (AxeData)other;
            data.attack = this.attack;
        }
    }

    class MissleData : WeaponData
    {
        public string name = null;
        public int step = 0;
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Missle();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new MissleData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            MissleData data = (MissleData)other;
            this.step = 0;
            this.name = data.name;
            this.attack = data.attack;
        }
    }

    abstract class Equipment : TezProtoObject
    {

    }

    abstract class Weapon : Equipment
    {

    }

    class Gun : Weapon, ITezProtoObjectDataGetter<GunData>
    {
        public GunData protoData => (GunData)mProtoData;
    }

    class Axe : Weapon, ITezProtoObjectDataGetter<AxeData>
    {
        public AxeData protoData => (AxeData)mProtoData;
    }

    class Missle : Weapon, ITezProtoObjectDataGetter<MissleData>
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
    }


    abstract class ArmorData : TezProtoObjectData
    {
        public TezValueInt armorAdd { get; private set; } = new TezValueInt(MyDescriptorConfig.Modifier.ArmorAdd);

        protected override void onInit()
        {

        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.armorAdd.value = reader.readInt("ArmorAdd");
        }


    }

    class ArmorPlateData : ArmorData
    {
        protected override TezProtoObjectData justNewProtoData()
        {
            return new ArmorPlateData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            ArmorPlateData data = (ArmorPlateData)other;
            this.armorAdd.value = data.armorAdd.value;
        }

        protected override TezProtoObject createObjectInternal(int index)
        {
            return new ArmorPlate();
        }
    }

    abstract class Armor : Equipment
    {

    }

    class ArmorPlate : Armor, ITezProtoObjectDataGetter<ArmorPlateData>
    {
        public ArmorPlateData protoData => (ArmorPlateData)mProtoData;
    }

    class HelmetData : ArmorData
    {
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Helmet();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new HelmetData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            HelmetData data = (HelmetData)other;
            this.armorAdd.value = data.armorAdd.value;
        }
    }

    class BreastplateData : ArmorData
    {
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Breastplate();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new BreastplateData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            BreastplateData data = (BreastplateData)other;
            this.armorAdd.value = data.armorAdd.value;
        }
    }

    class LegData : ArmorData
    {
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Leg();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new LegData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            LegData data = (LegData)other;
            this.armorAdd.value = data.armorAdd.value;
        }
    }

    [TezPrototypeRegister("Helmet", 3)]
    class Helmet : Armor, ITezProtoObjectDataGetter<HelmetData>
    {
        public HelmetData protoData => (HelmetData)mProtoData;
    }

    [TezPrototypeRegister("Breastplate", 2)]
    class Breastplate : Armor, ITezProtoObjectDataGetter<BreastplateData>
    {
        public BreastplateData protoData => (BreastplateData)mProtoData;
    }

    [TezPrototypeRegister("Leg", 1)]
    class Leg : Armor, ITezProtoObjectDataGetter<LegData>
    {
        public LegData protoData => (LegData)mProtoData;
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
        protected override TezProtoObject createObjectInternal(int index)
        {
            return new Character();
        }

        protected override TezProtoObjectData justNewProtoData()
        {
            return new CharacterData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {

        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {

        }


    }

    class ShipData : UnitData
    {
        public TezValueInt hull { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Hull);
        public TezValueInt armor { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Armor);
        public TezValueInt shield { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Shield);
        public TezValueInt power { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Power);

        public TezBonusInt hullCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.HullCapacity);
        public TezBonusInt armorCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.ArmorCapacity);
        public TezBonusInt shieldCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.ShieldCapacity);
        public TezBonusInt powerCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.PowerCapacity);

        protected override TezProtoObject createObjectInternal(int index)
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

        protected override TezProtoObjectData justNewProtoData()
        {
            return new ShipData();
        }

        protected override void copyDataFrom(TezProtoObjectData other)
        {
            ShipData data = (ShipData)other;
            this.hull.value = data.hull.value;
            this.armor.value = data.armor.value;
            this.shield.value = data.shield.value;
            this.power.value = data.power.value;
            this.hullCapacity.baseValue = data.hullCapacity.baseValue;
            this.armorCapacity.baseValue = data.armorCapacity.baseValue;
            this.shieldCapacity.baseValue = data.shieldCapacity.baseValue;
            this.powerCapacity.baseValue = data.powerCapacity.baseValue;
        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.hullCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.HullCapacity.name);
            this.armorCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.ArmorCapacity.name);
            this.shieldCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.ShieldCapacity.name);
            this.powerCapacity.baseValue = reader.readInt(MyDescriptorConfig.ShipPorperty.PowerCapacity.name);
        }


    }

    abstract class Unit : TezProtoObject
    {

    }

    class Character : Unit, ITezProtoObjectDataGetter<CharacterData>
    {
        public CharacterData protoData => (CharacterData)mProtoData;
    }

    [TezPrototypeRegister("Ship", ItemClassIndexConfig.Ship)]
    class Ship
        : Unit
        , ITezProtoObjectDataGetter<ShipData>
        , ITezBonusSystemHolder
    {
        //Life
        public TezLifeHolder lifeHolder { get; } = new TezLifeHolder();

        //Property
        TezValueArray mValueArray = new TezValueArray();
        public TezValueArray valueArray => mValueArray;

        //Bonusable
        TezBonusValueSystem mBonusSystem = new TezBonusValueSystem();
        public TezBonusValueSystem bonusSystem => mBonusSystem;

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
            mBonusSystem.init();
            mBonusSystem.register(this.protoData.hullCapacity);
            mBonusSystem.register(this.protoData.armorCapacity);
            mBonusSystem.register(this.protoData.shieldCapacity);
            mBonusSystem.register(this.protoData.powerCapacity);
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

        //         protected override TezProtoObjectData createProtoData()
        //         {
        //             return new ShipData();
        //         }
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
            Console.WriteLine("Create HealthPotion From ProtoDB");
            var potion1 = TezcatFramework.protoDB.createObject<HealthPotionData, HealthPotion>(0);
            potion1.init();

            Console.WriteLine($"Potion1: {potion1.protoInfo.NID}, HealthAdd: {potion1.protoData.healthAdd.value}");

            Console.WriteLine("Create HealthPotion From Potion1");
            var potion2 = potion1.protoData.createObjectByCopyMe<HealthPotion>();
            potion2.init();

            Console.WriteLine($"Potion2: {potion2.protoInfo.NID}, HealthAdd: {potion2.healthAdd.value}");
            Console.WriteLine();

            Console.WriteLine($"Potion1==Potion2 : {potion1 == potion2}");
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(Potion2) : {potion1.isTheSameProtoDataOf(potion2)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(Potion2) : {potion1.isTheSameProtoObjectOf(potion2)}");
            Console.WriteLine();

            Console.WriteLine("Potion1 CreateRedefineObject");
            var potion1_1 = potion1.protoData.createRedefineData().createObjectWithMe<HealthPotion>();
            potion1_1.init();
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(potion1_1) : {potion1.isTheSameProtoDataOf(potion1_1)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(potion1_1) : {potion1.isTheSameProtoObjectOf(potion1_1)}");
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(Potion2) : {potion1.isTheSameProtoDataOf(potion2)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(Potion2) : {potion1.isTheSameProtoObjectOf(potion2)}");
            Console.WriteLine($"Potion1_1.isTheSameProtoDataOf(Potion2) : {potion1_1.isTheSameProtoDataOf(potion2)}");
            Console.WriteLine($"Potion1_1.isTheSameProtoObjectOf(Potion2) : {potion1_1.isTheSameProtoObjectOf(potion2)}");

            Console.WriteLine();
            var potion3 = potion1.protoData.createObjectByCopyMe<HealthPotion>();
            potion3.init();
            Console.WriteLine($"Potion1==Potion3 : {potion1 == potion3}");
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(Potion3) : {potion1.isTheSameProtoDataOf(potion3)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(Potion3) : {potion1.isTheSameProtoObjectOf(potion3)}");

            potion1.close();
            potion1_1.close();
            potion2.close();
            potion3.close();

            var armor1 = TezcatFramework.protoDB.createObject<BreastplateData, Breastplate>(0);
            armor1.init();

            Console.WriteLine();
            Console.WriteLine($"Armor: {armor1.protoInfo.NID}, Armor: {armor1.protoData.armorAdd}");

            armor1.close();

            var wall = new Wall();
            wall.close();
        }

        protected override void onClose()
        {

        }
    }
}