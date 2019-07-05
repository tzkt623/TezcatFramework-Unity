using UnityEngine;

namespace tezcat.Framework.GraphicSystem
{
    public abstract class TezRenderCommand
    {
        public enum Category
        {
            WithGameObject,
            PerFrame
        }

        public readonly Category category;
        public int objectID { get; }
        public TezGraphicObject graphicObject { get; protected set; }

        public TezRenderCommand(Category category, int allow_id)
        {
            this.category = category;
            this.objectID = allow_id;
        }

        public virtual void close()
        {
            UnityEngine.Object.Destroy(graphicObject.gameObject);
            graphicObject = null;
        }
    }

    public abstract class TezRenderWithGameObject : TezRenderCommand
    {
        public TezRenderWithGameObject(int allow_id) : base(Category.WithGameObject, allow_id)
        {
            var gameObject = new GameObject();
            graphicObject = gameObject.AddComponent<TezGraphicObject>();
            graphicObject.objectID = allow_id;
        }

        public abstract void draw(Vector3[] vertex_array, Color color, Transform parent, Material material);
    }

    public class TezDrawEllipse : TezRenderWithGameObject
    {
        public TezDrawEllipse(int allow_id) : base(allow_id)
        {
            graphicObject.name = "TezDrawEllipse";
        }

        public override void draw(Vector3[] vertex_array, Color color, Transform parent, Material material)
        {
            graphicObject.transform.parent = parent;

            var mr = graphicObject.gameObject.AddComponent<MeshRenderer>();
            var mf = graphicObject.gameObject.AddComponent<MeshFilter>();

            var indices = new int[vertex_array.Length + 1];
            var colors = new Color[vertex_array.Length];
            for (int i = 0; i < indices.Length - 1; i++)
            {
                indices[i] = i;
                colors[i] = color;
            }

            var mesh = new Mesh();
            mesh.name = "EllipseMesh";
            mesh.vertices = vertex_array;
            indices[indices.Length - 1] = 0;
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
            mesh.colors = colors;

            material.color = color;
            mr.material = material;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            mr.receiveShadows = false;

            mf.mesh = mesh;
        }
    }

    public class TezDrawCircle : TezRenderWithGameObject
    {
        public TezDrawCircle(int allow_id) : base(allow_id)
        {
            graphicObject.name = "TezDrawCircle";
        }

        public override void draw(Vector3[] vertex_array, Color color, Transform parent, Material material)
        {
            graphicObject.transform.parent = parent;

            var mr = graphicObject.gameObject.AddComponent<MeshRenderer>();
            var mf = graphicObject.gameObject.AddComponent<MeshFilter>();

            var indices = new int[vertex_array.Length + 1];
            var colors = new Color[vertex_array.Length];
            for (int i = 0; i < indices.Length - 1; i++)
            {
                indices[i] = i;
                colors[i] = color;
            }

            var mesh = new Mesh();
            mesh.name = "CircleMesh";
            mesh.vertices = vertex_array;
            indices[indices.Length - 1] = 0;
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
            mesh.colors = colors;

            material.color = color;
            mr.material = material;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            mr.receiveShadows = false;

            mf.mesh = mesh;
        }
    }

    public class TezDrawRect : TezRenderWithGameObject
    {
        public TezDrawRect(int allow_id) : base(allow_id)
        {
            graphicObject.name = "TezDrawRect";
        }

        public override void draw(Vector3[] vertex_array, Color color, Transform parent, Material material)
        {
            graphicObject.transform.parent = parent;

            var mr = graphicObject.gameObject.AddComponent<MeshRenderer>();
            var mf = graphicObject.gameObject.AddComponent<MeshFilter>();

            var mesh = new Mesh();
            mesh.name = "RectMesh";
            mesh.vertices = vertex_array;
            mesh.SetIndices(new int[5] { 0, 1, 2, 3, 0 }, MeshTopology.LineStrip, 0);
            mesh.colors = new Color[4] { color, color, color, color };

            material.color = color;
            mr.material = material;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            mr.receiveShadows = false;

            mf.mesh = mesh;
        }
    }

    public class TezDrawLine : TezRenderWithGameObject
    {
        public TezDrawLine(int allow_id) : base(allow_id)
        {
            graphicObject.name = "TezDrawLine";
        }

        public override void draw(Vector3[] vertex_array, Color color, Transform parent, Material material)
        {
            graphicObject.transform.parent = parent;

            var mr = graphicObject.gameObject.AddComponent<MeshRenderer>();
            var mf = graphicObject.gameObject.AddComponent<MeshFilter>();

            var mesh = new Mesh();
            mesh.name = "LineMesh";
            mesh.vertices = vertex_array;
            mesh.SetIndices(new int[2] { 0, 1 }, MeshTopology.Lines, 0);
            mesh.colors = new Color[2] { color, color };

            material.color = color;
            mr.material = material;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            mr.receiveShadows = false;

            mf.mesh = mesh;
        }
    }
}