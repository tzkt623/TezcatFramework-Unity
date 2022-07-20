using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class Ship
    {
        public TezValider<Ship> valider = new TezValider<Ship>();
        public TezStateValider<Ship> valider2 = new TezStateValider<Ship>();

        public int health = 10;

        public void update()
        {
            this.health -= 1;
            if (this.health <= 0)
            {
                valider.setInvalid();
            }
        }
    }

    public class Missle
    {
        public TezValider<Ship> target = null;

        public void update()
        {
            if (this.target.isValied)
            {
                this.moveToTarget();
            }
            else
            {
                this.findOtherTarget();
                target.close();
                target = null;
            }
        }

        public void setTarget(Ship ship)
        {
            this.target = new TezValider<Ship>(ship.valider);
        }

        private void findOtherTarget()
        {

        }

        private void moveToTarget()
        {

        }
    }

    public class TestValider
    {
        Ship m_Ship = new Ship();
        Missle m_Missle = new Missle();

        public void init()
        {
            m_Missle.setTarget(m_Ship);
        }

        public void update()
        {
            m_Ship.update();

            m_Missle.update();
        }
    }
}