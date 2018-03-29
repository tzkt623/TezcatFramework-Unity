﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace tezcat
{
    public class TezScrollCell : UIBehaviour
    {
        public Vector2 size { get; private set; } = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
            size = ((RectTransform)this.transform).rect.size;
        }

        protected override void Start()
        {
            base.Start();
        }
    }


}
