using tezcat.Framework.Database;
using tezcat.Framework.Utility;
using tezcat.Unity;
using tezcat.Unity.Utility;
using UnityEngine;

namespace tezcat.Framework.Test
{
    public class TestKeyConfig
    {
        TezKeyConfig m_Config;
        TezKeyConfig m_SaveConfig;
        TezKeyConfig m_LoadConfig;
        TezKeyConfig m_ChangeKeyConfig;

        public void init()
        {
            var logic = TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("Logic");
            logic.addConfig(new UnityAdvanceKeyConfig2(
                "Test",
                new UnityKeyPressWrapper()
                {
                    keyCode = KeyCode.LeftControl
                },
                new UnityKeyPressWrapper()
                {
                    keyCode = KeyCode.G
                }));

            var sl = TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("SL");
            sl.addConfig(new UnityBaseKeyConfig1(
                "Save",
                new UnityKeyPressWrapper()
                {
                    keyCode = KeyCode.S
                }));

            sl.addConfig(new UnityBaseKeyConfig1(
                "Load",
                new UnityKeyPressWrapper()
                {
                    keyCode = KeyCode.L
                }));

            var change_key = TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("Option");
            change_key.addConfig(new UnityBaseKeyConfig1(
                "ChangeKey",
                new UnityKeyPressWrapper()
                {
                    keyCode = KeyCode.F1
                }));

        }

        public void getConfigs()
        {
            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("Logic").tryGetConfig("Test", out TezKeyConfig keyConfig);
            m_Config = keyConfig;

            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("SL").tryGetConfig("Save", out TezKeyConfig sc);
            m_SaveConfig = sc;

            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("SL").tryGetConfig("Load", out TezKeyConfig lc);
            m_LoadConfig = lc;

            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("Option").tryGetConfig("ChangeKey", out TezKeyConfig ck);
            m_ChangeKeyConfig = ck;
        }

        public void changeKey()
        {
            TezcatUnity.unityKeyConfigSystem.setChangeKey("Logic", "Test", 0);
///OR           UnityKeyConfigManager.instance.setChangeKey(m_Config.getWrapper(0));
        }

        public void update()
        {
            if (m_Config.active())
            {
                ///Do someting......
            }

            if (m_SaveConfig.active())
            {
                TezWriter writer = new TezJsonWriter();
                TezcatUnity.unityKeyConfigSystem.writeToSave(writer);
            }

            if (m_LoadConfig.active())
            {
                TezReader reader = new TezJsonReader();
                reader.load("DataPath");
                TezcatUnity.unityKeyConfigSystem.readFromSave(reader);
            }

            if(m_ChangeKeyConfig.active())
            {
                this.changeKey();
            }

            if(TezcatUnity.unityKeyConfigSystem.isWaitingChangeKey)
            {
                TezcatUnity.unityKeyConfigSystem.waitingChangeKey();
            }
        }
    }
}