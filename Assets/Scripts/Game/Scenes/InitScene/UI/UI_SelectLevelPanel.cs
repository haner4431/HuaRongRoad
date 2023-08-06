using System.Collections.Generic;
using DigitalHuarongRoad.Share;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_SelectLevelPanel : View
    {
        private Transform LevelView;
        public GameObject levelItem;
        private Button B_Close;
        private List<GameObject> levelItemList;

        private void Awake()
        {
            LevelView = Helper.GetTransfrom(transform, "Container/LevelView");
            levelItemList = new List<GameObject>(40);
            B_Close = Helper.GetComponent<Button>(transform, "B_Close");
            B_Close.onClick.AddListener(() =>
            {
                CloseWD();
                if (UIManager.IsOpenedWD(InitView.UI_InitMain) == null)
                {
                    OpenWD(InitView.UI_InitMain);
                }
            });
            for (int i = 1; i < 32; i++)
            {
                var go = Instantiate(levelItem,Helper.GetTransfrom(LevelView,"Viewport/Content"));
                go.name = i.ToString();
                var level = Helper.GetComponent<Text>(go.transform, "T_Level");
                level.text = i.ToString();
                levelItemList.Add(go);
                var tmepBtn = Helper.GetComponent<Button>(go.transform);
                tmepBtn.onClick.AddListener((() =>
                {
                    int level = int.Parse(go.name);
                    OpenWD(InitView.UI_LevelDetail, level);
                    //CloseWD();
                }));
            }
            Helper.SetActiveState(levelItem,false);
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            ReFreshView();
        }

        private void ReFreshView()
        {
            if(levelItemList == null || levelItemList.Count == 0 ) return;
            for (int i = 0; i < levelItemList.Count; i++)
            {
                SetViewData(levelItemList[i].transform, i);
            }
        }

        private void SetViewData(Transform data ,int index)
        {
            LevelInfo info = LevelInfoManager.Instance.GetLevelInfo(index+1);
            Transform showStarBox = Helper.GetTransfrom(data, "StarBox/ShowStarBox");
            if (info != null)
            {
                Helper.SetActiveState(showStarBox,true);
                for (int i = 0; i < info.levelAchievementStatus.Length; i++)
                {
                    if (info.levelAchievementStatus[i] == 1)
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
