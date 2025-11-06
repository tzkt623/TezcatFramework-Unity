using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestEnum : TezBaseTest
    {
        TezEnum Damage = null;
        TezEnum DamagePhysic = null;
        TezEnum DamagePhysicCut = null;
        TezEnum DamagePhysicPiercing = null;

        TezEnum DamageMagic = null;
        TezEnum DamageMagicFire = null;
        TezEnum DamageMagicIce = null;


        public TestEnum() : base("Enum")
        {

        }

        public override void init()
        {
            Damage = TezEnum.registerRoot("Damage", 100);
            {
                DamagePhysic = TezEnum.registerChildNode("DamagePhysic", Damage, 10);
                {
                    DamagePhysicCut = TezEnum.registerLeafNode("DamagePhysicCut", DamagePhysic);
                    DamagePhysicPiercing = TezEnum.registerLeafNode("DamagePhysicPiercing", DamagePhysic);
                }

                DamageMagic = TezEnum.registerChildNode("DamageMagic", Damage, 20);
                {
                    DamageMagicFire = TezEnum.registerLeafNode("DamageMagicFire", DamageMagic);
                    DamageMagicIce = TezEnum.registerLeafNode("DamageMagicIce", DamageMagic);
                }
            }

            var Resist = TezEnum.registerRoot("Resist", 2000);
            {
                var ResistPhysic = TezEnum.registerChildNode("ResistPhysic", Resist, 100);
                {
                    var ResistPhysicCut = TezEnum.registerLeafNode("ResistPhysicCut", ResistPhysic);
                    var ResistPhysicPiercing = TezEnum.registerLeafNode("ResistPhysicPiercing", ResistPhysic);
                }

                var ResistMagic = TezEnum.registerChildNode("ResistMagic", Resist, 200);
                {
                    var ResistMagicFire = TezEnum.registerLeafNode("ResistMagicFire", ResistMagic);
                    var ResistMagicIce = TezEnum.registerLeafNode("ResistMagicIce", ResistMagic);
                }
            }
        }

        public override void run()
        {
            foreach (var item in TezEnum.allEnum)
            {
                string v = $"{item.Name}".PadRight(30);
                Console.WriteLine($"{v} UID:{item.UID} RootID:{item.rootID} SubtypeID:{item.subtypeID} Index:{item.innerIndex} GlobalID:{item.GlobalID} MarkValue:{item.markValue}");
            }
        }

        protected override void onClose()
        {
            TezEnum.clear();
        }
    }
}
