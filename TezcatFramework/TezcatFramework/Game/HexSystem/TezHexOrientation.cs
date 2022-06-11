using UnityEngine;

namespace tezcat.Framework.Game
{
    public struct TezHexOrientation
    {
        /// <summary>
        /// The 2×2 forward matrix
        /// </summary>
        public float fm1, fm2, fm3, fm4;

        /// <summary>
        /// The 2×2 inverse matrix
        /// </summary>
        public float im1, im2, im3, im4;

        /// <summary>
        /// 角度
        /// </summary>
        public float start_angle;

        public enum Orientation
        {
            Pointy,
            Flat
        }

        public Orientation orientation;

        private TezHexOrientation(
            float fm1, float fm2, float fm3, float fm4,
            float im1, float im2, float im3, float im4,
            float start_angle,
            Orientation orientation)
        {
            this.fm1 = fm1;
            this.fm2 = fm2;
            this.fm3 = fm3;
            this.fm4 = fm4;
            this.im1 = im1;
            this.im2 = im2;
            this.im3 = im3;
            this.im4 = im4;
            this.start_angle = start_angle;
            this.orientation = orientation;
        }

        public static readonly TezHexOrientation Pointy = new TezHexOrientation(
            Mathf.Sqrt(3.0f), Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f,
            Mathf.Sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f,
            0.5f,
            Orientation.Pointy);

        public static readonly TezHexOrientation Flat = new TezHexOrientation(
            3.0f / 2.0f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f, Mathf.Sqrt(3.0f),
            2.0f / 3.0f, 0.0f, -1.0f / 3.0f, Mathf.Sqrt(3.0f) / 3.0f,
            0.0f,
            Orientation.Flat);
    }
}