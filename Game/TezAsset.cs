using tezcat.Core;
using tezcat.String;
using UnityEngine;

namespace tezcat
{
    public class TezAsset
    {
        public TezStaticString icon_0 { get; set; }
        public TezStaticString icon_1 { get; set; }
        public TezStaticString icon_2 { get; set; }
        public TezStaticString icon_3 { get; set; }

        public Sprite sprite_0 { get; set; }
        public Sprite sprite_1 { get; set; }
        public Sprite sprite_2 { get; set; }
        public Sprite sprite_3 { get; set; }

        public ITezPrefab prefab { get; set; }

        public void clear()
        {
            icon_0 = null;
            icon_1 = null;
            icon_2 = null;
            icon_3 = null;
        }
    }
}