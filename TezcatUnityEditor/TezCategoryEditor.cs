using tezcat.Unity.Tool;
using UnityEditor;
using UnityEngine;

namespace tezcat.Framework.MyEditor
{
    [CustomEditor(typeof(TezCategoryGenerator))]
    public class TezCategoryEditor : Editor
    {
        TezCategoryGenerator mGenerator;

        void OnEnable()
        {
            mGenerator = this.target as TezCategoryGenerator;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("CategoryFile"))
            {
                mGenerator.inCategoryFilePath = EditorUtility.OpenFilePanel("CategoryFile", "", "json");
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("CShapeFile SavePath"))
            {
                mGenerator.outCShapPath = EditorUtility.OpenFilePanel("CShapFile", "", "cs");
            }

            if (GUILayout.Button("ItemConfigFile SavePath"))
            {
                mGenerator.outItemConfigPath = EditorUtility.OpenFilePanel("ConfigFile", "", "json");
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Generator CShapeFile"))
            {
                mGenerator.generateCShapFile();
            }

            if (GUILayout.Button("Generator ItemConfigFile"))
            {
                mGenerator.generateItemConfigFile();
            }
        }
    }
}