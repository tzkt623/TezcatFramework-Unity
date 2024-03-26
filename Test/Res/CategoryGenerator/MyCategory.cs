using tezcat.Framework.Database;


namespace tezcat.Framework.Core
{
public static class MyCategory
{
public class Root : TezCategoryRootToken<Root, Root.Category>
{
public enum Category
{
Useable,
Equipment,
Unit,
}
private Root(Category value) : base(value)
{
}
public static readonly Root Useable = new Root(Category.Useable);
public static readonly Root Equipment = new Root(Category.Equipment);
public static readonly Root Unit = new Root(Category.Unit);
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
MagicPotion,
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

public class Equipment : TezCategoryPathToken<Equipment, Equipment.Category>
{
public enum Category
{
Weapon,
Armor,
}
private Equipment(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
{
}
public static readonly Equipment Weapon = new Equipment(Category.Weapon, Root.Equipment);
public static readonly Equipment Armor = new Equipment(Category.Armor, Root.Equipment);
}

public class Weapon : TezCategoryFinalToken<Weapon, Weapon.Category>
{
public enum Category
{
Gun,
Axe,
Missle,
}
private Weapon(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
{
}
public static readonly Weapon Gun = new Weapon(Category.Gun, Equipment.Weapon);
public static readonly Weapon Axe = new Weapon(Category.Axe, Equipment.Weapon);
public static readonly Weapon Missle = new Weapon(Category.Missle, Equipment.Weapon);
public static void init()
{
Gun.registerID();
Axe.registerID();
Missle.registerID();
}
}

public class Armor : TezCategoryFinalToken<Armor, Armor.Category>
{
public enum Category
{
Helmet,
Breastplate,
Leg,
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

public class Unit : TezCategoryFinalToken<Unit, Unit.Category>
{
public enum Category
{
Character,
Ship,
}
private Unit(Category value, ITezCategoryBaseToken parentToken) : base(value, parentToken)
{
}
public static readonly Unit Character = new Unit(Category.Character, Root.Unit);
public static readonly Unit Ship = new Unit(Category.Ship, Root.Unit);
public static void init()
{
Character.registerID();
Ship.registerID();
}
}

public static void init()
{
Potion.init();
Weapon.init();
Armor.init();
Unit.init();
}
}
}
