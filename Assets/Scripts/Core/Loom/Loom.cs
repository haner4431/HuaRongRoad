using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LFrame.Core
{
    public class Loom : MonoBehaviour
    {
        private static Loom instance;

        private static Loom Instance
        {
            get
            {
                return instance;
            }
        }

        private static Queue<Action> mainTreadTasks = new Queue<Action>(10000);

        public static void Initialise(Transform root)
        {
            root.AddComponent<Loom>();
        }

        private void Awake()
        {
            instance = this;
        }

        public void RunMainThread(Action action)
        {
            mainTreadTasks.Enqueue(action);
        }

        public static void RunMainTrd(Action action)
        {
            if (Instance != null)
            {
                Instance.RunMainThread(action);
            }
        }


        #region 协程封装
        public static Coroutine StartCR(IEnumerator cr)
        {
            if(cr == null) return null;
            return Instance.StartCoroutine(cr);
        }
        
        public static void StopCR(Coroutine cr)
        {
            if(cr == null) return;
            Instance.StopCoroutine(cr);
        }
        
        #endregion

        private void OnDestroy()
        {
            mainTreadTasks.Clear();
        }

        public static int maximumRunTask = 500;

        private void Update()
        {
            int count = maximumRunTask;
            while (count != 0)
            {
                if (mainTreadTasks.Count == 0)
                {
                    break;
                }
                else
                {
                    mainTreadTasks.Dequeue()?.Invoke();
                    count--;
                }
            }
        }
    }
}
