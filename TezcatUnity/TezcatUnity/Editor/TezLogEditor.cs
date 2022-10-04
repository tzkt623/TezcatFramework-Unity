using UnityEditor;
using UnityEngine;

namespace tezcat.TEditor
{
    public class TezLogEditor : EditorWindow
    {
        Vector2 m_ScrollPos = Vector2.zero;

        GUIStyle m_Info = null;
        GUIStyle m_Waring = null;
        GUIStyle m_Error = null;
        GUIStyle m_Assert = null;

        bool m_FullInfo = true;

        enum InfoType
        {
            All,
            Info,
            Waring,
            Error,
            Assert
        }
        InfoType m_Fliter = InfoType.All;

        [MenuItem("Tezcat/Debug/LogWindow")]
        public static void show()
        {
            EditorWindow.GetWindow(typeof(TezLogEditor), false, "TezDebugLog");
        }

        private void OnEnable()
        {
            m_Info = new GUIStyle();
            m_Info.normal.textColor = Color.blue;

            m_Waring = new GUIStyle();
            Color color;
            ColorUtility.TryParseHtmlString("#FF6F00FF", out color);
            m_Waring.normal.textColor = color;

            m_Error = new GUIStyle();
            m_Error.normal.textColor = Color.red;

            m_Assert = new GUIStyle();
            m_Assert.normal.textColor = Color.magenta;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            m_Fliter = (InfoType)EditorGUILayout.EnumPopup("过滤", m_Fliter);
            if(GUILayout.Button("清空"))
            {

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, new GUILayoutOption[0]);
            EditorGUILayout.EndScrollView();

            this.Repaint();
        }
    }
}