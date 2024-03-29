﻿using UnityEngine;

namespace tezcat.Unity.UI
{
    public static class TezLayout
    {
        /*
         * <pivot>
         * 
         * RectTransform实际所使用的中心点
         * 
         * 在UI中用0-1的规范值来确定相对位置
         * 
         * 实际位置需要根据rect的参数一起计算得出
         * 
         */

        /*
         * <anchoredPosition>
         * 
         * 此值跟pivot与anchorMin和anchorMax有关
         * 
         * 根据当前RectTransform的pivot的规范值在anchorMin和anchorMax的范围内生成一个anchor的引用点P
         * 此引用点P在anchorMin和anchorMax范围内的规范值与pivot的值是一致的
         * 
         * 然后以此引用点P为坐标系原点建立坐标系A
         * 计算得到的当前RectTransform的pivot在坐标系A上的位置
         * 
         * 如果anchorMin == anchorMax
         * 既anchor是一个点,没有范围
         * 则引用点P与anchor重合
         * 则anchor就是坐标系A的原点
         * 
         */

        /*
         * <sizeDelta>
         * 
         * 此值跟当前RectTransform的anchorMin,anchorMax,rect的size 与 parent的RectTransform的rect的size有关
         * 
         * offset = anchorMax - anchorMin
         * sizeDelta = 当前RectTransform的rect的size - parent的RectTransform的rect的size * offset 
         * 
         */

        /*
         * <localposition>
         * 
         * 此节点的pivot到父节点的pivot的相对坐标值
         */



        public static void setLayout(this RectTransform transform, float left, float bottom, float right, float top)
        {
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.pivot = new Vector2(0.5f, 0.5f);

            transform.offsetMin = new Vector2(left, bottom);
            transform.offsetMax = new Vector2(right, top);
        }

        public static void setLayoutZeroRect(this RectTransform transform)
        {
            transform.anchorMin = new Vector2(0.5f, 0.5f);
            transform.anchorMax = new Vector2(0.5f, 0.5f);
            transform.pivot = new Vector2(0.5f, 0.5f);

            transform.offsetMin = Vector2.zero;
            transform.offsetMax = Vector2.zero;
        }

        public static void setAsTopWindow(this TezWindow window)
        {
            if(window.transform.GetSiblingIndex() != window.transform.parent.childCount)
            {
                window.transform.SetAsLastSibling();
            }
        }
    }
}

