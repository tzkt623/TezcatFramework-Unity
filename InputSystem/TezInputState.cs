using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.InputSystem
{
    public abstract class TezInputState
    {
        public string name { get; protected set; }
        public bool blocked { get; protected set; } = false;

        protected static readonly EventSystem m_EventSystem = EventSystem.current;

        public TezInputState()
        {
            this.name = this.GetType().Name;
        }

        protected bool hoverUI
        {
            get { return m_EventSystem.IsPointerOverGameObject(); }
        }

        public virtual void update()
        {
            //             if (Input.GetKeyUp(KeyCode.F12))
            //             {
            //                 TezService.get<TezcatFramework>().createWindow<TezcatToolWindow>("TezcatToolWindow", TezLayer.last).open();
            //             }
        }

        public virtual void onEnter()
        {
            Debug.Log(string.Format("InputState >> Enter {0}", name));
        }
        public virtual void onExit()
        {
            Debug.Log(string.Format("InputState >> Exit {0}", name));
        }

        public virtual void setExtraData(ITezTuple data)
        {

        }
    }
}

