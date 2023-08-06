using DigitalHuarongRoad.Share;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_Gameover : ZoomedView
    {
        public Button B_Rechallenge;
        public Button B_Next;
        private ChessBoard chessBoard;
        public Text[] archivements;
        private string[] completeArchivementsTexts = new[] { "顺利通关", "在{0}步内通关", "在{0}秒内通关" };
        private string[] noCompleteArchivementsTexts = new[] { "通关失败", "未在{0}步内通关", "未在{0}秒内通关" };

        private void Awake()
        {
            B_Rechallenge = Helper.GetComponent<Button>(B_Rechallenge);
            B_Next = Helper.GetComponent<Button>(B_Next);
            for (int i = 0; i < archivements.Length; i++)
            {
                archivements[i] = Helper.GetComponent<Text>(archivements[i]);
            }

            B_Rechallenge.onClick.AddListener((() =>
            {
                CloseWD(() =>
                {
                    chessBoard.ReStart();
                });
            }));

            B_Next.onClick.AddListener((() =>
            {
                CloseWD(() =>
                {
                    chessBoard.NextLevel();
                });
            }));
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);

            if (args != null && args.Length > 0)
            {
                chessBoard = args[0] as ChessBoard;
            }

            if (chessBoard.level == 31)
            {
                Helper.SetActiveState(B_Next, false);
            }

            var levelInfo = LevelInfoManager.Instance.GetLevelInfo(chessBoard.level);
            Transform showStarBox = Helper.GetTransfrom(transform, "Container/StarBox/ShowStarBox");

            if (levelInfo != null)
            {
                Helper.SetActiveState(showStarBox, true);
                for (int i = 0; i < levelInfo.levelAchievementStatus.Length; i++)
                {
                    if (levelInfo.levelAchievementStatus[i] == 1)
                    {
                        Helper.SetActiveState(showStarBox.GetChild(i), true);

                       
                        switch (i)
                        {
                            case 0:
                            {
                                archivements[i].color = Color.white;
                                archivements[i].text = completeArchivementsTexts[i];
                                break;
                            }
                            case 1:
                            {
                                archivements[i].color = Color.white;
                                archivements[i].text =Helper.StringFormat(completeArchivementsTexts[i],chessBoard.LimitStep) ;
                                break;
                            }
                            case 2:
                            {
                                archivements[i].color = Color.white;
                                archivements[i].text =Helper.StringFormat(completeArchivementsTexts[i],chessBoard.LimitTime) ;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Helper.SetActiveState(showStarBox.GetChild(i), false);
                        switch (i)
                        {
                            case 0:
                            {
                                archivements[i].color = Color.red;
                                archivements[i].text = noCompleteArchivementsTexts[i];
                                break;
                            }
                            case 1:
                            {
                                archivements[i].color = Color.red;
                                archivements[i].text =Helper.StringFormat(noCompleteArchivementsTexts[i],chessBoard.LimitStep) ;
                                break;
                            }
                            case 2:
                            {
                                archivements[i].color = Color.red;
                                archivements[i].text =Helper.StringFormat(noCompleteArchivementsTexts[i],chessBoard.LimitTime) ;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Helper.SetActiveState(showStarBox, false);
            }
        }
    }
}
