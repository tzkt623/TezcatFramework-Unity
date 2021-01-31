namespace tezcat.Framework.InputSystem
{
    /// <summary>
    /// 
    /// 按键配置管理器类
    /// 
    /// 具体使用方法看TestKeyConfig
    /// 
    /// </summary>
    public class UnityKeyConfigSystem : TezKeyConfigManager
    {
        public static readonly UnityKeyConfigSystem instance = new UnityKeyConfigSystem();

        private UnityKeyConfigSystem()
        {

        }
    }
}