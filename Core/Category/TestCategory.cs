using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    public class TeztCategory
    {
        public void generateCShapFile()
        {
            /*
             * Json 格式如下
             * 
             * FinalToken为Array
             * RootToken和PathToken均为Object
             * WrapperClass是包装类名称也是文件名称
             * 
             * 使用TezCategorySystem.generateCodeFile自动生成下列所有类
             * 用于Database的分类以及游戏中的查询等功能
             * 
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
                ///仅路径
                ///不带文件名称
                ///不带分隔符
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

        public class Equipment : TezCategoryToken<Equipment, Equipment.Category>
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

        public class Armor : TezCategoryToken<Armor, Armor.Category>
        {
            public enum Category
            {
                ArmorPlate,
            }
            private Armor(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
            {
            }
            public static readonly Armor ArmorPlate = new Armor(Category.ArmorPlate, Equipment.Armor, Root.Equipment);
            public static void init() { }
        }

        public class Weapon : TezCategoryToken<Weapon, Weapon.Category>
        {
            public enum Category
            {
                Gun,
                Axe,
            }
            private Weapon(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
            {
            }
            public static readonly Weapon Gun = new Weapon(Category.Gun, Equipment.Weapon, Root.Equipment);
            public static readonly Weapon Axe = new Weapon(Category.Axe, Equipment.Weapon, Root.Equipment);
            public static void init() { }
        }

        public class Unit : TezCategoryToken<Unit, Unit.Category>
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

        public class Ship : TezCategoryToken<Ship, Ship.Category>
        {
            public enum Category
            {
                Frigate,
            }
            private Ship(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
            {
            }
            public static readonly Ship Frigate = new Ship(Category.Frigate, Unit.Ship, Root.Unit);
            public static void init() { }
        }

        public class T1 : TezCategoryToken<T1, T1.Category>
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

        public class T2 : TezCategoryToken<T2, T2.Category>
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

        public class T3 : TezCategoryToken<T3, T3.Category>
        {
            public enum Category
            {
                T4,
                T5,
                T6,
            }
            private T3(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
            {
            }
            public static readonly T3 T4 = new T3(Category.T4, T2.T3, Root.T1);
            public static readonly T3 T5 = new T3(Category.T5, T2.T3, Root.T1);
            public static readonly T3 T6 = new T3(Category.T6, T2.T3, Root.T1);
            public static void init() { }
        }

        public class T7 : TezCategoryToken<T7, T7.Category>
        {
            public enum Category
            {
                T8,
            }
            private T7(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
            {
            }
            public static readonly T7 T8 = new T7(Category.T8, T1.T7, Root.T1);
            public static void init() { }
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
