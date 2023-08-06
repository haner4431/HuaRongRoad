using System;
using System.Collections.Generic;
using System.Linq;
using LFrame.Core.Tools;
using LitJson;

namespace DigitalHuarongRoad.Share
{
    public class LevelInfo
    {
        public int levelNum; //关卡编号
        public int[] levelAchievementStatus; // 0表示未完成成就，1表示完成。成就信息：1，通关 2.规定步数内完成 3.规定时间内完成
    }
    
    public  class LevelInfoManager
    {
        LevelInfoManager()
        {
            LevelInfoDict = new Dictionary<int, LevelInfo>();
            loadAllLevelInfo();
        }
        
        private static readonly LevelInfoManager instance = new LevelInfoManager();

        public static  LevelInfoManager Instance
        {
            get
            {
                return instance;
            }
        }
        
        private  string levelInfoPath
        {
            get
            {
                return Helper.StringFormat("{0}LeveInfo/levelInfo", Constants.Docs_Path);
            }
        }

        private Dictionary<int, LevelInfo> LevelInfoDict;

        public void SaveLevelInfo(int levelNum,int[] levelAchievementStatus)
        {
            if (LevelInfoDict.ContainsKey(levelNum) == false)
            {
                LevelInfoDict.Add(levelNum,new LevelInfo());
            }

            LevelInfoDict[levelNum].levelNum = levelNum;
            LevelInfoDict[levelNum].levelAchievementStatus = levelAchievementStatus;
            string jsonText =  JsonMapper.ToJson(LevelInfoDict.Values.ToArray());
            Helper.SaveText(levelInfoPath,jsonText);
        }
        
        

        public LevelInfo GetLevelInfo(int level)
        {
            if (LevelInfoDict.ContainsKey(level))
            {
                return LevelInfoDict[level];
            }
            return null;
        }

        private void loadAllLevelInfo()
        {
            string jsons = Helper.OpenText(levelInfoPath);
            if(String.IsNullOrEmpty(jsons))return;
            LevelInfo[] levelInfoArray = JsonMapper.ToObject<LevelInfo[]>(jsons);
            LevelInfoDict = levelInfoArray.ToDictionary(info => info.levelNum,info => info );
        }
    }
}
