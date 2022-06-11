using UnityEngine;

namespace tezcat.Framework.Game.Galaxy
{
    public class TezGalaxyBody
    {
        public float offsetWidth { get; set; }
        public float offsetHeight { get; set; }

        /// <summary>
        /// 椭圆轨道的角度值
        /// </summary>
        public float angle { get; set; }

        /// <summary>
        /// 当前运行到的角度
        /// </summary>
        public float theta { get; set; }
        /// <summary>
        /// 运行的角速度
        /// </summary>
        public float velocityTheta { get; set; }

        /// <summary>
        /// 表面亮度
        /// </summary>
        public float brigtness { get; set; }

        /// <summary>
        /// 绕行中心
        /// </summary>
        public Vector3 center { get; set; } = Vector3.zero;

        /// <summary>
        /// 气温
        /// </summary>
        public float temperature { get; set; }

        public object usrdata { get; set; }
        public TezGalaxySimulator simulator { get; set; }

        public Vector3 calculateOrbit()
        {
            float alpha = this.theta * Mathf.Deg2Rad;
            float beta = angle;

            ///计算当前运行到的角度的坐标
            float sinalpha = Mathf.Sin(alpha);
            float cosalpha = Mathf.Cos(alpha);

            var x = offsetWidth * sinalpha;
            var y = offsetHeight * cosalpha;

            ///求按椭圆轨道角度偏移后的坐标
            float sinbeta = Mathf.Sin(beta);
            float cosbeta = Mathf.Cos(beta);

            ///旋转当前位置到运行角度的位置
            ///x1 = x0cosn - y0sinn
            ///y1 = x0sinn + y0cosn
            var pos_x = x * cosbeta - y * sinbeta;
            var pos_y = x * sinbeta + y * cosbeta;


            Vector3 pos = new Vector3(
                this.center.x + pos_x,
                this.center.y + pos_y,
                this.center.z);

            var pertAmp = this.simulator.pertAmp;
            var pertN = this.simulator.pertN;
            if (this.simulator.pertAmp > 0 && pertN > 0)
            {
                pos.x += (this.offsetWidth / pertAmp) * Mathf.Sin(alpha * 2 * pertN);
                pos.y += (this.offsetWidth / pertAmp) * Mathf.Cos(alpha * 2 * pertN);
            }

            return pos;
        }
    }
}