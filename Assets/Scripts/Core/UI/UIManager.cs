using System;
using System.Collections.Generic;
using LFrame.Core.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LFrame.Core.UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 打开始ui列表
        /// </summary>
        private List<IUIWindow> openedWindows;


        /// <summary>
        /// 缓存的ui列表
        /// </summary>
        private List<IUIWindow> cachedWindows;

        /// <summary>
        /// UI根Transform
        /// </summary>
        private static Transform root;

        /// <summary>
        /// IUIWindow中的panel，为不同的画布准备
        /// </summary>
        private Canvas canvas;

        public static void Initialise(Transform rootTransform)
        {
            root = rootTransform;
            root.AddComponent<UIManager>();
        }

        private void Awake()
        {
            instance = this;
            canvas = Helper.GetComponent<Canvas>(transform);
            openedWindows = new List<IUIWindow>(50);
            cachedWindows = new List<IUIWindow>(50);
        }

        private void Update()
        {
            if (canvas.worldCamera == null)
            {
                canvas.worldCamera = Camera.main;
            }
        }

        /// <summary>
        /// 判断面板是否打开
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public IUIWindow IsOpened(IUIMeta meta)
        {
            IUIWindow window = null;
            for (int i = 0; i < openedWindows.Count; i++)
            {
                if (openedWindows[i].Meta().Path() == meta.Path())
                {
                    window = openedWindows[i];
                }
            }

            return window;
        }

        public static IUIWindow IsOpenedWD(IUIMeta meta)
        {
            return Instance.IsOpened(meta);
        }

        /// <summary>
        /// 确保UI打开
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public IUIWindow EnsureOpened(IUIMeta meta)
        {
            IUIWindow window = null;
            for (int i = 0; i < openedWindows.Count; i++)
            {
                if (openedWindows[i].Meta().Path() == meta.Path())
                {
                    window = openedWindows[i];
                }
            }

            if (window == null)
            {
                for (int i = 0; i < cachedWindows.Count; i++)
                {
                    if (cachedWindows[i].Meta().Path() == meta.Path())
                    {
                        window = cachedWindows[i];
                    }
                }
            }


            if (window == null)
            {
                GameObject windowIns =
                    Resources.Load<GameObject>(Helper.StringFormat("{0}{1}", Constants.Prefab_PATH, meta.Path()));
                windowIns = GameObject.Instantiate(windowIns, root);
                windowIns.name = windowIns.name.Substring(0, windowIns.name.IndexOf("(Clone)"));
                Canvas panel = Helper.GetComponent<Canvas>(windowIns);
                window = windowIns.GetComponent<IUIWindow>();
                if (window == null)
                {
                    Helper.LogError("[OpenWindow] {0} not attach IUIWindow Component", meta.Path());
                }
                else
                {
                    if (panel != null)
                    {
                        window.Panel(panel);
                    }
                    else
                    {
                        Helper.LogError("[OpenWindow] {0} panel is null", meta.Path());
                    }

                    window.Meta(meta);
                }
               
            }
            
            return window;
        }


        public IUIWindow OpenWindow(IUIMeta meta, params object[] args)
        {
            IUIWindow window = null;

            window = EnsureOpened(meta);

            if (window != null)
            {
                window.OnOpen(args);
                for (int i = 0; i < openedWindows.Count; i++)
                {
                    var raycast = Helper.GetComponent<GraphicRaycaster>((openedWindows[i] as UIWindow).transform);
                    if (raycast.enabled)
                    {
                        raycast.enabled = false;
                        openedWindows[i].OnBlur();
                    }
                }

                cachedWindows.Remove(window);
                openedWindows.Add(window);
                var raycast1 = Helper.GetComponent<GraphicRaycaster>((window as UIWindow).transform);
                if (raycast1.enabled == false)
                {
                    raycast1.enabled = true;
                    window.OnFocus();
                }

                (window as UIWindow).transform.SetAsLastSibling();
            }

            return window;
        }


        public void CloseWindow(IUIMeta meta,Action callback = null)
        {
            IUIWindow window = null;

            if (window == null)
            {
                for (int i = 0; i < openedWindows.Count; i++)
                {
                    var temp1 = openedWindows[i];
                    var temp2 = openedWindows[i].Meta();
                    var temp3 = openedWindows[i].Meta().Path();
                    var temp4 = meta.Path();
                    if (openedWindows[i].Meta().Path() == meta.Path())
                    {
                        window = openedWindows[i];
                        if (meta.Cached())
                        {
                            cachedWindows.Add(window);
                        }
                        else
                        {
                            Destroy((window as UIWindow).gameObject);
                        }
                    }
                }
            }

            if (window != null)
            {
                window.OnClose(callback);
                openedWindows.Remove(window);
            }

            if (openedWindows.Count != 0)
            {
                var raycast =
                    Helper.GetComponent<GraphicRaycaster>(
                        (openedWindows[openedWindows.Count - 1] as UIWindow).transform);
                if (raycast.enabled == false)
                {
                    raycast.enabled = true;
                    openedWindows[openedWindows.Count - 1].OnFocus();
                }
            }
        }

        public void CloseAll()
        {
            int len = openedWindows.Count;
            for (int i = 0; i < len; i++)
            {
                CloseWindow(openedWindows[0].Meta());
            }
            
        }

        public static void CloseAllWD()
        {
            Instance.CloseAll();
        }

        public static IUIWindow OpenWD(IUIMeta meta, params object[] args)
        {
            return Instance.OpenWindow(meta, args);
        }

        public static void CloseWD(IUIMeta meta,Action callback = null)
        {
            Instance.CloseWindow(meta,callback);
        }

        public void Method()
        {
            throw new System.NotImplementedException();
        }
    }
}
