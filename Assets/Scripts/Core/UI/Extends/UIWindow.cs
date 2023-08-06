using System;
using LFrame.Core.Tools;
using LFrame.Core.UI;
using UnityEngine;

namespace LFrame.Core.UI
{
    public class UIWindow : MonoBehaviour, IUIWindow
    {
        protected IUIMeta meta;
        protected Canvas panel;

        public virtual IUIMeta Meta(IUIMeta meta = null)
        {
            if (meta != null)
            {
                this.meta = meta;
            }

            return this.meta;
        }

        public Canvas Panel(Canvas panel = null)
        {
            if (panel != null)
            {
                this.panel = panel;
            }

            return this.panel;
        }

        public virtual void OnOpen(params object[] args)
        {
        }

        public virtual void OnClose(Action callback)
        {
            if (callback != null) callback();
        }

        public void OnFocus()
        {
        }

        public void OnBlur()
        {
        }

        public void CloseWD(IUIMeta meta = null)
        {
            if (meta == null)
            {
                UIManager.CloseWD(this.Meta());
            }
            else
            {
                UIManager.CloseWD(meta);
            }
        }
        public void CloseWD(IUIMeta meta ,Action callback)
        {
            UIManager.CloseWD(meta,callback);
        }
        public void CloseWD(Action callback)
        {
            UIManager.CloseWD(this.Meta(),callback);
        }

        public IUIWindow OpenWD(IUIMeta meta, params object[] args)
        {
            return UIManager.OpenWD(meta, args);
        }
    }
}
