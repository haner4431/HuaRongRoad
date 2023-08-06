using System;
using DigitalHuarongRoad.Share;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine.UI;

namespace DigitalHuarongRoad
{
    public class UI_Setting: ZoomedView
    {
        public Button B_Close;
        public Slider musicSlider;
        public Slider soundSlider;

        private void Awake()
        {
            B_Close = Helper.GetComponent<Button>(B_Close);
            musicSlider = Helper.GetComponent<Slider>(musicSlider);
            soundSlider = Helper.GetComponent<Slider>(soundSlider);
            musicSlider.maxValue = 1;
            musicSlider.value = PlayerInfoModule.MusicVolume;
            soundSlider.maxValue = 1;
            soundSlider.value = PlayerInfoModule.SoundVolume;
            
            B_Close.onClick.AddListener(() =>
            {
                CloseWD();
            });
            
            musicSlider.onValueChanged.AddListener((value) =>
            {
                PlayerInfoModule.MusicVolume = value;
            });
            soundSlider.onValueChanged.AddListener((value) =>
            {
                PlayerInfoModule.SoundVolume = value;
            });
            
        }
    }
}
