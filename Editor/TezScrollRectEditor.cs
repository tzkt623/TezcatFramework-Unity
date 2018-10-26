using UnityEngine;
using UnityEditor.UI;
using UnityEditor;
using tezcat.Framework.UI;

namespace tezcat
{
    [CustomEditor(typeof(TezScrollRect), true)]
    [CanEditMultipleObjects]
    public class TezScrollRectEditor : ScrollRectEditor
    {
        TezScrollRect m_Target = null;

        #region Property
        private SerializedProperty m_CellControl = null;
        private SerializedProperty m_PrefabCell = null;
        #endregion

        #region Label
        private GUIContent m_Label_CellControl = new GUIContent("Cell控制", "Cell的Size控制方式");
        private GUIContent m_Label_PrefabCell = new GUIContent("Cell预制件");
        #endregion

        protected override void OnEnable()
        {
            m_Target = target as TezScrollRect;
            m_CellControl = base.serializedObject.FindProperty("m_CellControl");
            m_PrefabCell = base.serializedObject.FindProperty("m_PrefabCell");
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_Target = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            base.serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_PrefabCell, m_Label_PrefabCell, null);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}