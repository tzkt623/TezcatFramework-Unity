﻿namespace tezcat.Wrapper
{
    public interface ITezWrapper
    {
        string name { get; }
        string description { get; }
        void clear();
    }

    public interface ITezObjectWrapper : ITezWrapper
    {

    }

    public interface ITezItemWrapper : ITezWrapper
    {
        int storeID { get; }
    }

    public abstract class TezObjectWrapper : ITezWrapper
    {
        public abstract string name { get; }
        public abstract string description { get; }
        public abstract void clear();
    }
}