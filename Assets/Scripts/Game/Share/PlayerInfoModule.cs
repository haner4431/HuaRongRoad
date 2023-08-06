using LFrame.Core.Event;
using UnityEngine;

namespace DigitalHuarongRoad.Share
{
    public class PlayerInfoModule
    {
        public class SettingInfo
        {
            public float MusicVolume;
            public float SoundVolume;
        }

        public class GameCoinInfo
        {
            public int Energy;
        }
        private static readonly SettingInfo settingInfo = new SettingInfo();
        private static readonly GameCoinInfo gameCoinInfo = new GameCoinInfo();
        public static readonly PlayerInfoModule Instance = new PlayerInfoModule();

      

        PlayerInfoModule()
        {
            settingInfo.MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
            settingInfo.SoundVolume = PlayerPrefs.GetFloat("SoundVolume");

            gameCoinInfo.Energy = PlayerPrefs.GetInt("Energy");

            if (PlayerPrefs.GetInt("First") == 0)
            {
                MusicVolume = 1;
                SoundVolume = 1;
                Energy = 0;
                
                PlayerPrefs.SetInt("First",1);
                PlayerPrefs.Save();
            } 
        }

        public static float MusicVolume
        {
            get
            {
                return settingInfo.MusicVolume;
            }
            set
            {
                settingInfo.MusicVolume = value;
                EvtManager.Notify(GameEvents.MusicVolumeChange);
                PlayerPrefs.SetFloat("MusicVolume", settingInfo.MusicVolume);
                PlayerPrefs.Save();
            }
        }

        public static float SoundVolume
        {
            get
            {
                return settingInfo.SoundVolume;
            }
            set
            {
                settingInfo.SoundVolume = value;
                EvtManager.Notify(GameEvents.SoundVolumeChange);
                PlayerPrefs.SetFloat("SoundVolume", settingInfo.SoundVolume);
                PlayerPrefs.Save();
            }
        }

        public static int Energy
        {
            get
            {
                return gameCoinInfo.Energy;
            }
            set
            {
                gameCoinInfo.Energy = value;
                EvtManager.Notify(GameEvents.EnergyChange);
                PlayerPrefs.SetFloat("Energy", gameCoinInfo.Energy);
                PlayerPrefs.Save();
            }
        }
    }
}
