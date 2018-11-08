using System.Collections.Generic;
using tezcat.Framework.Math;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezHexCell
    {
        TezHexGrid.AxialCoordinate m_Coorinate = new TezHexGrid.AxialCoordinate();
        public TezHexGrid.CubeCoordinate cubeCoordinate
        {
            get { return m_Coorinate.toCube(); }
        }

        public TezHexGrid.AxialCoordinate axialCoordinate
        {
            get { return m_Coorinate; }
        }

        Vector3 m_Corner = Vector3.zero;
        public Vector3 corner
        {
            get { return m_Corner; }
        }

        Vector3 localPosition
        {
            get
            {
                switch (m_Grid.layout)
                {
                    case TezHexGrid.Layout.Pointy:
                        return new Vector3(
                            m_Grid.size * TezHexGrid.Sqrt3 * (m_Coorinate.q + m_Coorinate.r / 2.0f),
                            m_Grid.size * 3 / 2 * m_Coorinate.r,
                            0);
                    case TezHexGrid.Layout.Flat:
                        return new Vector3(
                            m_Grid.size * 3 / 2 * m_Coorinate.q,
                            m_Grid.size * TezHexGrid.Sqrt3 * (m_Coorinate.r + m_Coorinate.q / 2.0f),
                            0);
                }

                return Vector3.zero;
            }
        }

        public GameObject gameObject { get; set; }

        private TezHexGrid m_Grid;

        public TezHexCell()
        {
            gameObject = null;
        }

        public int getCellID()
        {
            var hash = TezHash.intHash(m_Coorinate.q);
            hash = TezHash.intHash(hash + m_Coorinate.r);
            return hash;
        }

        public static int getCellID(int q, int r)
        {
            var hash = TezHash.intHash(q);
            hash = TezHash.intHash(hash + r);
            return hash;
        }

        public void init(int x, int z, float depth_offset, TezHexGrid grid)
        {
            m_Coorinate.q = x;
            m_Coorinate.r = z;
            m_Grid = grid;

            float offset = 0;

            switch (m_Grid.layout)
            {
                case TezHexGrid.Layout.Pointy:
                    offset = z * (m_Grid.hDistance / 2);
                    m_Corner = new Vector3(x * m_Grid.hDistance + offset, depth_offset, z * m_Grid.vDistance);
                    break;
                case TezHexGrid.Layout.Flat:
                    offset = x * (m_Grid.hDistance / 2);
                    m_Corner = new Vector3(x * m_Grid.vDistance + offset, depth_offset, z * m_Grid.hDistance);
                    break;
            }
        }

        Vector3 createCorner(int index)
        {
            float angle_deg = 0;

            switch (m_Grid.layout)
            {
                case TezHexGrid.Layout.Pointy:
                    angle_deg = 60 * index + 30;
                    break;
                case TezHexGrid.Layout.Flat:
                    angle_deg = 60 * index;
                    break;
            }

            var angle_rad = Mathf.Deg2Rad * angle_deg;

            return new Vector3(
                m_Corner.x + m_Grid.size * Mathf.Cos(angle_rad),
                m_Corner.y,
                m_Corner.z + m_Grid.size * Mathf.Sin(angle_rad));
        }

        public TezHexGrid.HexMesh getHexMesh()
        {
            TezHexGrid.HexMesh mesh = new TezHexGrid.HexMesh();

            mesh.vertices = new List<Vector3>()
            {
                m_Corner,
                this.createCorner(0),
                this.createCorner(1),
                this.createCorner(2),
                this.createCorner(3),
                this.createCorner(4),
                this.createCorner(5)
            };

            mesh.indices = new List<int>()
            {
                0, 6, 5,
                0, 5, 4,
                0, 4, 3,
                0, 3, 2,
                0, 2, 1,
                0, 1, 6
            };

            return mesh;
        }

        public TezHexGrid.HexMesh getRectMesh()
        {
            TezHexGrid.HexMesh mesh = new TezHexGrid.HexMesh();

            mesh.vertices = new List<Vector3>()
            {
                new Vector3(m_Corner.x - m_Grid.cellWidth / 2, m_Corner.y, m_Corner.z + m_Grid.cellHeight / 2),
                new Vector3(m_Corner.x + m_Grid.cellWidth / 2, m_Corner.y, m_Corner.z + m_Grid.cellHeight / 2),
                new Vector3(m_Corner.x + m_Grid.cellWidth / 2, m_Corner.y, m_Corner.z - m_Grid.cellHeight / 2),
                new Vector3(m_Corner.x - m_Grid.cellWidth / 2, m_Corner.y, m_Corner.z - m_Grid.cellHeight / 2),
            };

            mesh.indices = new List<int>()
            {
                0, 2, 1,
                0, 3, 2
            };

            mesh.uv = new List<Vector2>()
            {
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0)
            };

            return mesh;
        }
    }

}

