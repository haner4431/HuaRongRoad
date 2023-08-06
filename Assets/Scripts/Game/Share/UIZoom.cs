using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace DigitalHuarongRoad.Share
{
    public class UIZoom: MonoBehaviour
    {
        private Tweener showTweener;
        private Tweener showTweener1;
        
        private Vector3 initScale;
        private float enlargeFactor = 1.5f;
        private float duration = 1.5f;

        private bool isAwake;
        private void OnEnable()
        {
            if (showTweener != null)
            {
                showTweener.Kill();
            }

            if (isAwake == false)
            {
                isAwake = true;
                initScale = transform.localScale;
            }
            showTweener = transform.DOScale(initScale * enlargeFactor,duration / 2);
            showTweener.SetEase(Ease.OutExpo);
            showTweener.OnComplete(() =>
            {
                showTweener = null;
                
                if (showTweener1 != null)
                {
                    showTweener1.Kill();
                }
                showTweener1 = transform.DOScale(initScale ,duration / 2);
                showTweener1.SetEase(Ease.OutExpo);
                showTweener1.OnComplete(() =>
                {
                    showTweener1 = null;
                });
            });

        }

       
    }
}
