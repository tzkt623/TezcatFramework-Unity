using System.Collections.Generic;
using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Framework.GraphicSystem
{
    public class TezGraphicSystem : ITezService
    {
        List<TezRenderCommand> m_Pool = new List<TezRenderCommand>();
        Queue<int> m_FreeID = new Queue<int>();
        int m_IDGiver = 0;
        Transform m_Root = null;

        List<TezRenderCommand> m_CMDs = new List<TezRenderCommand>();

        public Material drawLineMat { get; set; }
        public Material drawCircleMat { get; set; }
        public Material drawEllipseeMat { get; set; }
        public Material drawRectMat { get; set; }

        public TezGraphicSystem()
        {
            m_Pool.Capacity = 1000;

            GameObject go = new GameObject();
            go.name = this.GetType().Name;
            m_Root = go.GetComponent<Transform>();
        }

        private int giveID()
        {
            if (m_FreeID.Count > 0)
            {
                return m_FreeID.Dequeue();
            }

            return m_IDGiver++;
        }

        public void recycle(int object_id)
        {
            m_FreeID.Enqueue(object_id);
            m_Pool[object_id].close();
            m_Pool[object_id] = null;
        }

        private void add(TezRenderCommand cmd)
        {
            while (cmd.objectID >= m_Pool.Count)
            {
                m_Pool.Add(null);
            }

            m_Pool[cmd.objectID] = cmd;

            m_CMDs.Add(cmd);
        }

        public void close()
        {
            foreach (var item in m_Pool)
            {
                item?.close();
            }

            m_Pool.Clear();
            m_Pool = null;

            Object.Destroy(m_Root.gameObject);
        }


        #region Draw Function
        public void drawCircle(Vector3 center, float radius, int fragment, Color color, Transform parent, Material material)
        {
            float per = 360.0f / fragment;
            var vertex = new Vector3[fragment];
            for (int i = 0; i < fragment; i++)
            {
                var r = (i * per) * Mathf.Deg2Rad;
                var x = radius * Mathf.Sin(r);
                var z = radius * Mathf.Cos(r);
                vertex[i] = new Vector3(x, 0, z);
            }

            var cmd = new TezDrawCircle(this.giveID());
            cmd.graphicObject.transform.position = center;
            cmd.draw(vertex, color, parent, material);

            this.add(cmd);
        }

        public void drawCircle(Vector3 center, float radius, int fragment, Color color)
        {
            this.drawCircle(center, radius, fragment, color, m_Root, this.drawCircleMat);
        }

        public void drawEllipse(Vector3 center, float width, float height, int fragment, Color color, Transform parent)
        {
            this.drawEllipse(center, width, height, fragment, color, parent, this.drawEllipseeMat);
        }

        public void drawEllipse(Vector3 center, float width, float height, int fragment, Color color, Transform parent, Material material)
        {
            float per = 360.0f / fragment;
            var vertex = new Vector3[fragment];
            for (int i = 0; i < fragment; i++)
            {
                var r = (i * per) * Mathf.Deg2Rad;
                var x = width * Mathf.Sin(r);
                var z = height * Mathf.Cos(r);
                vertex[i] = new Vector3(x, 0, z);
            }

            var cmd = new TezDrawEllipse(this.giveID());
            cmd.graphicObject.transform.position = center;
            cmd.draw(vertex, color, parent, material);

            this.add(cmd);
        }

        public void drawRect(Vector3 center, float width, float height, Color color, Transform parent, Material material)
        {
            var vertex = new Vector3[4]
            {
                new Vector3(-width / 2, 0, height / 2),
                new Vector3(width / 2, 0, height / 2),
                new Vector3(width / 2, 0, -height / 2),
                new Vector3(-width / 2, 0, -height / 2)
            };

            var cmd = new TezDrawRect(this.giveID());
            cmd.graphicObject.transform.position = center;
            cmd.draw(vertex, color, parent, material);

            this.add(cmd);
        }

        public void drawRect(Vector3 center, float width, float height, Color color)
        {
            this.drawRect(center, width, height, color, m_Root, this.drawRectMat);
        }

        public void drawLine(Vector3 from, Vector3 to, Color color, Transform parent, Material material)
        {
            var center = (from + to) / 2;
            var forward = (to - from).normalized;
            var half_length = (to - from).magnitude / 2;

            from = -forward * half_length;
            to = forward * half_length;

            var cmd = new TezDrawLine(this.giveID());
            cmd.graphicObject.transform.position = center;
            cmd.draw(new Vector3[] { from, to }, color, parent, material);

            this.add(cmd);
        }


        public void drawLine(Vector3 from, Vector3 to, Color color)
        {
            this.drawLine(from, to, color, m_Root, this.drawLineMat);
        }
        #endregion

        public void clear()
        {
            for (int i = 0; i < m_CMDs.Count; i++)
            {
                m_CMDs[i].close();
            }
            m_CMDs.Clear();
        }
    }
}

