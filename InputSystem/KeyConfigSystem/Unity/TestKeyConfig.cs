using tezcat.Framework.Database;
using UnityEngine;

namespace tezcat.Framework.InputSystem
{
    public class TestKeyConfig
    {
        TezKeyConfig m_Config;
        TezKeyConfig m_SaveConfig;
        TezKeyConfig m_LoadConfig;

        public void init()
        {
            var logic = UnityKeyConfigSystem.instance.getOrCreateConfigLayer("Logic");
            logic.addConfig(new UnityAdvanceKeyConfig2(
                "Test",
                new UnityKeyPressWrapper()
                {
                    keyCode = KeyCode.LeftControl
                },
                new UnityKeyDownWrapper()
                {
                    keyCode = KeyCode.G
                }));

            var sl = UnityKeyConfigSystem.instance.getOrCreateConfigLayer("SL");
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
        }

        public void getConfigs()
        {
            UnityKeyConfigSystem.instance.getOrCreateConfigLayer("Logic").tryGetConfig("Test", out TezKeyConfig keyConfig);
            m_Config = keyConfig;

            UnityKeyConfigSystem.instance.getOrCreateConfigLayer("SL").tryGetConfig("Save", out TezKeyConfig sc);
            m_SaveConfig = sc;

            UnityKeyConfigSystem.instance.getOrCreateConfigLayer("SL").tryGetConfig("Save", out TezKeyConfig lc);
            m_LoadConfig = lc;
        }

        public void changeKey()
        {
            UnityKeyConfigSystem.instance.setChangeKey("Logic", "Test", 0);
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
                TezJsonWriter writer = new TezJsonWriter();
                UnityKeyConfigSystem.instance.writeToSave(writer);
            }

            if (m_LoadConfig.active())
            {
                TezReader reader = new TezJsonReader();
                reader.load("DataPath");
                UnityKeyConfigSystem.instance.readFromSave(reader);
            }

            if(UnityKeyConfigSystem.instance.isWaitingKey)
            {
                UnityKeyConfigSystem.instance.waitingKey();
            }
        }
    }
}