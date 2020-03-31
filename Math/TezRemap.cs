namespace tezcat.Framework.Math
{
    public class TezRemap
    {
        float m_ToMin;
        float m_FromMin;
        float m_Rate;

        public TezRemap() { }

        public TezRemap(float to_min, float to_max, float from_min, float from_max)
        {
            this.set(to_min, to_max, from_min, from_max);
        }

        public void set(float to_min, float to_max, float from_min, float from_max)
        {
            m_ToMin = to_min;
            m_FromMin = from_min;
            m_Rate = (to_max - to_min) / (from_max - from_min);
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
