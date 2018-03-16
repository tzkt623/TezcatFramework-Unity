using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace tezcat
{
    [CustomEditor(typeof(TezPoolMB), true)]
    [CanEditMultipleObjects]
    public class TezPoolMBEditor : Editor
    {
        #region Property
        SerializedProperty m_Prefab = null;
        SerializedProperty m_InitCount = null;
        SerializedProperty m_Dynamic = null;
        #endregion

        #region Label
        GUIContent m_Label_InitCount = new GUIContent("初始化数量");
        GUIContent m_Label_Prefab = new GUIContent("预制件");
        GUIContent m_Label_Dynamic = new GUIContent("动态指定");
        #endregion


        private void OnEnable()
        {
            m_InitCount = base.serializedObject.FindProperty("m_InitCount");
            m_Prefab = base.serializedObject.FindProperty("m_Prefab");
            m_Dynamic = base.serializedObject.FindProperty("m_Dynamic");
        }

        public override void OnInspectorGUI()
        {
//             base.serializedObject.Update();
//             EditorGUILayout.PropertyField(m_Dynamic, m_Label_Dynamic, null);
//             if(!m_Dynamic.boolValue)
//             {
//                 EditorGUI.indentLevel++;
//                 EditorGUILayout.PropertyField(m_Prefab, m_Label_Prefab, null);
//                 EditorGUILayout.PropertyField(m_InitCount, m_Label_InitCount, null);
//                 EditorGUI.indentLevel--;
//             }
//             base.serializedObject.ApplyModifiedProperties();
        }

    }
}