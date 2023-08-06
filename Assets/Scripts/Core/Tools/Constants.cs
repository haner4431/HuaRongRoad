using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LFrame.Core.Tools
{
    public static class Constants
    {
        public  const  string Prefab_PATH = "Prefabs/";
        public  const  string AUDIO_PATH = "Audio/";

        public static string Docs_Path
        {
            get
            {
#if UNITY_EDITOR
                string path = Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")) + "/Docs/";
#else
	             string path = Application.dataPath + "/Docs/";
#endif


                if (Directory.Exists(path) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(path);

                    }
                    catch (Exception e) 
                    {
                        Helper.LogError(e);
                    }
                 
                }
                return path;       
            }
        }
    }
}
