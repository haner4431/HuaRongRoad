using System;
using System.Collections;
using DigitalHuarongRoad;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class UI_LoadingPanel :  View
{
    private Slider loadSlider;
    private Button B_Skip;
    private Coroutine watchNavInitProgressCR;

    private float curProgress;
    private void Awake()
    {
        loadSlider = Helper.GetComponent<Slider>(transform,"Load_Slider");

#if UNITY_EDITOR
        B_Skip = Helper.GetComponent<Button>(transform, "B_Skip");
        B_Skip.onClick.AddListener((() =>
        {
            OpenWD(InitView.UI_InitMain);
            CloseWD();
        }));
#else
	          Helper.SetActiveState(B_Skip, false);
#endif


        loadSlider.value = 0;
    }

    private void Update()
    {
        if (curProgress >= 1)
        {
            loadSlider.value = 1;
        }
        else
        {
            loadSlider.value = Mathf.Lerp(loadSlider.value, curProgress, 0.1f);
        }
       
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        watchNavInitProgressCR = StartCoroutine(WatchNavInitProgress());
    }

    public override void OnClose(Action callback)
    {
        base.OnClose(callback);
        if(loadSlider !=null)
            loadSlider.value = 0;
        if (watchNavInitProgressCR != null)
        {
            StopCoroutine(watchNavInitProgressCR);
            watchNavInitProgressCR = null;
        }
       
    }
    

    private IEnumerator WatchNavInitProgress()
    {
        while (loadSlider.value != 1)
        {
            curProgress = GameMain.GetLoadingProgress();//Navigation.Instance.QueryTableSize / (float)181440;
            yield return null;
        }
        GameMain.ExecuteLoadingComplete();
        GameMain.ResetLoadingStatus();
       
        yield break;
    }
    
}
