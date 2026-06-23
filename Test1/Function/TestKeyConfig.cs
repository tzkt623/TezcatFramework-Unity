using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Unity;
using tezcat.Unity.Utility;
using UnityEngine;

namespace tezcat.Framework.Test
{
    public class TestKeyConfig : TezBaseTest
    {
        TezKeyConfig mConfig;
        TezKeyConfig mSaveConfig;
        TezKeyConfig mLoadConfig;
        TezKeyConfig mChangeKeyConfig;

        public TestKeyConfig() : base("KeyConfig")
        {

        }

        public override void init()
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
            mConfig = keyConfig;

            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("SL").tryGetConfig("Save", out TezKeyConfig sc);
            mSaveConfig = sc;

            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("SL").tryGetConfig("Load", out TezKeyConfig lc);
            mLoadConfig = lc;

            TezcatUnity.unityKeyConfigSystem.getOrCreateConfigLayer("Option").tryGetConfig("ChangeKey", out TezKeyConfig ck);
            mChangeKeyConfig = ck;
        }

        public void changeKey()
        {
            TezcatUnity.unityKeyConfigSystem.setChangeKey("Logic", "Test", 0);
///OR           UnityKeyConfigManager.instance.setChangeKey(m_Config.getWrapper(0));
        }

        public override void run()
        {
            if (mConfig.active())
            {
                ///Do someting......
            }

            if (mSaveConfig.active())
            {
                TezWriter writer = new TezJsonWriter();
                TezcatUnity.unityKeyConfigSystem.writeToSave(writer);
            }

            if (mLoadConfig.active())
            {
                TezFileReader reader = new TezJsonReader();
                reader.load("DataPath");
                TezcatUnity.unityKeyConfigSystem.readFromSave(reader);
            }

            if(mChangeKeyConfig.active())
            {
                this.changeKey();
            }

            if(TezcatUnity.unityKeyConfigSystem.isWaitingChangeKey)
            {
                TezcatUnity.unityKeyConfigSystem.waitingChangeKey();
            }
        }

        protected override void onClose()
        {
            TezcatUnity.unityKeyConfigSystem.close();
        }
    }
}