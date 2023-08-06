using System.Collections;
using DG.Tweening;
using DigitalHuarongRoad;
using LFrame.Core;
using LFrame.Core.Audio;
using UnityEngine;
public class CameraCtrl : MonoBehaviour
{
    [SerializeField]private  float _maxY;
    [SerializeField]private  float _maxX;
    public Transform viewport1;
    public Transform viewport2;
    public int currentViewport;
    private float Pitch;
    private float Yaw;
    private Tweener posTweener;
    private Tweener rotTweener;
    public float posTransitionTime = 1.5f;
    public float rotTransitionTime= 1.5f;

    private static CameraCtrl instance;

    public static CameraCtrl Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
        MoveTrans(1);
        Loom.StartCR(Ready_CR());
    }

    private IEnumerator Ready_CR()
    {
        yield return new WaitUntil(() => ChessBoard.Instance != null);
        ChessBoard.Instance.OnClicked += ClickHandle;
    }

    public void MoveTrans(int point)
    {
        switch (point)
        {
            case 1:
            {
                currentViewport = 1;
                MoveTrans(viewport1);
                break;
            }  
            case 2:
            {
                currentViewport = 2;
                MoveTrans(viewport2);
                break;
            }
            
            default: break;
        }
    }
    private void MoveTrans(Transform target)
    {
        if (posTweener!=null)
        {
            posTweener.Kill();
        }

        posTweener = transform.DOMove(target.position, posTransitionTime);
        posTweener.SetEase(Ease.OutExpo);
        posTweener.OnComplete(() => posTweener = null);
        
        if (rotTweener!=null)
        {
            rotTweener.Kill();
        }

        rotTweener = transform.DORotate(target.rotation.eulerAngles, rotTransitionTime);
        rotTweener.SetEase(Ease.OutExpo);
        rotTweener.OnComplete(() => rotTweener = null);

            
    }
    private void ClickHandle(GameObject go)
    {
        if (go.CompareTag("ChessArea"))
        {
            MoveTrans(2);
        }
        else
        {
            MoveTrans(1);
        }
        AudioManager.Instance.PlayClip("UI_Click");
    }    
  
}
