using System;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_InitMain : View
    {
        private Button B_SelectLevel;
        private Button B_Setting;
        private Button B_Exit;

        private void Awake()
        {
            B_SelectLevel = Helper.GetComponent<Button>(gameObject, "Box/B_SelectLevel");
            B_Setting = Helper.GetComponent<Button>(gameObject, "Box/B_Setting");
            B_Exit = Helper.GetComponent<Button>(gameObject, "Box/B_Exit");

            B_SelectLevel.onClick.AddListener(() =>
            {
                OpenWD(InitView.UI_SelectLevelPanel);
                //CloseWD();
            });
            B_Setting.onClick.AddListener(() =>
            {
                OpenWD(ShareView.UI_Setting);
            });
            B_Exit.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
	            Application.Quit();
#endif
            });
        }
    }
}
