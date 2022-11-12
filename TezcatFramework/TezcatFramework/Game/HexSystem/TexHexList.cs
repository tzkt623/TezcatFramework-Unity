using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public interface ITezHexBlock
    {
        TezHexCubeCoordinate coordinate { get; }
    }

    public class TexHexList<T> where T : ITezHexBlock
    {
        class Wrapper
        {
            /// <summary>
            /// 1,1
            /// 1,-1
            /// -1,1
            /// -1,-1
            /// </summary>
            T[] mArray = new T[4];

            public T get(int q, int r)
            {
                return mArray[(q < 0 ? 2 : 0) + (r < 0 ? 1 : 0)];
            }

            public void add(T node)
            {
                var c = node.coordinate;
                mArray[(c.q < 0 ? 2 : 0) + (c.r < 0 ? 1 : 0)] = node;
            }

            public void close()
            {
                mArray = null;
            }
        }

//        Wrapper[,] m_List = new Wrapper[1, 1];

        List<Wrapper> mList = new List<Wrapper>();

        public void add(T node)
        {
            var coordinate = node.coordinate;
            var q = Mathf.Abs(coordinate.q);
            var r = Mathf.Abs(coordinate.r);

        }

        public T get(int q, int r)
        {
            var x = Mathf.Abs(q);
            var y = Mathf.Abs(r);

            return default(T);
        }

    }
}