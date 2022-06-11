using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Test
{
    public class TeztCategory
    {
        public void generateCShapFile()
        {
            /*
             * Json text
             * 
             * use TezCategorySystem.generateCodeFile to generate a xx.cs file
             * 
             {
                "Namespace": "tezcat.Framework.Core",
                "WrapperClass": "TestCategory",
                "Root": {
                    "Equipment": {
                        "Armor": [
                            "ArmorPlate"
                        ],
                        "Weapon": [
                            "Gun",
                            "Axe"
                        ]
                    },
                    "Unit": {
                        "Ship": [
                            "Frigate"
                        ]
                    },
                    "T1": {
                        "T2": {
                            "T3": [
                                "T4",
                                "T5",
                                "T6"
                            ]
                        },
                        "T7": [
                            "T8"
                        ]
                    }
                }
            }
            *
            *
            */
            TezJsonReader reader = new TezJsonReader();
            if (reader.load("xxxxx.json"))
            {
                TezCategorySystem.generateCodeFile("MyFilePath", reader);
            }
        }

        public void dosomething()
        {
            TestCategory.init();
        }
    }


    public static class TestCategory
    {
        public class Root : TezCategoryRootToken<Root, Root.Category>
        {
            public enum Category
            {
                Equipment,
                Unit,
                T1,
            }
            private Root(Category value) : base(value)
            {
            }
            public static readonly Root Equipment = new Root(Category.Equipment);
            public static readonly Root Unit = new Root(Category.Unit);
            public static readonly Root T1 = new Root(Category.T1);
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
                ArmorPlate,
            }
            private Armor(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly Armor ArmorPlate = new Armor(Category.ArmorPlate, Equipment.Armor);
            public static void init()
            {
                ArmorPlate.registerID(Root.Equipment);
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
                Gun.registerID(Root.Equipment);
                Axe.registerID(Root.Equipment);
            }
        }

        public class Unit : TezCategoryPathToken<Unit, Unit.Category>
        {
            public enum Category
            {
                Ship,
            }
            private Unit(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly Unit Ship = new Unit(Category.Ship, Root.Unit);
        }

        public class Ship : TezCategoryFinalToken<Ship, Ship.Category>
        {
            public enum Category
            {
                Frigate,
            }
            private Ship(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly Ship Frigate = new Ship(Category.Frigate, Unit.Ship);
            public static void init()
            {
                Frigate.registerID(Root.Unit);
            }
        }

        public class T1 : TezCategoryPathToken<T1, T1.Category>
        {
            public enum Category
            {
                T2,
                T7,
            }
            private T1(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly T1 T2 = new T1(Category.T2, Root.T1);
            public static readonly T1 T7 = new T1(Category.T7, Root.T1);
        }

        public class T2 : TezCategoryPathToken<T2, T2.Category>
        {
            public enum Category
            {
                T3,
            }
            private T2(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly T2 T3 = new T2(Category.T3, T1.T2);
        }

        public class T3 : TezCategoryFinalToken<T3, T3.Category>
        {
            public enum Category
            {
                T4,
                T5,
                T6,
            }
            private T3(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly T3 T4 = new T3(Category.T4, T2.T3);
            public static readonly T3 T5 = new T3(Category.T5, T2.T3);
            public static readonly T3 T6 = new T3(Category.T6, T2.T3);
            public static void init()
            {
                T4.registerID(Root.T1);
                T5.registerID(Root.T1);
                T6.registerID(Root.T1);
            }
        }

        public class T7 : TezCategoryFinalToken<T7, T7.Category>
        {
            public enum Category
            {
                T8,
            }
            private T7(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
            {
            }
            public static readonly T7 T8 = new T7(Category.T8, T1.T7);
            public static void init()
            {
                T8.registerID(Root.T1);
            }
        }

        public static void init()
        {
            Armor.init();
            Weapon.init();
            Ship.init();
            T3.init();
            T7.init();
        }
    }
}
