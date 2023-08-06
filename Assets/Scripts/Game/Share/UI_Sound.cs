using System.Collections;
using System.Collections.Generic;
using LFrame.Core.Audio;
using LFrame.Core.Tools;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sound : MonoBehaviour
{
    public enum SoundType
    {
        Click,
        Open,
        Close
    }

   

    public SoundType type;
    public string audio = "";

    private Button button;

    private void Awake()
    {
        button =Helper.GetComponent<Button>(transform);    
        if(button != null)
        {
            button.onClick.AddListener(() =>
            {
                if(type == SoundType.Click)
                    AudioManager.Instance.PlayClip(audio  == ""? "UI_Click" : audio);
            });
        }
    }

    private void OnEnable()
    {
        if (type == SoundType.Open)
            AudioManager.Instance.PlayClip(audio == "" ? "UI_Open" : audio);
    }

    private void OnDisable()
    {
        if (type == SoundType.Close)
            AudioManager.Instance.PlayClip(audio == "" ? "UI_Close" : audio);
    }
}
