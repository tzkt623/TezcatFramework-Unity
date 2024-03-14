using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezAffixType
    {
        Pre, Suf
    }

    public interface ITezAffix : ITezCloseable
    {
        int typeID { get; }
        int indexID { get; }
        string name { get; }
        TezAffixType affixType { get; }
    }

    public abstract class TezPrefix : TezAffix
    {
        public override TezAffixType affixType => TezAffixType.Pre;
    }

    public abstract class TezSuffix : TezAffix
    {
        public override TezAffixType affixType => TezAffixType.Suf;
    }

    public abstract class TezAffix
        : ITezAffix
    {
        public abstract int typeID { get; }
        public abstract int indexID { get; }
        public abstract string name { get; }
        public abstract TezAffixType affixType { get; }
        public abstract void close();
    }
}

