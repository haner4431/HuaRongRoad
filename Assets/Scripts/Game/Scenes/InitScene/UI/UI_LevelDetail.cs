using DigitalHuarongRoad.Share;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_LevelDetail : ZoomedView
    {
        private Button B_Return;
        private Button B_Challenge;
        private Text T_Title;
        
        private int level;
        private void Awake()
        {
            B_Return = Helper.GetComponent<Button>(transform, "Container/B_Return");
            B_Challenge = Helper.GetComponent<Button>(transform, "Container/B_Challenge");
            T_Title = Helper.GetComponent<Text>(transform, "Container/Title/T_Title");
            B_Return.onClick.AddListener((() =>
            {
                CloseWD();
                if (UIManager.IsOpenedWD(InitView.UI_SelectLevelPanel) == null)
                {
                    OpenWD(InitView.UI_SelectLevelPanel);
                }
            }));

            B_Challenge.onClick.AddListener((() =>
            {
                UIManager.CloseAllWD();
                GameMain.Instance.StartGame(level);
            }));
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            if (args.Length > 0)
            {
                level = (int)args[0];
            }

            T_Title.text = Helper.StringFormat("第{0}关", level);
            var levelInfo =  LevelInfoManager.Instance.GetLevelInfo(level);
            Transform showStarBox = Helper.GetTransfrom(transform, "Container/StarBox/ShowStarBox");
            
            if (levelInfo != null)
            {
                Helper.SetActiveState(showStarBox,true);
                for (int i = 0; i < levelInfo.levelAchievementStatus.Length; i++)
                {
                    if (levelInfo.levelAchievementStatus[i] == 1)
                    {
                        Helper.SetActiveState(showStarBox.GetChild(i),true);
                    }
                    else
                    {
                        Helper.SetActiveState(showStarBox.GetChild(i),false);
                    }
                
                }
            }
            else
            {
                Helper.SetActiveState(showStarBox,false);
            }

        }
    }
}
