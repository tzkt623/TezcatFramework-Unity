namespace tezcat.Framework.Utility
{
    public class TezRemapF
    {
        float m_Rate;

        float m_ToMin = int.MaxValue;
        float m_ToMax = int.MinValue;

        float m_FromMin = int.MaxValue;
        float m_FromMax = int.MinValue;

        public TezRemapF() { }

        public TezRemapF(float to_min, float to_max, float from_min, float from_max)
        {
            this.set(to_min, to_max, from_min, from_max);
        }

        public void reset()
        {
            m_ToMin = int.MaxValue;
            m_ToMax = int.MinValue;

            m_FromMin = int.MaxValue;
            m_FromMax = int.MinValue;

            m_Rate = 0;
        }

        public void setMinMaxTo(float min, float max)
        {
            m_ToMin = min;
            m_ToMax = max;
        }

        public void setMinMaxFrom(float value)
        {
            if (value < m_FromMin)
            {
                m_FromMin = value;
            }

            if (value > m_FromMax)
            {
                m_FromMax = value;
            }
        }

        public void calculateRate()
        {
            m_Rate = (m_ToMax - m_ToMin) / (m_FromMax - m_FromMin);
        }

        public void set(float to_min, float to_max, float from_min, float from_max)
        {
            m_ToMin = to_min;
            m_ToMax = to_max;

            m_FromMin = from_min;
            m_FromMax = from_max;

            this.calculateRate();
        }

        public float calculate(float from_value)
        {
            return m_ToMin + m_Rate * (from_value - m_FromMin);
        }

        public static float calculate(float to_min, float to_max, float from_min, float from_max, float from_value)
        {
            return to_min + (to_max - to_min) / (from_max - from_min) * (from_value - from_min);
        }
    }
}