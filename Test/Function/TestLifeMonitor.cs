using tezcat.Framework.ArchetypeECS;

namespace tezcat.Framework.Test
{
    public class TestLifeMonitor : TezBaseTest
    {
        /// <summary>
        /// <see cref="Ship">See Ship Memeber</see>
        /// </summary>
        TezWorld.Entity mShipEntity;

        TezWorld.Entity mMissle;
        TezWorld.Entity mMissle2;

        public TestLifeMonitor() : base("LifeMonitor")
        {

        }

        protected override void onClose()
        {
            TezWorld.instance.removeEntity(mShipEntity);
            TezWorld.instance.removeEntity(mMissle);
            TezWorld.instance.removeEntity(mMissle2);
        }

        public override void init()
        {
            //var proto = TezcatFramework.protoDB.getProto<Ship>("Battleship");

            var data = TezcatFramework.protoDB.createObjectData<ShipData>("Battleship", Core.TezProtoObjectCreateMode.New);
            mShipEntity = data.instantiateEntity();
            var ship = TezWorld.instance.getComponent<ComUnit, Ship>(mShipEntity);

            mMissle = TezWorld.instance.createEntity<EntityMaskID_Weapon>();
            TezWorld.instance.initComponent<ComWeaponData, MissleData>(mMissle);
            var missle = TezWorld.instance.initComponent<ComWeapon, Missle>(mMissle);
            missle.name = "M1";
            missle.step = 4;
            missle.setTarget(ship);

            mMissle2 = TezWorld.instance.createEntity<EntityMaskID_Weapon>();
            TezWorld.instance.initComponent<ComWeaponData, MissleData>(mMissle2);
            var missle2 = TezWorld.instance.initComponent<ComWeapon, Missle>(mMissle2);
            missle2.name = "M2";
            missle2.step = 2;
            missle2.setTarget(ship);
        }

        public override void run()
        {
            int count = 6;
            var weapon_set = TezWorld.instance.query(new TezWorld.ArchetypeKey(TezWorld.ComponentID<ComWeapon>.ID));
            var unit_set = TezWorld.instance.query(new TezWorld.ArchetypeKey(TezWorld.ComponentID<ComUnit>.ID));

            while (count-- > 0)
            {
                foreach (var chunk in weapon_set)
                {
                    var list = chunk.getComponents(TezWorld.ComponentID<ComWeapon>.ID);
                    foreach (var item in list)
                    {
                        ((ComWeapon)item).update();
                    }
                }

                foreach (var chunk in unit_set)
                {
                    var list = chunk.getComponents(TezWorld.ComponentID<ComUnit>.ID);
                    foreach (var item in list)
                    {
                        ((ComUnit)item).update();
                    }
                }

//                 mMissle.update();
//                 mMissle2.update();
//                 mShipEntity.update();
            }
        }
    }
}