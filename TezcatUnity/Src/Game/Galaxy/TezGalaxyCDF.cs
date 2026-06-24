using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezGalaxyCDF
    {
        TezEventExtension.Function<float, float> m_pDistFun;

        float m_fMin;
        float m_fMax;
        float m_fWidth;
        int m_nSteps;

        // parameters for realistic star distribution
        float m_I0;
        float m_k;
        float m_a;
        float m_RBulge;

        List<float> m_vM1 = new List<float>();
        List<float> m_vY1 = new List<float>();
        List<float> m_vX1 = new List<float>();

        List<float> m_vM2 = new List<float>();
        List<float> m_vY2 = new List<float>();
        List<float> m_vX2 = new List<float>();

        public void setupRealistic(float I0, float k, float a, float RBulge, float min, float max, int nSteps)
        {
            m_fMin = min;
            m_fMax = max;
            m_nSteps = nSteps;

            m_I0 = I0;
            m_k = k;
            m_a = a;
            m_RBulge = RBulge;

            m_pDistFun = Intensity;

            // build the distribution function
            BuildCDF(m_nSteps);
        }

        private void BuildCDF(int nSteps)
        {
            float h = (m_fMax - m_fMin) / nSteps;
            float x = 0, y = 0;

            m_vX1.Clear();
            m_vY1.Clear();
            m_vX2.Clear();
            m_vY2.Clear();
            m_vM1.Clear();
            m_vM2.Clear();

            // Simpson rule for integration of the distribution function
            m_vY1.Add(0.0f);
            m_vX1.Add(0.0f);
            for (int i = 0; i < nSteps; i += 2)
            {
                x = (i + 2) * h;
                y += h / 3 * (m_pDistFun(m_fMin + i * h) + 4 * m_pDistFun(m_fMin + (i + 1) * h) + m_pDistFun(m_fMin + (i + 2) * h));

                m_vM1.Add((y - m_vY1[m_vY1.Count - 1]) / (2 * h));
                m_vX1.Add(x);
                m_vY1.Add(y);

                //    printf("%2.2f, %2.2f, %2.2f\n", m_fMin + (i+2) * h, v, h);
            }
            m_vM1.Add(0.0f);

            // all arrays must have the same length
            if (m_vM1.Count != m_vX1.Count || m_vM1.Count != m_vY1.Count)
            {
                throw new Exception("CumulativeDistributionFunction::BuildCDF: array size mismatch (1)!");
            }

            // normieren
            for (int i = 0; i < m_vY1.Count; ++i)
            {
                m_vY1[i] /= m_vY1[m_vY1.Count - 1];
                m_vM1[i] /= m_vY1[m_vY1.Count - 1];
            }

            //
            m_vX2.Add(0.0f);
            m_vY2.Add(0.0f);

            float p = 0;
            h = 1.0f / nSteps;
            for (int i = 1, k = 0; i < nSteps; ++i)
            {
                p = i * h;

                for (; m_vY1[k + 1] <= p; ++k)
                {
                }


                y = m_vX1[k] + (p - m_vY1[k]) / m_vM1[k];

                //    printf("%2.4f, %2.4f, k=%d, %2.4f, %2.4f\n", p, y, k, m_vY1[k], m_vM1[k]);

                m_vM2.Add((y - m_vY2[m_vY2.Count - 1]) / h);
                m_vX2.Add(p);
                m_vY2.Add(y);
            }
            m_vM2.Add(0.0f);

            // all arrays must have the same length
            if (m_vM2.Count != m_vX2.Count || m_vM2.Count != m_vY2.Count)
            {
                throw new Exception("CumulativeDistributionFunction::BuildCDF: array size mismatch (1)!");
            }
        }

        public float probFromVal(float fVal)
        {
            if (fVal < m_fMin || fVal > m_fMax)
            {
                throw new Exception("out of range");
            }

            float h = 2 * ((m_fMax - m_fMin) / m_nSteps);
            int i = (int)((fVal - m_fMin) / h);
            float remainder = fVal - i * h;

            //  printf("fVal=%2.2f; h=%2.2f; i=%d; m_vVal[i]=%2.2f; m_vAsc[i]=%2.2f;\n", fVal, h, i, m_vVal[i], m_vAsc[i]);

            //           assert(i >= 0 && i < (init)m_vM1.size());
            return (m_vY1[i] + m_vM1[i] * remainder) /* / m_vVal.back()*/;
        }

        public float valFromProb(float fVal)
        {
            if (fVal < 0 || fVal > 1)
            {
                throw new Exception("out of range");
            }

            float h = 1.0f / (m_vY2.Count - 1);

            int i = (int)(fVal / h);
            float remainder = fVal - i * h;

            //            assert(i >= 0 && i < (init)m_vM2.size());
            return (m_vY2[i] + m_vM2[i] * remainder) /* / m_vVal.back()*/;
        }

        private float IntensityBulge(float R, float I0, float k)
        {
            return I0 * Mathf.Exp(-k * Mathf.Pow(R, 0.25f));
        }

        //-------------------------------------------------------------------------------------------------
        private float IntensityDisc(float R, float I0, float a)
        {
            return I0 * Mathf.Exp(-R / a);
        }

        //-------------------------------------------------------------------------------------------------
        private float Intensity(float x)
        {
            return (x < m_RBulge) ? IntensityBulge(x, m_I0, m_k) : IntensityDisc(x - m_RBulge, IntensityBulge(m_RBulge, m_I0, m_k), m_a);
        }
    }
}