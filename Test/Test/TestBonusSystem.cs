using System;
using tezcat.Framework.BonusSystem;

namespace tezcat.Framework.Test
{
    /// <summary>
    /// 第一类加成写法
    /// </summary>
    public class MyPathes
    {
        public static readonly TezBonusToken Equipment = TezBonusTokenCreator<MyPathes>.createToken("Equipment", TezBonusTokenType.Root, null);

        public static readonly TezBonusToken Turrent = TezBonusTokenCreator<MyPathes>.createToken("Turrent", TezBonusTokenType.Path, Equipment);
        public static readonly TezBonusToken LaserTurrent = TezBonusTokenCreator<MyPathes>.createToken("LaserTurrent", TezBonusTokenType.Leaf, Turrent);

        public static readonly TezBonusToken Armor = TezBonusTokenCreator<MyPathes>.createToken("Armor", TezBonusTokenType.Path, Equipment);
        public static readonly TezBonusToken ArmorPlate = TezBonusTokenCreator<MyPathes>.createToken("ArmorPlate", TezBonusTokenType.Leaf, Armor);
        public static readonly TezBonusToken ArmorRepairer = TezBonusTokenCreator<MyPathes>.createToken("ArmorRepairer", TezBonusTokenType.Leaf, Armor);



        public static readonly TezBonusToken Battle = TezBonusTokenCreator<MyPathes>.createToken("Battle", TezBonusTokenType.Root, null);

        public static readonly TezBonusToken Melee = TezBonusTokenCreator<MyPathes>.createToken("Melee", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Slash = TezBonusTokenCreator<MyPathes>.createToken("Slash", TezBonusTokenType.Leaf, Melee);

        public static readonly TezBonusToken Range = TezBonusTokenCreator<MyPathes>.createToken("Range", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Shoot = TezBonusTokenCreator<MyPathes>.createToken("Shoot", TezBonusTokenType.Leaf, Range);
    }

    /// <summary>
    /// 第二类加成写法
    /// </summary>
    public class MySkillPathes
    {
        public static readonly TezBonusToken Battle = TezBonusTokenCreator<MySkillPathes>.createToken("Battle", TezBonusTokenType.Root, null);

        public static readonly TezBonusToken Melee = TezBonusTokenCreator<MySkillPathes>.createToken("Melee", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Slash = TezBonusTokenCreator<MySkillPathes>.createToken("Slash", TezBonusTokenType.Leaf, Melee);

        public static readonly TezBonusToken Range = TezBonusTokenCreator<MySkillPathes>.createToken("Range", TezBonusTokenType.Path, Battle);
        public static readonly TezBonusToken Shoot = TezBonusTokenCreator<MySkillPathes>.createToken("Shoot", TezBonusTokenType.Leaf, Range);
    }


    public class MyBountyLeaf : TezBonusTreeLeafNode
    {
        public MyBountyLeaf(int id, ITezBonusTree system) : base(id, system)
        {

        }
    }

    public class MyBountyPath : TezBonusTreePathNode
    {
        public MyBountyPath(int id, ITezBonusTree tree) : base(id, tree)
        {

        }
    }


    public class MyBountyTree : TezBonusTree<TezBonusTreeListContainer>
    {
        protected override TezBonusTreeNode onCreateNode(ITezBonusToken token)
        {
            switch (token.tokenType)
            {
                case TezBonusTokenType.Root:
                case TezBonusTokenType.Path:
                    return new MyBountyPath(token.tokenID, this);
                case TezBonusTokenType.Leaf:
                    return new MyBountyLeaf(token.tokenID, this);
                default:
                    throw new Exception();
            }
        }
    }

    public class MyAddAttack : ITezBonusObject
    {
        public TezBonusPath bonusPath { get; } = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.Turrent);
        public int addAttack = 5;
    }

    public class MyLaserTurrent
    {
        public int attack;
        public TezBonusAgent agent = new TezBonusAgent();
        public TezBonusPath bonusPath = TezBonusTokenCreator<MyPathes>.getPath(MyPathes.LaserTurrent);

        public void init()
        {
            agent.setPath(this.bonusPath);
            agent.setListener(this.onAddBountyObject, this.onRemoveBountyObject);
        }

        private void onAddBountyObject(ITezBonusObject obj)
        {
            MyAddAttack mobj = (MyAddAttack)obj;
            attack += mobj.addAttack;
        }

        private void onRemoveBountyObject(ITezBonusObject obj)
        {
            MyAddAttack mobj = (MyAddAttack)obj;
            attack -= mobj.addAttack;
        }
    }

    public class MyUnit
    {
        MyBountyTree tree = new MyBountyTree();
        MyLaserTurrent laserTurrent = new MyLaserTurrent();

        public void init()
        {
            tree.registerAgent(this.laserTurrent.agent);
        }

        public void addObject()
        {
            tree.addBonusObject(new MyAddAttack());
        }
    }

    class TestBountySystem
    {
        public void test()
        {
            MyUnit myUnit = new MyUnit();
        }
    }
}