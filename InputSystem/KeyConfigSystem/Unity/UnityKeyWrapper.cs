using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using UnityEngine;

namespace tezcat.Framework.InputSystem
{
    public abstract class UnityKeyWrapper : TezKeyWrapper
    {
        static readonly Array s_Keys = Enum.GetValues(typeof(KeyCode));

        public enum State
        {
            Press,
            Down,
            Up
        }

        public override string name => keyCode.ToString();
        public abstract State state { get; }

        public KeyCode keyCode;

        public override void writeToSave(TezWriter writer)
        {
            writer.write("State", (int)this.state);
            writer.write("KeyCode", keyCode.ToString());
        }

        public override void readFromSave(TezReader reader)
        {
            if (Enum.TryParse(reader.readString("KeyCode"), out KeyCode result))
            {
                keyCode = result;
            }
            else
            {
                throw new Exception();
            }
        }

        protected override bool onChangeKeyDown()
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode item in s_Keys)
                {
                    if (Input.GetKeyDown(item))
                    {
                        keyCode = item;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}