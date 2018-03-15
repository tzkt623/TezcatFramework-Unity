using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat
{
    public class TezUILayout
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
    }
}

