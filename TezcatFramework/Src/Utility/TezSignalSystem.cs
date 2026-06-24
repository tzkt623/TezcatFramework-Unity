using System;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezSignalSystem
    {
        public interface ISignal
        {
            ISignal add(string phaseName);
        }

        class Signal : ISignal
        {
            public Action onComplete;
            public HashSet<string> signals;

            public ISignal add(string phaseName)
            {
                this.signals.Add(phaseName);
                return this;
            }
        }

        Dictionary<string, Signal> mDict = new Dictionary<string, Signal>();

        public void registerSignal(string signalName, Action onComplete, ICollection<string> phaseNames)
        {
            if (!mDict.TryGetValue(signalName, out Signal signal))
            {
                signal = new Signal
                {
                    signals = new HashSet<string>(phaseNames),
                    onComplete = onComplete
                };
                mDict.Add(signalName, signal);
            }

            signal.onComplete = onComplete;
        }

        public ISignal registerSignal(string signalName, Action onComplete)
        {
            if (!mDict.TryGetValue(signalName, out Signal signal))
            {
                signal = new Signal 
                { 
                    signals = new HashSet<string>(),
                    onComplete = onComplete
                };
                mDict.Add(signalName, signal);
            }

            return signal;
        }

        public void addSignalPhase(string signalName, string phaseName)
        {
            if (mDict.TryGetValue(signalName, out Signal signal))
            {
                signal.signals.Add(phaseName);
            }
        }

        public void emitSignal(string signalName, string phaseName)
        {
            if (mDict.TryGetValue(signalName, out Signal signal))
            {
                if(signal.signals.Remove(phaseName))
                {
                    if(signal.signals.Count == 0)
                    {
                        signal.onComplete.Invoke();
                        signal.signals = null;
                        signal.onComplete = null;
                        mDict.Remove(signalName);
                    }
                }
            }
            else
            {
                throw new Exception($"Signal '{signalName}' not registered.");
            }
        }

        public void clear()
        {
            mDict.Clear();
        }
    }
}
