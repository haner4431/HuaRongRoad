using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LFrame.Core.Tools
{
    public enum LogType
    {
        Log,
        Warning,
        Error
    }

    public class Helper
    {
        #region 日志打印封装

        private static StringBuilder stringBuilder = new StringBuilder();

        public static string StringFormat(string format, params object[] args)
        {
            try
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Remove(0, stringBuilder.Length);
                }

                stringBuilder.AppendFormat(format, args);

                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("format err" + e.Message);
                return string.Empty;
            }
        }

        public static void Log(object format, params object[] args)
        {
            LogHandle(format, LogType.Log, args);
        }

        public static void LogWarning(object format, params object[] args)
        {
            LogHandle(format, LogType.Warning, args);
        }

        public static void LogError(object format, params object[] args)
        {
            LogHandle(format, LogType.Error, args);
        }

        private static void LogHandle(object format, LogType type, params object[] args)
        {
            string log = null;
            if (format != null)
            {
                if (format is string && args != null && args.Length != 0)
                {
                    log = StringFormat(format as string, args);
                }
                else
                {
                    log = format.ToString();
                }
            }

            switch (type)
            {
                case LogType.Log:
                {
                    Debug.Log(log);
                    break;
                }
                case LogType.Warning:
                {
                    Debug.LogWarning(log);
                    break;
                }
                case LogType.Error:
                {
                    Debug.LogError(log);
                    break;
                }
            }
        }

        #endregion

        public static void SetActiveState(object root, bool state, string path = "")
        {
            Transform obj = null;
            if (root == null)
            {
                LogError("[Helper.SetActiveState] obj is null !");
                return;
            }
            else if (root is GameObject)
            {
                obj = GetTransfrom((root as GameObject).transform, path);
            }
            else if (root is Component)
            {
                obj = GetTransfrom((root as Component).transform, path);
            }
            else
            {
                LogError("[Helper.SetActiveState] obj type error!");
                return;
            }

            obj.gameObject.SetActive(state);
        }

        public static T GetComponent<T>(Object root, string path = "") where T : Component
        {
            if (root == null) return null;

            Transform target = null;
            if (root is Transform)
            {
                target = root as Transform;
            }
            else if (root is GameObject)
            {
                target = (root as GameObject).transform;
            }
            else if (root is Component)
            {
                target = (root as Component).transform;
            }

            if (target == null)
            {
                LogError("root is invalid obj");
                return null;
            }
            
            if (String.IsNullOrEmpty(path))
            {
                return target.GetComponent<T>();
            }
            else
            {
                target = target.transform.Find(path);
            }

            if (target == null)
            {
                LogError("not find obj by path: {0}", path);
                return null;
            }

            return target.GetComponent<T>();
        }

        public static T GetComponentInChildren<T>(Component root, string path = "") where T : Component
        {
            Transform target;
            if (String.IsNullOrEmpty(path))
            {
                return root.GetComponentInChildren<T>();
            }
            else
            {
                target = root.transform.Find(path);
            }

            return target.GetComponentInChildren<T>();
        }

        public static T GetComponentInParent<T>(Component root, string path = "") where T : Component
        {
            Transform target;
            if (String.IsNullOrEmpty(path))
            {
                return root.GetComponentInParent<T>();
            }
            else
            {
                target = root.transform.Find(path);
            }

            return target.GetComponentInParent<T>();
        }

        public static T[] GetComponents<T>(Component root, string path = "") where T : Component
        {
            Transform target;
            if (String.IsNullOrEmpty(path))
            {
                return root.GetComponents<T>();
            }
            else
            {
                target = root.transform.Find(path);
            }

            return target.GetComponents<T>();
        }

        public static T[] GetComponentsInChildren<T>(Component root, string path = "") where T : Component
        {
            Transform target;
            if (String.IsNullOrEmpty(path))
            {
                return root.GetComponentsInChildren<T>();
            }
            else
            {
                target = root.transform.Find(path);
            }

            return target.GetComponentsInChildren<T>();
        }

        public static T[] GetComponentsInParent<T>(Component root, string path = "") where T : Component
        {
            Transform target;
            if (String.IsNullOrEmpty(path))
            {
                return root.GetComponentsInParent<T>();
            }
            else
            {
                target = root.transform.Find(path);
            }

            return target.GetComponentsInParent<T>();
        }


        public static Transform GetTransfrom(Component root, string path = "")
        {
            return GetComponent<Transform>(root, path);
        }


        /// <summary>
        /// 保存文件（文本）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool SaveText(string path, string content)
        {
            return SaveFile(path, Encoding.UTF8.GetBytes(content));
        }


        /// <summary>
        /// 保存文件（字节）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool SaveFile(string path, byte[] buffer)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || buffer == null)
                {
                    return false;
                }

                string directory = path.Substring(0, path.IndexOf(Path.GetFileName(path)));
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var file = File.Open(path, FileMode.CreateNew))
                {
                    if (file != null)
                    {
                        file.Write(buffer, 0, buffer.Length);
                        file.Close();
                        file.Dispose();
                        return true;
                    }
                }
            }
            catch (Exception e) { Debug.LogError(e.Message); }

            return false;
        }

        /// <summary>
        /// 打开文件（文本）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string OpenText(string path) { return Encoding.UTF8.GetString(OpenFile(path)); }

        /// <summary>
        /// 打开文件（字节）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] OpenFile(string path)
        {
            byte[] bytes = new byte[0];
            try
            {
                if (File.Exists(path) == false)
                {
                    return bytes;
                }

                using (var file = File.OpenRead(path))
                {
                    if (file != null)
                    {
                        bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        file.Close();
                        file.Dispose();
                        return bytes;
                    }
                }
            }
            catch (Exception e) { Debug.LogError(e.Message); }

            return bytes;
        }
    }
}
