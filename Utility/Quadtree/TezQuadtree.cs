#if false
#define OptimizeSearch
#endif

using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Shape;
using UnityEngine;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// TODO:优化四叉树的插入 先查找大概位置
    /// </summary>
    /// <typeparam name="Object"></typeparam>
    public class TezQuadtree
    {
        const int Quadrant_1 = 0;
        const int Quadrant_2 = 1;
        const int Quadrant_3 = 2;
        const int Quadrant_4 = 3;

        public event TezEventExtension.Action onRefresh;

        public int maxLevel { get; set; }
        public int maxObject { get; set; }

        bool m_IsSubdivided = false;
        List<TezQtEntry> m_Objects = null;
        TezQuadtree[] m_Nodes = null;
        public TezRectangle bounds { get; private set; }
        int m_Level = 0;

        public TezQuadtree(int level, TezRectangle bounds, int max_level, int max_object)
        {
            this.maxLevel = max_level;
            this.maxObject = max_object;

            m_Level = level;
            m_Nodes = new TezQuadtree[4];
            this.bounds = bounds;
            m_Objects = new List<TezQtEntry>(max_object);
        }

        public void clear()
        {
            m_Objects.Clear();

            for (int i = 0; i < m_Nodes.Length; i++)
            {
                m_Nodes[i]?.clear();
                m_Nodes[i] = null;
            }
        }

        private void subdivide()
        {
            int half_width_l = bounds.width >> 1;
            int half_height_b = bounds.height >> 1;

            int half_width_r = half_width_l;
            int half_height_t = half_height_b;

            int ox = bounds.originX;
            int oy = bounds.originY;

            m_Nodes[Quadrant_1] = new TezQuadtree(
                m_Level + 1,
                new TezRectangle()
                {
                    originX = ox + half_width_l,
                    originY = oy + half_height_b,
                    width = half_width_r,
                    height = half_height_t
                },
                this.maxLevel,
                this.maxObject);

            m_Nodes[Quadrant_2] = new TezQuadtree(
                m_Level + 1,
                new TezRectangle()
                {
                    originX = ox,
                    originY = oy + half_height_b,
                    width = half_width_l,
                    height = half_height_t
                },
                this.maxLevel,
                this.maxObject);

            m_Nodes[Quadrant_3] = new TezQuadtree(
                m_Level + 1,
                new TezRectangle()
                {
                    originX = ox,
                    originY = oy,
                    width = half_width_l,
                    height = half_height_b
                },
                this.maxLevel,
                this.maxObject);

            m_Nodes[Quadrant_4] = new TezQuadtree(
                m_Level + 1,
                new TezRectangle()
                {
                    originX = ox + half_width_l,
                    originY = oy,
                    width = half_width_r,
                    height = half_height_b
                },
                this.maxLevel,
                this.maxObject);
        }

        private int calculateQuadrant(TezShape shape)
        {
            bool in_left = shape.originX <= bounds.midX;
            bool in_bottom = shape.originY <= bounds.midY;

            if (in_left)
            {
                if (in_bottom)
                {
                    return Quadrant_3;
                }
                else
                {
                    return Quadrant_2;
                }
            }
            else
            {
                if (in_bottom)
                {
                    return Quadrant_4;
                }
                else
                {
                    return Quadrant_1;
                }
            }
        }

        public bool insert(TezQtEntry obj)
        {
            ///形状没有被完美包含在范围内
            if (!bounds.contains(obj.shape))
            {
                return false;
            }

            if (!m_IsSubdivided && m_Objects.Count < this.maxObject)
            {
                m_Objects.Add(obj);
                return true;
            }

            if (!m_IsSubdivided)
            {
                if(m_Level + 1 == this.maxLevel)
                {
                    Debug.Log("超过最大分裂值,留在当前");
                    m_Objects.Add(obj);
                    return true;
                }

                m_IsSubdivided = true;

                this.subdivide();

                for (int oi = m_Objects.Count - 1; oi >= 0; oi--)
                {
                    var old = m_Objects[oi];
#if OptimizeSearch
                    if (m_Nodes[this.calculateQuadrant(obj.shape)].insert(obj))
                    {
                        m_Objects.RemoveAt(oi);
                    }
#else
                    if (m_Nodes[Quadrant_1].insert(old))
                    {
                        m_Objects.RemoveAt(oi);
                        continue;
                    }

                    if (m_Nodes[Quadrant_2].insert(old))
                    {
                        m_Objects.RemoveAt(oi);
                        continue;
                    }

                    if (m_Nodes[Quadrant_3].insert(old))
                    {
                        m_Objects.RemoveAt(oi);
                        continue;
                    }

                    if (m_Nodes[Quadrant_4].insert(old))
                    {
                        m_Objects.RemoveAt(oi);
                        continue;
                    }
#endif
                }
            }

#if OptimizeSearch
            var quadrant = this.calculateQuadrant(obj.shape);
            if (m_Nodes[quadrant].insert(obj))
            {
                return true;
            }
#else
            if (m_Nodes[Quadrant_1].insert(obj))
            {
                return true;
            }

            if (m_Nodes[Quadrant_2].insert(obj))
            {
                return true;
            }

            if (m_Nodes[Quadrant_3].insert(obj))
            {
                return true;
            }

            if (m_Nodes[Quadrant_4].insert(obj))
            {
                return true;
            }
#endif

            Debug.Log("插入失败,超出数量,留在父节点");
//            m_Objects.Add(obj);
            return false;
        }

        public List<TezQtEntry> retrieve(TezQtEntry obj)
        {
            List<TezQtEntry> result = new List<TezQtEntry>();

            var shape = obj.shape;
            if (!bounds.intersects(shape))
            {
                return result;
            }

            if (m_Objects.Count > 0)
            {
                result.AddRange(m_Objects);
            }

            if (m_IsSubdivided)
            {
                m_Nodes[Quadrant_1].retrieve(shape, ref result);
                m_Nodes[Quadrant_2].retrieve(shape, ref result);
                m_Nodes[Quadrant_3].retrieve(shape, ref result);
                m_Nodes[Quadrant_4].retrieve(shape, ref result);
            }

            return result;
        }

        private void retrieve(TezShape shape, ref List<TezQtEntry> result)
        {
            if (!bounds.intersects(shape))
            {
                return;
            }

            if (m_Objects.Count > 0)
            {
                result.AddRange(m_Objects);
            }

            if (m_IsSubdivided)
            {
                m_Nodes[Quadrant_1].retrieve(shape, ref result);
                m_Nodes[Quadrant_2].retrieve(shape, ref result);
                m_Nodes[Quadrant_3].retrieve(shape, ref result);
                m_Nodes[Quadrant_4].retrieve(shape, ref result);
            }
        }

        public void refresh()
        {
            List<TezQtEntry> list = new List<TezQtEntry>();

            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (m_Objects[i].dead)
                {
                    m_Objects[i].close();
                    m_Objects.RemoveAt(i);
                }
                else if (!bounds.contains(m_Objects[i].shape))
                {
                    list.Add(m_Objects[i]);
                    m_Objects.RemoveAt(i);
                }
            }

            if (m_IsSubdivided)
            {
                m_Nodes[Quadrant_1].refresh(ref list);
                m_Nodes[Quadrant_2].refresh(ref list);
                m_Nodes[Quadrant_3].refresh(ref list);
                m_Nodes[Quadrant_4].refresh(ref list);
            }

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    this.insert(list[i]);
                }
            }

            list.Clear();
            list = null;

            onRefresh?.Invoke();
        }

        private void refresh(ref List<TezQtEntry> list)
        {
            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (m_Objects[i].dead)
                {
                    m_Objects[i].close();
                    m_Objects.RemoveAt(i);
                }
                else if (!bounds.contains(m_Objects[i].shape))
                {
                    list.Add(m_Objects[i]);
                    m_Objects.RemoveAt(i);
                }
            }

            if (m_IsSubdivided)
            {
                m_Nodes[Quadrant_1].refresh(ref list);
                m_Nodes[Quadrant_2].refresh(ref list);
                m_Nodes[Quadrant_3].refresh(ref list);
                m_Nodes[Quadrant_4].refresh(ref list);
            }
        }

        public void close()
        {
            m_Objects.Clear();

            for (int i = 0; i < m_Nodes.Length; i++)
            {
                m_Nodes[i]?.close();
                m_Nodes[i] = null;
            }
        }

        public void draw(TezEventExtension.Action<TezRectangle> draw_function)
        {
            if (m_IsSubdivided)
            {
                m_Nodes[Quadrant_1].draw(draw_function);
                m_Nodes[Quadrant_2].draw(draw_function);
                m_Nodes[Quadrant_3].draw(draw_function);
                m_Nodes[Quadrant_4].draw(draw_function);
            }

            draw_function(bounds);
        }
    }
}