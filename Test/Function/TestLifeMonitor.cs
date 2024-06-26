﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    public class TestLifeMonitor : TezBaseTest
    {
        /// <summary>
        /// <see cref="Ship">See Ship Memeber</see>
        /// </summary>
        Ship mShip = null;

        Missle mMissle = null;
        Missle mMissle2 = null;

        public TestLifeMonitor() : base("LifeMonitor")
        {

        }

        protected override void onClose()
        {
            mShip.close();
            mMissle.close();
            mMissle2.close();

            mShip = null;
            mMissle = null;
            mMissle2 = null;
        }

        public override void init()
        {
            var proto = TezcatFramework.protoDB.getProto<Ship>("Battleship");

            mShip = proto.spawnObject<Ship>();
            mShip.init();
            mMissle = new Missle()
            {
                name = "M1",
                step = 4
            };
            mMissle.init();
            mMissle.setTarget(mShip);

            mMissle2 = new Missle()
            {
                name = "M2",
                step = 2
            };
            mMissle2.init();
            mMissle2.setTarget(mShip);
        }

        public override void run()
        {
            int count = 6;
            while (count-- > 0)
            {
                mMissle.update();
                mMissle2.update();
                mShip.update();
            }
        }
    }
}