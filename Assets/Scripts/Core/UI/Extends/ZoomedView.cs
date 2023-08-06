using System;
using DG.Tweening;
using LFrame.Core.Tools;
using UnityEngine;

namespace LFrame.Core.UI
{
    public class ZoomedView : UIWindow
    {
        public float zoomedInTime = 0.5f;
        public float zoomedOutTime = 0.5f;

        public float zoomScaleFactor = 0.01f;

        private Tweener inTweener = null;
        private Tweener outTweener = null;

        private Vector3 outScale = Vector3.zero;
        private Vector3 inScale = Vector3.zero;

        // private void Awake()
        // {
        //     inScale = transform.localScale;
        //     outScale = transform.localScale
        //                * zoomScaleFactor;
        // }
        private bool isAwake;

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            Helper.SetActiveState(gameObject, true);
            if (inTweener != null)
                inTweener.Kill();
            if (outTweener != null)
                outTweener.Kill();
            if (isAwake == false)
            {
                inScale = transform.localScale;
                outScale = transform.localScale
                           * zoomScaleFactor;
                isAwake = true;
            }

            transform.localScale = outScale;
            inTweener = transform.DOScale(inScale, zoomedInTime);
            inTweener.SetEase(Ease.OutExpo);
        }

        public override void OnClose(Action callback)
        {
           
            if (inTweener != null)
                inTweener.Kill();
            if (outTweener != null)
                outTweener.Kill();
            transform.localScale = inScale;
            outTweener = transform.DOScale(outScale, zoomedOutTime);
            outTweener.SetEase(Ease.OutExpo);
            outTweener.OnComplete(() =>
            {
                base.OnClose(callback);
                Helper.SetActiveState(gameObject, false);
            });
        }
    }
}
