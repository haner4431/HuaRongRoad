using System;
using LFrame.Core.Scene;
using LFrame.Core.UI;
using UnityEngine;

namespace DigitalHuarongRoad
{
    public class GameMain : MonoBehaviour
    {
        private static GameMain instance;

        public static GameMain Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        #region 加载界面相关

        /// <summary>
        /// 加载进度条变化函数
        /// </summary>
        public static Func<float> loadingProgressValueChangeFun;

        /// <summary>
        /// 加载完成回调
        /// </summary>
        public static Action loadingCompelete;


        /// <summary>
        /// 获取当前加载进度值
        /// </summary>
        public static float GetLoadingProgress()
        {
            if (loadingProgressValueChangeFun != null)
            {
                return loadingProgressValueChangeFun();
            }
            else
            {
                return 0;
            }
        }

        public static void ExecuteLoadingComplete()
        {
            UIManager.CloseWD(ShareView.UI_LoadingPanel, loadingCompelete);
        }

        /// <summary>
        /// 显示进度加载面板
        /// </summary>
        /// <param name="valueUpdate">进度值更新函数，值限定为0~1</param>
        /// <param name="loadingCompeleteCallback">加载完成回调函数</param>
        public static void ShowLoadingProgress(Func<float> valueUpdate, Action loadingCompeleteCallback)
        {
            UIManager.OpenWD(ShareView.UI_LoadingPanel);
            loadingProgressValueChangeFun = valueUpdate;
            loadingCompelete = loadingCompeleteCallback;
        }

        /// <summary>
        /// 加载状态还原
        /// </summary>
        public static void ResetLoadingStatus()
        {
            loadingProgressValueChangeFun = null;
            loadingCompelete = null;
        }

        #endregion


        public void StartGame(int gamelevel)
        {
            SceneManager.GotoScene("Game", gamelevel);
        }
    }
}
