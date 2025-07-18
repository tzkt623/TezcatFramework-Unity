﻿using System.Collections.Generic;

namespace tezcat.Unity.UI
{
    public class TezLayer : TezBaseWidget
    {
        #region Manager
        static List<TezLayer> Manager = new List<TezLayer>();
        static int sLastID = -1;

        public static TezLayer get(int index)
        {
            return Manager[index];
        }

        public static TezLayer last
        {
            get { return Manager[sLastID]; }
        }

        public static TezLayer overlay { get; protected set; } = null;

        public static void register(TezLayer layer)
        {
            if (layer.ID == -1)
            {
                layer.ID = Manager.Count;
                Manager.Add(layer);
            }
            else
            {
                while (Manager.Count <= layer.ID)
                {
                    Manager.Add(null);
                }

                Manager[layer.ID] = layer;
            }

            sLastID = Manager.Count - 1;
        }

        public static void sortLayers()
        {
            for (int i = 0; i < Manager.Count; i++)
            {
                Manager[i].sort();
            }
        }
        #endregion

        public int ID { get; private set; } = -1;

        protected virtual void sort()
        {
            this.transform.SetSiblingIndex(ID);
            this.name = $"Layer_{ID}";
        }
    }
}