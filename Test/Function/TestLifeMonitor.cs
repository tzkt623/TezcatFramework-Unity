#define DeleteThisDefineToOtherMode

using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class Ship
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
#if DeleteThisDefineToOtherMode
            if (this.lifeMonitor.isValied)
            {
                if (this.health == 0)
                {
                    Console.WriteLine("Ship Dead");
                    this.lifeMonitor.setInvalid();
                }
            }
#else
            if (this.lifeMonitor2.isValied)
            {
                if (this.health == 0)
                {
                    Console.WriteLine("Ship Dead");
                    this.lifeMonitor2.setInvalid();
                }
            }
#endif
        }

        public void close()
        {
#if DeleteThisDefineToOtherMode
            this.lifeMonitor.close();
#else
            this.lifeMonitor2.close();
#endif
        }
    }

    public class Missle
    {
        public string name = null;
        public int step = 0;
        public TezLifeMonitor<Ship> targetT = null;
        public TezLifeMonitor target = null;
        bool stop = false;

        public void update()
        {
            if(stop)
            {
                return;
            }

#if DeleteThisDefineToOtherMode
            if (this.target == null)
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
                this.target.close();
                this.target = null;
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
            this.target = new TezLifeMonitor();
            this.target.setMonitorFrom(ship.lifeMonitor);
            //this.target.setManagedObject(ship);
#else
            this.target = new TezLifeMonitor<Ship>(ship.lifeMonitor2);
#endif
        }

        private void moveToTarget()
        {
            Console.WriteLine($"{name}: Move To Target......");
            step--;
            if (step == 0)
            {
                this.target.tryGetObject<Ship>(out var ship);
                ship.health = 0;
                Console.WriteLine($"{name} hit target!!");
                this.target.close();
                this.target = null;
                this.stop = true;
            }
        }

        private void findOtherTarget()
        {
            Console.WriteLine($"{name}: Find Other Target......");
        }

        public void close()
        {
            this.target?.close();
        }
    }

    public class TestLifeMonitor : TezBaseTest
    {
        Ship mShip = null;
        Missle mMissle = null;
        Missle mMissle2 = null;

        public TestLifeMonitor() : base("LifeMonitor")
        {
        }

        public override void close()
        {
            mShip.close();
            mMissle.close();
            mMissle2.close();
        }

        public override void init()
        {
            mShip = new Ship();
            mMissle = new Missle()
            {
                name = "M1",
                step = 4
            };
            mMissle.setTarget(mShip);

            mMissle2 = new Missle()
            {
                name = "M2",
                step = 2
            };
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