#define DeleteThisDefineToOtherMode

using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class Ship
#if DeleteThisDefineToOtherMode
        : ITezLifeMonitorEntry
#endif
    {
#if DeleteThisDefineToOtherMode
        public TezLifeMonitor lifeMonitor { get; } = new TezLifeMonitor();
#else
        public TezLifeMonitor<Ship> lifeMonitor2 = null;
        public TezStateMonitor<Ship> lifeMonitor3 = null;
#endif


        public int health = 5;

        public Ship()
        {
#if DeleteThisDefineToOtherMode
            this.lifeMonitor.setManagedObject(this);
#else

            this.lifeMonitor2 = new TezLifeMonitor<Ship>(this);
            this.lifeMonitor3 = new TezStateMonitor<Ship>(this);
#endif
        }

        public void update()
        {
            if (this.health == 0)
            {
                return;
            }

            this.health -= 1;
            if (this.health == 0)
            {
#if DeleteThisDefineToOtherMode
                this.lifeMonitor.close();
#else
                this.lifeMonitor2.setInvalid();
#endif

            }
        }

        public void close()
        {
#if DeleteThisDefineToOtherMode
#else
            this.lifeMonitor2.close();
#endif
        }
    }

    public class Missle
    {
        public TezLifeMonitor<Ship> target = null;
        public TezLifeMonitorSlot slot = null;

        public void update()
        {
#if DeleteThisDefineToOtherMode
            if (this.slot == null)
            {
                this.findOtherTarget();
                return;
            }

            if (this.slot.isValied)
            {
                this.moveToTarget();
            }
            else
            {
                this.slot.close();
                this.slot = null;
            }
#else
            if (target == null)
            {
                this.findOtherTarget();
                return;
            }

            if (this.target.isValied)
            {
                this.moveToTarget();
            }
            else
            {
                target.close();
                target = null;
            }
#endif

        }

        public void setTarget(Ship ship)
        {

#if DeleteThisDefineToOtherMode
            this.slot = new TezLifeMonitorSlot(ship);
#else
            this.target = new TezLifeMonitor<Ship>(ship.lifeMonitor2);
#endif
        }

        private void moveToTarget()
        {
            Console.WriteLine("Move To Target......");
        }

        private void findOtherTarget()
        {
            Console.WriteLine("Find Other Target......");
        }

        public void close()
        {

        }
    }

    public class TestLifeMonitor : TezBaseTest
    {
        Ship mShip = null;
        Missle mMissle = null;

        public TestLifeMonitor() : base("LifeMonitor")
        {
        }

        public override void close()
        {
            mShip.close();
            mMissle.close();
        }

        public override void init()
        {
            mShip = new Ship();
            mMissle = new Missle();
            mMissle.setTarget(mShip);
        }

        public override void run()
        {
            while (mShip.health > 0)
            {
                mShip.update();
                mMissle.update();
            }

            mMissle.update();
        }
    }
}