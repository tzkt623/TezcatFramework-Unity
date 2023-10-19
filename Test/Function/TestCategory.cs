using tezcat.Framework.Database;

namespace tezcat.Framework.Test
{
    public static class MyCategory
    {
        public class Root : TezCategoryRootToken<Root, Root.Category>
        {
            public enum Category
            {
                Equipment,
                Unit,
                Useable,
            }

            private Root(Category value) : base(value)
            {

            }

            public static readonly Root Equipment = new Root(Category.Equipment);
            public static readonly Root Unit = new Root(Category.Unit);
            public static readonly Root Useable = new Root(Category.Useable);
        }

        public class Equipment : TezCategoryPathToken<Equipment, Equipment.Category>
        {
            public enum Category
            {
                Armor,
                Weapon,
            }

            private Equipment(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {

            }

            public static readonly Equipment Armor = new Equipment(Category.Armor, Root.Equipment);
            public static readonly Equipment Weapon = new Equipment(Category.Weapon, Root.Equipment);
        }

        public class Armor : TezCategoryFinalToken<Armor, Armor.Category>
        {
            public enum Category
            {
                Helmet,
                Breastplate,
                Leg
            }

            private Armor(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {

            }

            public static readonly Armor Helmet = new Armor(Category.Helmet, Equipment.Armor);
            public static readonly Armor Breastplate = new Armor(Category.Breastplate, Equipment.Armor);
            public static readonly Armor Leg = new Armor(Category.Leg, Equipment.Armor);
            public static void init()
            {
                Helmet.registerID();
                Breastplate.registerID();
                Leg.registerID();
            }
        }

        public class Weapon : TezCategoryFinalToken<Weapon, Weapon.Category>
        {
            public enum Category
            {
                Gun,
                Axe,
            }

            private Weapon(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {

            }

            public static readonly Weapon Gun = new Weapon(Category.Gun, Equipment.Weapon);
            public static readonly Weapon Axe = new Weapon(Category.Axe, Equipment.Weapon);

            public static void init()
            {
                Gun.registerID();
                Axe.registerID();
            }
        }

        public class Unit : TezCategoryFinalToken<Unit, Unit.Category>
        {
            public enum Category
            {
                Character,
            }

            private Unit(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {

            }

            public static readonly Unit Character = new Unit(Category.Character, Root.Unit);

            public static void init()
            {
                Character.registerID();
            }
        }

        public class Useable : TezCategoryPathToken<Useable, Useable.Category>
        {
            public enum Category
            {
                Potion,
            }

            private Useable(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {

            }

            public static readonly Useable Potion = new Useable(Category.Potion, Root.Useable);
        }

        public class Potion : TezCategoryFinalToken<Potion, Potion.Category>
        {
            public enum Category
            {
                HealthPotion,
                MagicPotion
            }

            private Potion(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {

            }

            public static readonly Potion HealthPotion = new Potion(Category.HealthPotion, Useable.Potion);
            public static readonly Potion MagicPotion = new Potion(Category.MagicPotion, Useable.Potion);

            public static void init()
            {
                HealthPotion.registerID();
                MagicPotion.registerID();
            }
        }

        public static void init()
        {
            Armor.init();
            Weapon.init();
            Unit.init();
            Potion.init();
        }
    }

    public class TestCategory : TezBaseTest
    {
        public TestCategory() : base("CategoryGenerator")
        {
        }

        public void generateCShapFile()
        {
            string loadPath = Path.root + "Category.json";
            string savePath = Path.root;
            TezJsonReader reader = new TezJsonReader();
            if (reader.load(loadPath))
            {
                TezCategorySystem.generateCodeFile(savePath, reader);
            }
        }

        public override void run()
        {
            this.generateCShapFile();
        }
    }
}
