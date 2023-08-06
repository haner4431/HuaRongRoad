using System;
using UnityEngine;

namespace LFrame.Core.UI
{
    public interface IUIWindow
    {
        
        /// <summary>
        /// 获取和设置window元数据
        /// </summary>
        /// <returns></returns>
        IUIMeta Meta(IUIMeta meta = null);

        /// <summary>
        /// 获取和设置ui载体
        /// </summary>
        /// <returns></returns>
        Canvas Panel(Canvas panel = null);
        
        /// <summary>
        /// 生命周期，打开窗口
        /// </summary>
        /// <param name="args">透传参数</param>
        void OnOpen(params object[] args);
        
        /// <summary>
        /// 生命周期，关闭窗口
        /// </summary>
        void OnClose(Action callback = null);
        
        /// <summary>
        /// 生命周期，窗口聚焦
        /// </summary>
        void OnFocus();
        
        /// <summary>
        /// 生命周期，窗口失焦
        /// </summary>
        void OnBlur();
    }
}
