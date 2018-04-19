namespace tezcat.Utility
{
    public abstract class TezPropertyInjector<T, InjectValue> where T : TezPropertyValue
    {
        public abstract void inject(T property, InjectValue value);
    }
}