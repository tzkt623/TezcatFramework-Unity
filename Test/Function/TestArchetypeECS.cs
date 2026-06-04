using System;
using tezcat.Framework.ArchetypeECS;
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

    }
    #region Register


    abstract class ComOrgData : TezProtoObjectData, TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComOrgData>.ID;
        public int entityID { get; set; }
    }

    abstract class ComPotionData : TezProtoObjectData, TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComPotionData>.ID;
        public int entityID { get; set; }
    }


    abstract class ComPotion : TezWorld.IComponent
    {
        public enum PotionType
        {
            Health,
            Magic,
        }

        public int componentID => TezWorld.ComponentID<ComPotion>.ID;
        public int entityID { get; set; }


        public PotionType potionType;
        public int value;

        public abstract void close();
    }

    #endregion

    #region ArchetypeMask
    class EntityMaskID_Potion : TezWorld.ArchetypeMaskID<ComPotionData, ComPotion> { }
    class EntityMaskID_Weapon : TezWorld.ArchetypeMaskID<ComWeaponData, ComWeapon> { }
    class EntityMaskID_Armor : TezWorld.ArchetypeMaskID<ComArmorData, ComArmor> { }
    class EntityMaskID_Unit : TezWorld.ArchetypeMaskID<ComUnitData, ComUnit> { }

    class EntityRegistry
    {
        public static void init()
        {
            TezWorld.registerArchetype<EntityMaskID_Potion>();
            TezWorld.registerArchetype<EntityMaskID_Weapon>();
            TezWorld.registerArchetype<EntityMaskID_Armor>();
            TezWorld.registerArchetype<EntityMaskID_Unit>();

            TezWorld.buildingWorld();
        }
    }

    #endregion


    #region Useable
    class MagicPotionData : ComPotionData
    {
        public int magicAdd;

        protected override void onInit()
        {

        }

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new MagicPotionData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            MagicPotionData data = (MagicPotionData)other;
            this.magicAdd = data.magicAdd;
        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.magicAdd = reader.readInt("MagicAdd");
        }

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Potion>(entity);
            TezWorld.initComponent<ComPotionData, MagicPotionData>(entity, (MagicPotionData)protoData);
            TezWorld.initComponent<ComPotion, MagicPotion>(entity);
        }
    }

    class HealthPotionData : ComPotionData
    {
        public TezBonusModifier healthAdd { get; private set; } = new TezBonusModifier(MyDescriptorConfig.HumanProperty.HealthCapacity);

        protected override void onInit()
        {

        }

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new HealthPotionData();
        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.healthAdd.value = reader.readInt("HealthAdd");
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            HealthPotionData data = (HealthPotionData)other;
            this.healthAdd.value = data.healthAdd.value;
        }

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Potion>(entity);
            TezWorld.initComponent<ComPotionData, HealthPotionData>(entity, (HealthPotionData)protoData);
            TezWorld.initComponent<ComPotion, HealthPotion>(entity);
        }
    }

    abstract class Useable : TezProtoObject
    {

    }

    abstract class Potion : Useable
    {

    }

    class MagicPotion : ComPotion
    {
        public override void close()
        {
            throw new NotImplementedException();
        }
    }

    class HealthPotion : ComPotion
    {
        public override void close()
        {

        }
    }
    #endregion

    #region Equipment
    abstract class ComWeaponData : TezProtoObjectData, TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComWeaponData>.ID;
        public int entityID { get; set; }

        public int attack;

        protected override void onInit()
        {

        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.attack = reader.readInt("Attack");
        }


    }

    class GunData : ComWeaponData
    {
        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new GunData
            {
                attack = this.attack
            };
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            GunData otherData = (GunData)other;
            this.attack = otherData.attack;
        }

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Weapon>(entity);
            TezWorld.initComponent<ComWeaponData, GunData>(entity, (GunData)protoData);
            TezWorld.initComponent<ComWeapon, Gun>(entity);
        }
    }

    class AxeData : ComWeaponData
    {
        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new AxeData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            AxeData data = (AxeData)other;
            data.attack = this.attack;
        }

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Weapon>(entity);
            TezWorld.initComponent<ComWeaponData, AxeData>(entity, (AxeData)protoData);
            TezWorld.initComponent<ComWeapon, Axe>(entity);
        }
    }

    class MissleData : ComWeaponData
    {
        public string name = null;
        public int step = 0;

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new MissleData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            MissleData data = (MissleData)other;
            this.step = 0;
            this.name = data.name;
            this.attack = data.attack;
        }

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Weapon>(entity);
            TezWorld.initComponent<ComWeaponData, MissleData>(entity, (MissleData)protoData);
            TezWorld.initComponent<ComWeapon, Missle>(entity);
        }
    }

    abstract class Equipment : TezProtoObject
    {

    }

    abstract class ComWeapon : TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComWeapon>.ID;
        public int entityID { get; set; }

        public virtual void close()
        {
        }

        public virtual void update()
        {

        }
    }

    class Gun : ComWeapon
    {

    }

    [TezClassFactoryRegister(name = "Axe")]
    class Axe : ComWeapon
    {

    }

    class Missle : ComWeapon
    {
        public string name = null;
        public int step = 0;
        public TezLifeMonitor target = null;
        bool stop = false;

        public override void update()
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
                TezWorld.getComponent<ComUnitData, ShipData>(ship.entityID).hull.value = 0;

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

        public override void close()
        {
            this.target?.close();
        }
    }


    abstract class ComArmorData : TezProtoObjectData, TezWorld.IComponent
    {
        public TezValueInt armorAdd { get; private set; } = new TezValueInt(MyDescriptorConfig.Modifier.ArmorAdd);

        public int componentID => TezWorld.ComponentID<ComArmorData>.ID;
        public int entityID { get; set; }

        protected override void onInit()
        {

        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {
            this.armorAdd.value = reader.readInt("ArmorAdd");
        }
    }

    abstract class ComArmor : TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComArmor>.ID;
        public int entityID { get; set; }

        public virtual void close() { }
    }

    class ArmorPlateData : ComArmorData
    {
        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new ArmorPlateData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            ArmorPlateData data = (ArmorPlateData)other;
            this.armorAdd.value = data.armorAdd.value;
        }

        
        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Armor>(entity);
            TezWorld.initComponent<ComArmorData, ArmorPlateData>(entity, (ArmorPlateData)protoData);
            TezWorld.initComponent<ComArmor, ArmorPlate>(entity);
        }
    }

    class ArmorPlate : ComArmor
    {

    }

    class HelmetData : ComArmorData
    {
        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Armor>(entity);
            TezWorld.initComponent<ComArmorData, HelmetData>(entity, (HelmetData)protoData);
            TezWorld.initComponent<ComArmor, Helmet>(entity);
        }

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new HelmetData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            HelmetData data = (HelmetData)other;
            this.armorAdd.value = data.armorAdd.value;
        }
    }

    class BreastplateData : ComArmorData
    {
        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Armor>(entity);
            TezWorld.initComponent<ComArmorData, BreastplateData>(entity, (BreastplateData)protoData);
            TezWorld.initComponent<ComArmor, Breastplate>(entity);
        }

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new BreastplateData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            BreastplateData data = (BreastplateData)other;
            this.armorAdd.value = data.armorAdd.value;
        }
    }

    class LegData : ComArmorData
    {
        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Armor>(entity);
            TezWorld.initComponent<ComArmorData, LegData>(entity, (LegData)protoData);
            TezWorld.initComponent<ComArmor, Leg>(entity);
        }

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new LegData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {
            LegData data = (LegData)other;
            this.armorAdd.value = data.armorAdd.value;
        }
    }

    [TezPrototypeRegister("Helmet", 3)]
    class Helmet : ComArmor
    {

    }

    [TezPrototypeRegister("Breastplate", 2)]
    class Breastplate : ComArmor
    {

    }

    [TezPrototypeRegister("Leg", 1)]
    class Leg : ComArmor
    {

    }
    #endregion

    #region Unit
    abstract class ComUnitData : TezProtoObjectData, TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComUnitData>.ID;
        public int entityID { get; set; }
   
        protected override void onInit()
        {

        }
    }

    class CharacterData : ComUnitData
    {
        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new CharacterData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
        {

        }

        protected override void onLoadProtoData(TezSaveController.Reader reader)
        {

        }

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Unit>(entity);
            TezWorld.initComponent<ComUnitData, CharacterData>(entity, (CharacterData)protoData);
            TezWorld.initComponent<ComUnit, Character>(entity);
        }
    }

    class ShipData : ComUnitData
    {
        public TezValueInt hull { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Hull);
        public TezValueInt armor { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Armor);
        public TezValueInt shield { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Shield);
        public TezValueInt power { get; private set; } = new TezValueInt(MyDescriptorConfig.ShipValue.Power);

        public TezBonusInt hullCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.HullCapacity);
        public TezBonusInt armorCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.ArmorCapacity);
        public TezBonusInt shieldCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.ShieldCapacity);
        public TezBonusInt powerCapacity { get; private set; } = new TezBonusInt(new TezBonusModifierCache(), MyDescriptorConfig.ShipPorperty.PowerCapacity);

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

        protected override TezProtoObjectData beginNewObject_JustNewProtoData()
        {
            return new ShipData();
        }

        protected override void endNewObject_CopyDataFrom(TezProtoObjectData other)
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

        protected override void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData)
        {
            TezWorld.registerEntity<EntityMaskID_Unit>(entity);
            var data = TezWorld.initComponent<ComUnitData, ShipData>(entity, (ShipData)protoData);
            var ship = TezWorld.initComponent<ComUnit, Ship>(entity);
            ship.setShipData(data);
        }

    }

    abstract class ComUnit : TezWorld.IComponent
    {
        public int componentID => TezWorld.ComponentID<ComUnit>.ID;
        public int entityID { get; set; }
        public virtual void close() { }

        public virtual void update() { }
    }

    class Character : ComUnit
    {

    }

    [TezPrototypeRegister("Ship", ItemClassIndexConfig.Ship)]
    class Ship
        : ComUnit
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

        ShipData protoData = null;

        public Ship()
        {
            this.lifeHolder.create(this);
            //default value
            this.protoData.hullCapacity.baseValue = 5;
            this.protoData.armorCapacity.baseValue = 34;
            this.protoData.shieldCapacity.baseValue = 8;
            this.protoData.powerCapacity.baseValue = 50;
        }

        public void setShipData(ShipData data)
        {
            protoData = data;
            this.initBonusSystem();
            this.initValue();
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

        public override void update()
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

        public override void close()
        {
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

    class TestArchetypeECS : TezBaseTest
    {
        public TestArchetypeECS() : base("ArchetypeECS")
        {

        }

        public override void init()
        {

        }

        public override void run()
        {
            Console.WriteLine("Create HealthPotion From ProtoDB");
            var e_potion1 = TezcatFramework.protoDB.createEntity<HealthPotionData>(0);
            var potion1 = TezWorld.getComponent<ComPotionData, HealthPotionData>(e_potion1);

            Console.WriteLine($"Potion1: {potion1.protoInfo.NID}, HealthAdd: {potion1.healthAdd.value}");

            Console.WriteLine("Create HealthPotion From Potion1");
            var e_potion2 = potion1.createEntity();
            var potion2 = TezWorld.getComponent<ComPotionData, HealthPotionData>(e_potion2);

            Console.WriteLine($"Potion2: {potion2.protoInfo.NID}, HealthAdd: {potion2.healthAdd.value}");
            Console.WriteLine();

            Console.WriteLine($"Potion1==Potion2 : {potion1 == potion2}");
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(Potion2) : {potion1.isTheSameProtoDataOf(potion2)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(Potion2) : {e_potion1 == e_potion2}");
            Console.WriteLine();

            Console.WriteLine("Potion1 CreateRedefineObject");
            var e_potion1_1 = potion1.detachEntity();
            var potion1_1 = TezWorld.getComponent<ComPotionData, HealthPotionData>(e_potion1_1);

            Console.WriteLine($"Potion1.isTheSameProtoDataOf(potion1_1) : {potion1.isTheSameProtoDataOf(potion1_1)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(potion1_1) : {e_potion1 == e_potion1_1}");
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(Potion2) : {potion1.isTheSameProtoDataOf(potion2)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(Potion2) : {e_potion1 == e_potion2}");
            Console.WriteLine($"Potion1_1.isTheSameProtoDataOf(Potion2) : {potion1_1.isTheSameProtoDataOf(potion2)}");
            Console.WriteLine($"Potion1_1.isTheSameProtoObjectOf(Potion2) : {e_potion1_1 == e_potion2}");
            Console.WriteLine();

            var e_potion3 = potion1.createEntity();
            var potion3 = TezWorld.getComponent<ComPotionData, HealthPotionData>(e_potion3);

            Console.WriteLine($"Potion1==Potion3 : {potion1 == potion3}");
            Console.WriteLine($"Potion1.isTheSameProtoDataOf(Potion3) : {potion1.isTheSameProtoDataOf(potion3)}");
            Console.WriteLine($"Potion1.isTheSameProtoObjectOf(Potion3) : {e_potion1 == e_potion3}");

            TezWorld.removeEntity(e_potion1);
            TezWorld.removeEntity(e_potion1_1);
            TezWorld.removeEntity(e_potion2);
            TezWorld.removeEntity(e_potion3);
    
            Console.WriteLine();
            Console.WriteLine("Create Breastplate From ProtoDB");

            var e_armor1 = TezcatFramework.protoDB.createEntity<BreastplateData>(0);
            var armor1 = TezWorld.getComponent<ComArmorData, BreastplateData>(e_armor1);

            Console.WriteLine();
            Console.WriteLine($"Armor: {armor1.protoInfo.NID}, Armor: {armor1.armorAdd}");

            TezWorld.removeEntity(e_armor1);

            var wall = new Wall();
            wall.close();
        }

        protected override void onClose()
        {

        }
    }
}