﻿using tezcat.DataBase;

namespace tezcat.Wrapper
{
    public interface ITezWrapper
    {
        void clear();
    }

    public interface ITezObjectWrapper : ITezWrapper
    {
        string name { get; }
        string description { get; }
    }

    public interface ITezItemWrapper : ITezWrapper
    {
        TezItem item { get; }
        int count { get; }

        void showTip();
        void hideTip();
        void onDrop();
    }

    public abstract class TezObjectWrapper : ITezObjectWrapper
    {
        public abstract string name { get; }
        public abstract string description { get; }
        public abstract void clear();
    }
}