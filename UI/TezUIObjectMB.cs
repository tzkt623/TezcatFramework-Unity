using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat
{
    public abstract class TezUIObjectMB
        : MonoBehaviour
    {
        protected void Awake()
        {
            this.onAwake();
        }

        protected void Start()
        {
            this.onStart();
        }

        protected void OnDestroy()
        {
            this.onDelete();
        }

        protected void OnDisable()
        {
            this.onDisable();
        }

        protected void OnEnable()
        {
            this.onEnable();
        }

        protected abstract void onAwake();
        protected abstract void onStart();
        protected abstract void onDelete();
        protected abstract void onDisable();
        protected abstract void onEnable();
    }
}