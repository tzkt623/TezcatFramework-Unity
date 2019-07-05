using UnityEngine;
using System.Collections;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Galaxy
{
    public class TezStar
    {
        public float offsetWidth { get; set; }
        public float offsetHeight { get; set; }
        public float theta { get; set; }
        public float angle { get; set; }
        public Vector3 center { get; set; } = Vector3.zero;
        public float temperature { get; set; }

        public float velocityTheta { get; set; }

        public object usrdata { get; set; }

        public int pertN = 3;
        public float pertAmp = 30;

        public Vector3 calculateOrbit()
        {
            float beta = -angle;
            float alpha = this.theta * Mathf.Deg2Rad;

            // temporaries to save cpu time
            float cosalpha = Mathf.Cos(alpha);
            float sinalpha = Mathf.Sin(alpha);
            float cosbeta = Mathf.Cos(beta);
            float sinbeta = Mathf.Sin(beta);

            Vector3 pos = new Vector3(
                this.center.x + (offsetWidth * cosalpha * cosbeta - offsetHeight * sinalpha * sinbeta),
                this.center.y + (offsetWidth * cosalpha * sinbeta + offsetHeight * sinalpha * cosbeta),
                this.center.z);

            if (pertAmp > 0 && pertN > 0)
            {
                pos.x += (this.offsetWidth / pertAmp) * Mathf.Sin(alpha * 2 * pertN);
                pos.y += (this.offsetWidth / pertAmp) * Mathf.Cos(alpha * 2 * pertN);
            }

            return pos;
        }
    }

    public class TezGalaxySimulator
    {
        public float innerExcentricity { get; set; }
        public float outerExcentricity { get; set; }

        public float angleOffset { get; set; }

        /// <summary>
        /// 核心半径
        /// </summary>
        public float radiusCore { get; set; }
        /// <summary>
        /// 星系半径
        /// </summary>
        public float radiusGalaxy { get; set; }

        /// <summary>
        /// 最大轨道范围
        /// </summary>
        public float radiusFarField { get; set; }

        public int starCount { get; set; }

        TezStar[] m_Stars = null;

        public void foreachStar(TezEventExtension.Action<TezStar> function)
        {
            for (int i = 0; i < this.starCount; i++)
            {
                function(m_Stars[i]);
            }
        }

        public void generate(
            float galaxy_radius,
            float core_radius,
            float angle_offset,
            float inner_excentricity,
            float outer_excentricity,
            int star_count)
        {
            this.radiusGalaxy = galaxy_radius;
            this.radiusCore = core_radius;
            this.radiusFarField = galaxy_radius * 2;
            this.angleOffset = angle_offset;
            this.innerExcentricity = inner_excentricity;
            this.outerExcentricity = outer_excentricity;
            this.starCount = star_count;

            m_Stars = new TezStar[this.starCount];

            var center_star = new TezStar();
            center_star.offsetWidth = 0;
            center_star.offsetHeight = 0;
            center_star.theta = 0;
            center_star.angle = 0;
            center_star.velocityTheta = 0;
            center_star.temperature = 6000;
            m_Stars[0] = center_star;

            var second_star = new TezStar();
            second_star.offsetWidth = this.radiusCore;
            second_star.offsetHeight = this.radiusCore * this.calculateExcentricity(this.radiusCore);
            second_star.theta = 0;
            second_star.angle = this.calculateAngleOffset(radiusCore);
            second_star.velocityTheta = this.calculateOrbitalVelocity((second_star.offsetWidth + second_star.offsetHeight) / 2.0f);
            second_star.temperature = 6000;
            m_Stars[1] = second_star;

            var third_star = new TezStar();
            third_star.offsetWidth = this.radiusGalaxy;
            third_star.offsetHeight = this.radiusGalaxy * this.calculateExcentricity(this.radiusGalaxy);
            third_star.theta = 0;
            third_star.angle = this.calculateAngleOffset(radiusGalaxy);
            third_star.velocityTheta = this.calculateOrbitalVelocity((third_star.offsetWidth + third_star.offsetHeight) / 2.0f);
            third_star.temperature = 6000;
            m_Stars[2] = third_star;

            TezGalaxyCDF CDF = new TezGalaxyCDF();
            CDF.setupRealistic(
                1.0f,
                0.02f,
                this.radiusGalaxy / 3.0f,
                this.radiusCore,
                0,
                this.radiusFarField,
                1000);

            for (int i = 3; i < this.starCount; i++)
            {
                var radius = CDF.valFromProb(Random.value);

                var star = new TezStar();
                star.offsetWidth = radius;
                star.offsetHeight = radius * this.calculateExcentricity(radius);
                star.theta = Random.value * 360;
                star.angle = this.calculateAngleOffset(radius);
                star.velocityTheta = this.calculateOrbitalVelocity((star.offsetWidth + star.offsetHeight) / 2.0f);
                star.temperature = 6000 + (4000 * Random.value) - 2000;
                m_Stars[i] = star;
                star.center = new Vector3(0, 0, (Random.value * 2 - 1) * 40);
            }
        }

        private float calculateExcentricity(float radius)
        {
            if (radius < this.radiusCore)
            {
                // Core region of the galaxy. Innermost part is round
                // excentricity increasing linear to the border of the core.
                return 1 + (radius / this.radiusCore) * (this.innerExcentricity - 1);
            }
            else if (radius > this.radiusCore && radius <= this.radiusGalaxy)
            {
                return this.innerExcentricity + (radius - this.radiusCore) / (this.radiusGalaxy - this.radiusCore) * (this.outerExcentricity - this.innerExcentricity);
            }
            else if (radius > this.radiusGalaxy && radius < this.radiusFarField)
            {
                // excentricity is slowly reduced to 1.
                return this.outerExcentricity + (radius - this.radiusGalaxy) / (this.radiusFarField - this.radiusGalaxy) * (1 - this.outerExcentricity);
            }

            return 1;
        }

        private float calculateAngleOffset(float radius)
        {
            return radius * this.angleOffset;
        }

        private float calculateOrbitalVelocity(float radius)
        {
            float vel_kms = 0;  // velovity in kilometer per seconds
            vel_kms = v(radius);

            // Calculate velocity in degree per year
            float u = 2 * Mathf.PI * radius * 3.08567758129e13f;        // Umfang in km
            float time = u / (vel_kms * 365.25f * 86400);  // Umlaufzeit in Jahren

            return 360.0f / time;                                   // Grad pro Jahr
        }

        #region VelocityCurve
        static float MS(float r)
        {
            float d = 2000;  // Dicke der Scheibe
            float rho_so = 1;  // Dichte im Mittelpunkt
            float rH = 2000; // Radius auf dem die Dichte um die Hälfte gefallen ist
            return rho_so * Mathf.Exp(-r / rH) * (r * r) * Mathf.PI * d;
        }

        static float MH(float r)
        {
            float rho_h0 = 0.15f; // Dichte des Halos im Zentrum
            float rC = 2500;     // typische skalenlänge im Halo
            return rho_h0 * 1 / (1 + Mathf.Pow(r / rC, 2)) * (4 * Mathf.PI * Mathf.Pow(r, 3) / 3);
        }

        // Velocity curve with dark matter
        static float v(float r)
        {
            float MZ = 100;
            float G = 6.672e-11f;
            return 20000 * Mathf.Sqrt(G * (MH(r) + MS(r) + MZ) / r);
        }

        // velocity curve without dark matter
        static float vd(float r)
        {
            float MZ = 100;
            float G = 6.672e-11f;
            return 20000 * Mathf.Sqrt(G * (MS(r) + MZ) / r);
        }
        #endregion

    }
}