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
             * RootName的值一定与下方配置Key的值相同
             * 会生成下列所有类以自动注册
             * 用于Database的分类以及游戏中的查询等功能
             * 
             * 
             {
                "Namespace": "tezcat.Framework.Core",
                "Prefix": "DB",
                "RootName": "TestCategory",
                "TestCategory": {
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
    }

    public class DBTestCategory : TezCategoryRootToken<DBTestCategory, DBTestCategory.Category>
    {
        public enum Category
        {
            Equipment,
            Unit,
            T1,
        }
        private DBTestCategory(Category value) : base(value)
        {
        }
        public static readonly DBTestCategory Equipment = new DBTestCategory(Category.Equipment);
        public static readonly DBTestCategory Unit = new DBTestCategory(Category.Unit);
        public static readonly DBTestCategory T1 = new DBTestCategory(Category.T1);
    }
    public class DBEquipment : TezCategoryToken<DBEquipment, DBEquipment.Category>
    {
        public enum Category
        {
            Armor,
            Weapon,
        }
        private DBEquipment(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
        {
        }
        public static readonly DBEquipment Armor = new DBEquipment(Category.Armor, DBTestCategory.Equipment);
        public static readonly DBEquipment Weapon = new DBEquipment(Category.Weapon, DBTestCategory.Equipment);
    }
    public class DBArmor : TezCategoryToken<DBArmor, DBArmor.Category>
    {
        public enum Category
        {
            ArmorPlate,
        }
        private DBArmor(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
        {
        }
        public static readonly DBArmor ArmorPlate = new DBArmor(Category.ArmorPlate, DBEquipment.Armor, DBTestCategory.Equipment);
    }
    public class DBWeapon : TezCategoryToken<DBWeapon, DBWeapon.Category>
    {
        public enum Category
        {
            Gun,
            Axe,
        }
        private DBWeapon(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
        {
        }
        public static readonly DBWeapon Gun = new DBWeapon(Category.Gun, DBEquipment.Weapon, DBTestCategory.Equipment);
        public static readonly DBWeapon Axe = new DBWeapon(Category.Axe, DBEquipment.Weapon, DBTestCategory.Equipment);
    }
    public class DBUnit : TezCategoryToken<DBUnit, DBUnit.Category>
    {
        public enum Category
        {
            Ship,
        }
        private DBUnit(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
        {
        }
        public static readonly DBUnit Ship = new DBUnit(Category.Ship, DBTestCategory.Unit);
    }
    public class DBShip : TezCategoryToken<DBShip, DBShip.Category>
    {
        public enum Category
        {
            Frigate,
        }
        private DBShip(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
        {
        }
        public static readonly DBShip Frigate = new DBShip(Category.Frigate, DBUnit.Ship, DBTestCategory.Unit);
    }
    public class DBT1 : TezCategoryToken<DBT1, DBT1.Category>
    {
        public enum Category
        {
            T2,
            T7,
        }
        private DBT1(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
        {
        }
        public static readonly DBT1 T2 = new DBT1(Category.T2, DBTestCategory.T1);
        public static readonly DBT1 T7 = new DBT1(Category.T7, DBTestCategory.T1);
    }
    public class DBT2 : TezCategoryToken<DBT2, DBT2.Category>
    {
        public enum Category
        {
            T3,
        }
        private DBT2(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
        {
        }
        public static readonly DBT2 T3 = new DBT2(Category.T3, DBT1.T2);
    }
    public class DBT3 : TezCategoryToken<DBT3, DBT3.Category>
    {
        public enum Category
        {
            T4,
            T5,
            T6,
        }
        private DBT3(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
        {
        }
        public static readonly DBT3 T4 = new DBT3(Category.T4, DBT2.T3, DBTestCategory.T1);
        public static readonly DBT3 T5 = new DBT3(Category.T5, DBT2.T3, DBTestCategory.T1);
        public static readonly DBT3 T6 = new DBT3(Category.T6, DBT2.T3, DBTestCategory.T1);
    }
    public class DBT7 : TezCategoryToken<DBT7, DBT7.Category>
    {
        public enum Category
        {
            T8,
        }
        private DBT7(Category value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken, rootToken)
        {
        }
        public static readonly DBT7 T8 = new DBT7(Category.T8, DBT1.T7, DBTestCategory.T1);
    }
}
