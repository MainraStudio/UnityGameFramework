using System;
using Unity.VisualScripting;
using UnityEngine;

namespace DebugToolsPlus
{
    public static class DColors
    {
        static Color[] colors = new Color[]
        {
            Color.white,
            Color.black,
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            new Color(0.596f, 0.317f, 0.812f),
            new Color(0.162f, 1f, 0.764f),
            new Color(1f, 0.62f, 0.86f),
        };

        public static int ColorLength {  get { return colors.Length; } }

        /// <summary>
        /// Obtiene un color.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Color GetColor(int index)
        {
            index = Mathf.Clamp(index, 0, colors.Length);
            return colors[index];
        }

        /// <summary>
        /// Obtiene un color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetColor(DColor color)
        {
            return GetColor((int)color);
        }

        /// <summary>
        /// Obtiene el valor Hexadecimal de un color.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetColorHex(int index)
        {
            return $"#{GetColor(index).ToHexString()}";
        }

        /// <summary>
        /// Obtiene el valor Hexadecimal de un color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string GetColorHex(DColor color)
        {
            return $"#{GetColor(color).ToHexString()}";
        }

        /// <summary>
        /// Obtiene el estilo para el texto en base al color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle GetStyle(int color)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = GetColor(color);
            return style;
        }

        /// <summary>
        /// Obtiene el estilo para el texto en base al color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle GetStyle(DColor color)
        {
            return GetStyle((int)color);
        }
    }
}
