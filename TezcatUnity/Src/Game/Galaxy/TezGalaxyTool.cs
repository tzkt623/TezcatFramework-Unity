using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezGalaxyTool
    {
        #region VelocityCurve
        static float MS(float r)
        {
            float d = 2000f;  // Dicke der Scheibe
            float rho_so = 1f;  // Dichte im Mittelpunkt
            float rH = 2000f; // Radius auf dem die Dichte um die Hälfte gefallen ist
            return rho_so * Mathf.Exp(-r / rH) * (r * r) * Mathf.PI * d;
        }

        static float MH(float r)
        {
            float rho_h0 = 0.15f; // Dichte des Halos im Zentrum
            float rC = 2500f;     // typische skalenlänge im Halo
            return rho_h0 * 1 / (1 + Mathf.Pow(r / rC, 2)) * (4 * Mathf.PI * Mathf.Pow(r, 3) / 3);
        }

        // Velocity curve with dark matter
        public static float v(float r)
        {

            float MZ = 100f;
            float G = 6.672e-11f;
            return 20000f * Mathf.Sqrt(G * (MH(r) + MS(r) + MZ) / r);
        }

        // velocity curve without dark matter
        public static float vd(float r)
        {
            float MZ = 100f;
            float G = 6.672e-11f;
            return 20000f * Mathf.Sqrt(G * (MS(r) + MZ) / r);
        }
        #endregion
    }
}