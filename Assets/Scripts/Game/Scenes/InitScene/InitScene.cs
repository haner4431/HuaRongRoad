using LFrame.Core.Audio;
using LFrame.Core.Scene;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;

namespace DigitalHuarongRoad
{
    public class InitScene: Scene
    {

        public static readonly InitScene Instance = new InitScene();

        private bool navFlag;
        private bool openSelectLevel;
        InitScene()
        {
            this.name = "Init";
        }

        public override void Start(params object[] param)
        {
            base.Start(param);
            if (param !=null  && param.Length > 0)
            {
                 openSelectLevel = (bool)param[0] ;
              
            }
            else
            {
                openSelectLevel = false;
            }
        }

        public override void Onload()
        {
            base.Onload();
            if (navFlag == false)
            {
                navFlag = true;
                GameMain.ShowLoadingProgress(() =>
                {
                    return Navigation.Instance.QueryTableSize / (float)181440;
                },(() =>
                {
                    UIManager.OpenWD(InitView.UI_InitMain);
                }));
            }
            else
            {
                UIManager.OpenWD(InitView.UI_InitMain);
            }
            AudioManager.Instance.PlayMusic(new [] { "BGM_05", "BGM_06" , "BGM_07", "BGM_08" });
            if (openSelectLevel)
            {
                UIManager.OpenWD(InitView.UI_SelectLevelPanel);
            }
        }

        public override void Stop()
        {
            base.Stop();
            AudioManager.Instance.StopAll();
        }
    }
}
