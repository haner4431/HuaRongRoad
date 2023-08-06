using System;
using LFrame.Core.Tools;

namespace LFrame.Core.UI
{
    public class View: UIWindow
    {
        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
            Helper.SetActiveState(gameObject, true);
        }

        public override void OnClose(Action callback)
        {
            base.OnClose(callback);
            Helper.SetActiveState(gameObject, false);
        }
    }
}
