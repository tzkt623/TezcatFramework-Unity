using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

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

    abstract class Armor : Equipment
    {
        public int armorAdd;

        protected override void onCopyDataFrome(ITezItemObject template)
        {
            base.onCopyDataFrome(template);
            this.armorAdd = ((Armor)template).armorAdd;
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            armorAdd = reader.readInt("ArmorAdd");
        }
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
    #endregion

    class TestObject : TezBaseTest
    {
        public TestObject() : base("Objects")
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
    }
}