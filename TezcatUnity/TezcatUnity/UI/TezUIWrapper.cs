using tezcat.Framework.Core;

namespace tezcat.Unity.UI
{
    /// <summary>
    /// Object转换到UI的包装层
    /// UI-Wrapper-Objct的方式将Object绑定到UI层
    /// 可以非常好的分离UI与Object
    /// 例如
    /// 可以在Wrapper层实现UI上显示名称的翻译与图标的查找等
    /// </summary>
    public abstract class TezUIWrapper : ITezCloseable
    {
        public void close()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }
}