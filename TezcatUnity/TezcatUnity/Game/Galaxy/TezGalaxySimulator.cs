using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezGalaxySimulator
    {
        float m_InnerExcentricity;
        public float innerExcentricity
        {
            get { return m_InnerExcentricity; }
            set
            {
                m_InnerExcentricity = value;
                this.refresh();
            }
        }

        float m_OuterExcentricity;
        public float outerExcentricity
        {
            get { return m_OuterExcentricity; }
            set
            {
                m_OuterExcentricity = value;
                this.refresh();
            }
        }

        float m_AngleOffset = 0;
        public float angleOffset
        {
            get { return m_AngleOffset; }
            set
            {
                m_AngleOffset = value;
                this.refresh();
            }
        }

        /// <summary>
        /// 核心半径
        /// </summary>
        float m_RadiusCore = 0;
        public float radiusCore
        {
            get
            {
                return m_RadiusCore;
            }
            set
            {
                m_RadiusCore = value;
                this.refresh();
            }
        }

        /// <summary>
        /// 星系半径
        /// </summary>
        float m_RadiusGalaxy = 0;
        public float radiusGalaxy
        {
            get
            {
                return m_RadiusGalaxy;
            }
            set
            {
                m_RadiusGalaxy = value;
                this.radiusFarField = m_RadiusGalaxy * 2;
                this.refresh();
            }
        }

        int m_PertN;
        public int pertN
        {
            get
            {
                return m_PertN;
            }
            set
            {
                m_PertN = value;
                //                this.refresh();
            }
        }

        float m_PertAmp;
        public float pertAmp
        {
            get
            {
                return m_PertAmp;
            }
            set
            {
                m_PertAmp = value;
                //               this.refresh();
            }
        }

        /// <summary>
        /// 最大轨道范围
        /// </summary>
        public float radiusFarField { get; set; }

        public int starCount { get; set; }
        public int dustCount { get; set; }
        public int h2Count { get; set; } = 300;

        TezGalaxyBody[] m_Stars = null;
        TezGalaxyBody[] m_Dust = null;
        TezGalaxyBody[] m_H2 = null;

        TezGalaxyCDF m_CDF = new TezGalaxyCDF();

        public void foreachStar(TezEventExtension.Action<TezGalaxyBody> function)
        {
            for (int i = 0; i < this.starCount; i++)
            {
                function(m_Stars[i]);
            }
        }

        public void foreachDust(TezEventExtension.Action<TezGalaxyBody> function)
        {
            for (int i = 0; i < this.dustCount; i++)
            {
                function(m_Dust[i]);
            }
        }

        public void foreachH2(TezEventExtension.Action<TezGalaxyBody> function)
        {
            var count = this.h2Count * 2;
            for (int i = 0; i < count; i++)
            {
                function(m_H2[i]);
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
            m_RadiusGalaxy = galaxy_radius;
            this.radiusFarField = m_RadiusGalaxy * 2;
            m_RadiusCore = core_radius;
            m_AngleOffset = angle_offset;

            m_InnerExcentricity = inner_excentricity;
            m_OuterExcentricity = outer_excentricity;

            this.starCount = star_count;
            this.dustCount = star_count / 2;

            #region Star
            m_Stars = new TezGalaxyBody[this.starCount];

            var center_star = new TezGalaxyBody() { simulator = this };
            center_star.offsetWidth = 0;
            center_star.offsetHeight = 0;
            center_star.theta = 0;
            center_star.angle = 0;
            center_star.velocityTheta = 0;
            center_star.temperature = 6000;
            m_Stars[0] = center_star;

            var second_star = new TezGalaxyBody() { simulator = this };
            second_star.offsetWidth = this.radiusCore;
            second_star.offsetHeight = this.radiusCore * this.calculateExcentricity(this.radiusCore);
            second_star.theta = 0;
            second_star.angle = this.calculateAngleOffset(radiusCore);
            second_star.velocityTheta = this.calculateOrbitalVelocity((second_star.offsetWidth + second_star.offsetHeight) / 2.0f);
            second_star.temperature = 6000;
            m_Stars[1] = second_star;

            var third_star = new TezGalaxyBody() { simulator = this };
            third_star.offsetWidth = this.radiusGalaxy;
            third_star.offsetHeight = this.radiusGalaxy * this.calculateExcentricity(this.radiusGalaxy);
            third_star.theta = 0;
            third_star.angle = this.calculateAngleOffset(radiusGalaxy);
            third_star.velocityTheta = this.calculateOrbitalVelocity((third_star.offsetWidth + third_star.offsetHeight) / 2.0f);
            third_star.temperature = 6000;
            m_Stars[2] = third_star;

            m_CDF.setupRealistic(
                1.0f,
                0.02f,
                this.radiusGalaxy / 3.0f,
                this.radiusCore,
                0,
                this.radiusFarField,
                1000);

            for (int i = 3; i < this.starCount; i++)
            {
                var radius = m_CDF.valFromProb(Random.value);

                var star = new TezGalaxyBody() { simulator = this };
                star.offsetWidth = radius;
                star.offsetHeight = radius * this.calculateExcentricity(radius);
                star.angle = this.calculateAngleOffset(radius);
                star.theta = Random.value * 360.0f;
                star.velocityTheta = this.calculateOrbitalVelocity((star.offsetWidth + star.offsetHeight) / 2.0f);
                star.center = new Vector3(0, 0, ((Random.value * 2) - 1) * 40f);
                star.temperature = 6000 + (4000 * Random.value) - 2000;
                star.brigtness = 0.3f + 0.2f * Random.value;
                m_Stars[i] = star;
            }
            #endregion


            #region Dust
            m_Dust = new TezGalaxyBody[this.dustCount];
            float x, y, rad;
            for (int i = 0; i < this.dustCount; i++)
            {
                if (i % 4 == 0)
                {
                    rad = m_CDF.valFromProb(Random.value);
                }
                else
                {
                    x = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                    y = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                    rad = Mathf.Sqrt(x * x + y * y);
                }

                var dust = new TezGalaxyBody() { simulator = this };
                dust.offsetWidth = rad;
                dust.offsetHeight = rad * this.calculateExcentricity(rad);
                dust.angle = this.calculateAngleOffset(rad);
                dust.theta = 360.0f * Random.value;
                dust.velocityTheta = this.calculateOrbitalVelocity((dust.offsetWidth + dust.offsetHeight) / 2.0f);
                dust.center = new Vector3(0, 0, ((Random.value * 2) - 1) * 40f);
                dust.temperature = 5000 + rad / 4.5f;
                dust.brigtness = 0.015f + 0.01f * Random.value;
                m_Dust[i] = dust;

            }
            #endregion

            #region H2
            m_H2 = new TezGalaxyBody[this.h2Count * 2];
            for (int i = 0; i < h2Count; ++i)
            {
                x = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                y = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                rad = Mathf.Sqrt(x * x + y * y);

                int k1 = 2 * i;
                var h2_1 = new TezGalaxyBody() { simulator = this };
                h2_1.offsetWidth = rad;
                h2_1.offsetHeight = rad * this.calculateExcentricity(rad);
                h2_1.angle = this.calculateAngleOffset(rad);
                h2_1.theta = 360.0f * Random.value;
                h2_1.velocityTheta = this.calculateOrbitalVelocity((h2_1.offsetWidth + h2_1.offsetHeight) / 2.0f);
                h2_1.center = new Vector3(0, 0, ((Random.value * 2) - 1) * 40f);
                h2_1.temperature = 6000 + (6000 * Random.value) - 3000;
                h2_1.brigtness = 0.1f + 0.05f * Random.value;
                m_H2[k1] = h2_1;

                // Create second point 100 pc away from the first one
                var h2_2 = new TezGalaxyBody() { simulator = this };
                int dist = 1000;
                int k2 = 2 * i + 1;
                h2_2.offsetWidth = rad + dist;
                h2_2.offsetHeight = rad /*+ dist*/ * this.calculateExcentricity(rad /*+ dist*/);
                h2_2.angle = this.calculateAngleOffset(rad);
                h2_2.theta = h2_1.theta;
                h2_2.velocityTheta = h2_1.velocityTheta;
                h2_2.center = h2_1.center;
                h2_2.temperature = h2_1.temperature;
                h2_2.brigtness = h2_1.brigtness;
                m_H2[k2] = h2_2;
            }
            #endregion
        }

        private void refresh()
        {
            #region Star
            var center_star = m_Stars[0];
            center_star.offsetWidth = 0;
            center_star.offsetHeight = 0;
            center_star.theta = 0;
            center_star.angle = 0;
            center_star.velocityTheta = 0;
            center_star.temperature = 6000;

            var second_star = m_Stars[1];
            second_star.offsetWidth = this.radiusCore;
            second_star.offsetHeight = this.radiusCore * this.calculateExcentricity(this.radiusCore);
            second_star.theta = 0;
            second_star.angle = this.calculateAngleOffset(radiusCore);
            second_star.velocityTheta = this.calculateOrbitalVelocity((second_star.offsetWidth + second_star.offsetHeight) / 2.0f);
            second_star.temperature = 6000;

            var third_star = m_Stars[2];
            third_star.offsetWidth = this.radiusGalaxy;
            third_star.offsetHeight = this.radiusGalaxy * this.calculateExcentricity(this.radiusGalaxy);
            third_star.theta = 0;
            third_star.angle = this.calculateAngleOffset(radiusGalaxy);
            third_star.velocityTheta = this.calculateOrbitalVelocity((third_star.offsetWidth + third_star.offsetHeight) / 2.0f);
            third_star.temperature = 6000;

            m_CDF.setupRealistic(
                1.0f,
                0.02f,
                this.radiusGalaxy / 3.0f,
                this.radiusCore,
                0,
                this.radiusFarField,
                1000);

            for (int i = 3; i < this.starCount; i++)
            {
                var radius = m_CDF.valFromProb(Random.value);

                var star = m_Stars[i];
                star.offsetWidth = radius;
                star.offsetHeight = radius * this.calculateExcentricity(radius);
                star.angle = this.calculateAngleOffset(radius);
                star.velocityTheta = this.calculateOrbitalVelocity((star.offsetWidth + star.offsetHeight) / 2.0f);
            }
            #endregion

            #region Dust
            float x, y, rad;
            for (int i = 0; i < this.dustCount; i++)
            {
                if (i % 4 == 0)
                {
                    rad = m_CDF.valFromProb(Random.value);
                }
                else
                {
                    x = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                    y = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                    rad = Mathf.Sqrt(x * x + y * y);
                }

                var dust = m_Dust[i];
                dust.offsetWidth = rad;
                dust.offsetHeight = rad * this.calculateExcentricity(rad);
                dust.angle = this.calculateAngleOffset(rad);
                dust.velocityTheta = this.calculateOrbitalVelocity((dust.offsetWidth + dust.offsetHeight) / 2.0f);
            }
            #endregion

            #region H2
            for (int i = 0; i < h2Count; ++i)
            {
                x = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                y = 2 * this.radiusGalaxy * Random.value - this.radiusGalaxy;
                rad = Mathf.Sqrt(x * x + y * y);

                int k1 = 2 * i;
                var h2_1 = m_H2[k1];
                h2_1.offsetWidth = rad;
                h2_1.offsetHeight = rad * this.calculateExcentricity(rad);
                h2_1.angle = this.calculateAngleOffset(rad);
                h2_1.theta = 360.0f * Random.value;
                h2_1.velocityTheta = this.calculateOrbitalVelocity((h2_1.offsetWidth + h2_1.offsetHeight) / 2.0f);

                // Create second point 100 pc away from the first one
                int dist = 1000;
                int k2 = 2 * i + 1;
                var h2_2 = m_H2[k2];
                h2_2.offsetWidth = rad + dist;
                h2_2.offsetHeight = rad /*+ dist*/ * this.calculateExcentricity(rad /*+ dist*/);
                h2_2.angle = this.calculateAngleOffset(rad);
                h2_2.theta = h2_1.theta;
                h2_2.velocityTheta = h2_1.velocityTheta;
            }
            #endregion
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
            vel_kms = TezGalaxyTool.v(radius);

            // Calculate velocity in degree per year
            float u = 2f * Mathf.PI * radius * 3.08567758129e13f;        // Umfang in km
            float time = u / (vel_kms * 365.25f * 86400f);  // Umlaufzeit in Jahren

            return 360.0f / time;                                   // Grad pro Jahr
        }
    }
}