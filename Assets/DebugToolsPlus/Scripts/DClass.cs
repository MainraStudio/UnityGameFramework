using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DebugToolsPlus
{
    public static class D
    {
        /// <summary>
        /// Crea el formato del titulo.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        static string FormatTitle(string title, int color)
        {
            return $"<color=\"{DColors.GetColorHex(color)}\"><b>[{title}]</b></color>";
        }

        /// <summary>
        /// Formatea un texto con colores.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string FormatText(string text, int color)
        {
            return $"<color=\"{DColors.GetColorHex(color)}\"><b>{text}</b></color>";
        }

        /// <summary>
        /// Formatea un texto con colores.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string FormatText(string text, DColor color)
        {
            return FormatText(text, (int)color);
        }

        #region [LOG]

        /// <summary>
        /// Manda un mensaje por consola con un formato indicado.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="logType"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, Object context, LogType logType, int color, bool colorMessage)
        {
            if (isRecording)
            {
                dump.Add($"[{title}] {message}");
            }

            title = FormatTitle(title, color);
            if(colorMessage) message = FormatText(message, color);
            string log = $"{title} {message}";

            Debug.LogFormat(logType, LogOption.None, context, log);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void Log(string title, string message)
        {
            Log(title, message, null, LogType.Log, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un Object de Contexto.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void Log(string title, string message, Object context)
        {
            Log(title, message, context, LogType.Log, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un Object de Contexto e indicando un LogType.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="logType"></param>
        public static void Log(string title, string message, Object context, LogType logType)
        {
            Log(title, message, context, logType, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un Object de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, Object context, int color)
        {
            Log(title, message, context, LogType.Log, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un Object de Contexto y un color eligiendo si mostrar el color en todo el mensaje.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, Object context, int color, bool colorMessage)
        {
            Log(title, message, context, LogType.Log, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un Object de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, Object context, DColor color)
        {
            Log(title, message, context, LogType.Log, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un Object de Contexto y un color eligiendo si mostrar el color en todo el mensaje.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, Object context, DColor color, bool colorMessage)
        {
            Log(title, message, context, LogType.Log, (int)color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, int color)
        {
            Log(title, message, null, LogType.Log, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, int color, bool colorMessage)
        {
            Log(title, message, null, LogType.Log, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, DColor color)
        {
            Log(title, message, null, LogType.Log, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, DColor color, bool colorMessage)
        {
            Log(title, message, null, LogType.Log, (int)color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un tipo de Log.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        public static void Log(string title, string message, LogType logType)
        {
            Log(title, message, null, logType, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un tipo de Log y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, LogType logType, int color)
        {
            Log(title, message, null, logType, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un tipo de Log y un color eligiendo si mostrar el color en todo el mensaje.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, LogType logType, int color, bool colorMessage)
        {
            Log(title, message, null, logType, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un tipo de Log y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, LogType logType, DColor color)
        {
            Log(title, message, null, logType, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Log por la consola con un tipo de Log y un color eligiendo si mostrar el color en todo el mensaje.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        /// <param name="color"></param>
        public static void Log(string title, string message, LogType logType, DColor color, bool colorMessage)
        {
            Log(title, message, null, logType, (int)color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void LogWarning(string title, string message)
        {
            Log(title, message, null, LogType.Warning, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un Objeto de Contexto.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogWarning(string title, string message, Object context)
        {
            Log(title, message, context, LogType.Warning, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un Objeto de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogWarning(string title, string message, Object context, int color)
        {
            Log(title, message, context, LogType.Warning, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un Objeto de Contexto y un color eligiendo si mostrar el color en todo el mensaje.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogWarning(string title, string message, Object context, int color, bool colorMessage)
        {
            Log(title, message, context, LogType.Warning, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un Objeto de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogWarning(string title, string message, Object context, DColor color)
        {
            Log(title, message, context, LogType.Warning, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un Objeto de Contexto y un color eligiendo si mostrar el color en todo el mensaje.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogWarning(string title, string message, Object context, DColor color, bool colorMessage)
        {
            Log(title, message, context, LogType.Warning, (int)color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogWarning(string title, string message, int color)
        {
            Log(title, message, null, LogType.Warning, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogWarning(string title, string message, int color, bool colorMessage)
        {
            Log(title, message, null, LogType.Warning, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogWarning(string title, string message, DColor color)
        {
            Log(title, message, null, LogType.Warning, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogWarning(string title, string message, DColor color, bool colorMessage)
        {
            Log(title, message, null, LogType.Warning, (int)color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void LogError(string title, string message)
        {
            Log(title, message, null, LogType.Error, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un Objeto de Contexto.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogError(string title, string message, Object context)
        {
            Log(title, message, context, LogType.Error, 0, false);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un Objeto de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogError(string title, string message, Object context, int color)
        {
            Log(title, message, context, LogType.Error, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un Objeto de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogError(string title, string message, Object context, int color, bool colorMessage)
        {
            Log(title, message, context, LogType.Error, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un Objeto de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogError(string title, string message, Object context, DColor color)
        {
            Log(title, message, context, LogType.Error, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un Objeto de Contexto y un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public static void LogError(string title, string message, Object context, DColor color, bool colorMessage)
        {
            Log(title, message, context, LogType.Error, (int)color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogError(string title, string message, int color)
        {
            Log(title, message, null, LogType.Error, color, false);
        }

        /// <summary>
        /// Manda un mensaje de Warning por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogError(string title, string message, int color, bool colorMessage)
        {
            Log(title, message, null, LogType.Error, color, colorMessage);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogError(string title, string message, DColor color)
        {
            Log(title, message, null, LogType.Error, (int)color, false);
        }

        /// <summary>
        /// Manda un mensaje de Error por la consola con un color.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void LogError(string title, string message, DColor color, bool colorMessage)
        {
            Log(title, message, null, LogType.Error, (int)color, colorMessage);
        }

        #endregion

        #region [DUMP]

        /// <summary>
        /// Almacena todos los logs para luego mandarlos a un archivo.
        /// </summary>
        static List<string> dump = new List<string>();

        /// <summary>
        /// Indica si esta grabando los logs o no.
        /// </summary>
        static bool isRecording = false;

        public const string DUMP_PATH = "Assets/DebugToolsPlus/Dumps";
        public const string DUMP_NAME = "Dump";

        /// <summary>
        /// Gets the path to the dump file.
        /// </summary>
        /// <returns></returns>
        static string GetDumpPath()
        {
            System.DateTime now = System.DateTime.Now;
            return $"{DUMP_PATH}/{DUMP_NAME}{now.Day}_{now.Hour}_{now.Minute}_{now.Second}.txt";
        }

        /// <summary>
        /// Starts a recording of the logs.
        /// </summary>
        public static void StartRecording()
        {
            isRecording = true;
        }

        /// <summary>
        /// Pause a recording of the logs.
        /// </summary>
        public static void PauseRecording()
        {
            isRecording = false;
        }

        /// <summary>
        /// Stops the recording of the logs.
        /// </summary>
        public static void StopRecording()
        {
            isRecording = false;
            SaveDump();
            dump.Clear();
        }

        /// <summary>
        /// Save the dump to a file.
        /// </summary>
        static void SaveDump()
        {
            if (dump.Count == 0) return;

#if UNITY_EDITOR

            if (!File.Exists(DUMP_PATH))
            {
                Directory.CreateDirectory(DUMP_PATH);
            }

            string path = GetDumpPath();
            System.IO.File.WriteAllLines(path, dump);

            Log("DUMP", $"Dump saved to: {GetDumpPath()}");
            AssetDatabase.Refresh();

#endif

            dump.Clear();
        }

        #endregion 
    }
}
