using System.Collections;
using DigitalHuarongRoad.Share;
using LFrame.Core;
using LFrame.Core.Audio;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;
using SceneManager = LFrame.Core.Scene.SceneManager;

namespace DigitalHuarongRoad
{
    public class Launcher: MonoBehaviour
    {
        public IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            UIManager.Initialise(Helper.GetTransfrom(transform,"UI"));
            Navigation.Intialise(Helper.GetTransfrom(transform,"Nav"));
            Loom.Initialise(Helper.GetTransfrom(transform,"Loom"));
            AudioManager.Initialise(Helper.GetTransfrom(transform, "Audio"));
            var levelInfoManagerInit =  LevelInfoManager.Instance;
            
            yield return new WaitForSeconds(1);
            SceneManager.GotoScene("Init");
            yield break;
        }
    }
}
