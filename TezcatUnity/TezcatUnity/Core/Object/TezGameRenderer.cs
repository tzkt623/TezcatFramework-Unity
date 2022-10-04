using tezcat.Framework.Core;

namespace tezcat.Unity.Core
{
    /// <summary>
    /// 主渲染对象
    /// </summary>
    public abstract class TezGameRenderer
        : TezMonoObject
    {

    }


    public interface ITezGameObjectRenderer
    {
        TezGameObject baseObject { get; }
        void bind(TezGameObject gameObject);
    }

    public abstract class TezGameObjectRenderer<T>
        : TezGameRenderer
        , ITezGameObjectRenderer
        where T : TezGameObject
    {
        protected T mBaseObject = null;

        public TezGameObject baseObject => mBaseObject;
        public T currentObject => mBaseObject;

        public void bind(TezGameObject gameObject)
        {
            mBaseObject = (T)gameObject;
        }

        public void bind(T currentObject)
        {
            this.bind(currentObject);
        }

        protected override void onClose()
        {
            mBaseObject = null;
        }
    }
}