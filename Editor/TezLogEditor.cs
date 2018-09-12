﻿using tezcat.Core;
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
                TezService.get<TezDebug>().clear();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, new GUILayoutOption[0]);
            this.drawLabel();
            EditorGUILayout.EndScrollView();

            this.Repaint();
        }

        private void drawLabel()
        {
            foreach (var content in TezService.get<TezDebug>().infoQueue)
            {
                switch (m_Fliter)
                {
                    case InfoType.All:
                        this.drawAll(content);
                        break;
                    case InfoType.Info:
                        this.drawInfo(content);
                        break;
                    case InfoType.Waring:
                        this.drawWaring(content);
                        break;
                    case InfoType.Error:
                        this.drawError(content);
                        break;
                    case InfoType.Assert:
                        this.drawAssert(content);
                        break;
                    default:
                        break;
                }
            }
        }

        private void drawAll(TezDebugInfo content)
        {
            switch (content.type)
            {
                case TezDebugType.Info:
                    EditorGUILayout.LabelField(content.content, m_Info);
                    break;
                case TezDebugType.Waring:
                    EditorGUILayout.LabelField(content.content, m_Waring);
                    break;
                case TezDebugType.Error:
                    EditorGUILayout.LabelField(content.content, m_Error);
                    break;
                case TezDebugType.Assert:
                    EditorGUILayout.LabelField(content.content, m_Assert);
                    break;
            }
        }

        private void drawInfo(TezDebugInfo content)
        {
            switch (content.type)
            {
                case TezDebugType.Info:
                    EditorGUILayout.LabelField(content.content, m_Info);
                    break;
            }
        }

        private void drawWaring(TezDebugInfo content)
        {
            switch (content.type)
            {
                case TezDebugType.Waring:
                    EditorGUILayout.LabelField(content.content, m_Waring);
                    break;
            }
        }

        private void drawError(TezDebugInfo content)
        {
            switch (content.type)
            {
                case TezDebugType.Error:
                    EditorGUILayout.LabelField(content.content, m_Error);
                    break;
            }
        }

        private void drawAssert(TezDebugInfo content)
        {
            switch (content.type)
            {
                case TezDebugType.Assert:
                    EditorGUILayout.LabelField(content.content, m_Assert);
                    break;
            }
        }
    }
}