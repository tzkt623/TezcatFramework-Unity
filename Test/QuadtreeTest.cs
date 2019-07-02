using System.Collections.Generic;
using tezcat.Framework.GraphicSystem;
using tezcat.Framework.Shape;
using tezcat.Framework.Utility;
using UnityEngine;

public class QuadtreeTest : MonoBehaviour
{
    [SerializeField]
    public Transform m_Mark = null;

    TezQuadtree m_Quadtree = null;
    TezGraphicSystem m_Graphic = null;
    TezGraphicSystem m_GraphicRect = null;

    Vector3 m_RectBegin = Vector3.zero;
    bool m_Drag = false;

    List<Transform> m_Objects = new List<Transform>();

    void Start()
    {
        m_Graphic = new TezGraphicSystem();
        m_GraphicRect = new TezGraphicSystem();

        var rect = new TezRectangle()
        {
            originX = 0,
            originY = 0,
            width = 200,
            height = 200
        };

        m_Quadtree = new TezQuadtree(0, rect, 5, 5);


        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, rect.height),
            new Vector3(rect.width, 0, rect.height),
            new Vector3(rect.width, 0, 0)
        };

        int[] triangle = new int[6]
        {
            0, 1, 3,
            1, 2, 3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();

        var mf = this.GetComponent<MeshFilter>();
        mf.mesh = mesh;

        var mr = this.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));
        mr.material.color = Color.black;

        this.gameObject.AddComponent<MeshCollider>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                var pos = hit.point;
                var mark = Instantiate(m_Mark);
                mark.gameObject.SetActive(true);
                mark.position = pos;

                var point = new TezQtPoint(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z))
                {
                    usrData = mark
                };
                m_Objects.Add(mark);

                m_Graphic.clear();
                m_Quadtree.insert(point);
                m_Quadtree.draw((TezRectangle rect) =>
                {
                    var point2 = new Vector3(rect.midX, 0, rect.midY);
                    m_Graphic.drawRect(point2, rect.width, rect.height, Color.red);
                });
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!m_Drag)
            {
                for (int i = 0; i < m_Objects.Count; i++)
                {
                    m_Objects[i].GetComponent<MeshRenderer>().material.color = Color.white;
                }

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    m_RectBegin = hit.point;
                    m_Drag = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                var pos = hit.point;
                var mid = (pos + m_RectBegin) / 2;
                var length = pos - m_RectBegin;

                m_GraphicRect.clear();
                m_GraphicRect.drawRect(mid, length.x, length.z, Color.green);
                m_Drag = false;

                var obj = new TezQtRectangle(
                    Mathf.CeilToInt(m_RectBegin.x),
                    Mathf.CeilToInt(m_RectBegin.z),
                    Mathf.CeilToInt(length.x),
                    Mathf.CeilToInt(length.z));

                var rectangle = obj.rectangle;

                var list = m_Quadtree.retrieve(obj);
                for (int i = 0; i < list.Count; i++)
                {
                    Transform tran = (Transform)list[i].usrData;
                    var tran_pos = tran.position;
                    if(tran_pos.x >= rectangle.originX
                        && tran_pos.z >= rectangle.originY
                        && tran_pos.x <= rectangle.maxX
                        && tran_pos.z <= rectangle.maxY)
                    {
                        tran.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    }
                }
            }
        }
    }
}
