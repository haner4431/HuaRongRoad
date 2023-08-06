using System.Collections;
using DigitalHuarongRoad;
using LFrame.Core.Tools;
using UnityEngine.SceneManagement;


namespace LFrame.Core.Scene
{
    public class SceneManager
    {
        SceneManager()
        {
            
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += Onload;
        }

        ~SceneManager()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= Onload;
        }

        public static SceneManager instance;

        public static SceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneManager();
                }

                return instance;
            }
        }


        public static IScene last;

        public static IScene Last
        {
            get
            {
                return last;
            }
        }

        public static IScene current;

        public static IScene Current
        {
            get
            {
                return current;
            }
        }

        public void Onload(UnityEngine.SceneManagement.Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == current.Name())
            {
                current.Onload();
            }
            else
            {
                Helper.LogWarning($"实际加载完成的{scene.name}场景和当前不符合{Current.Name()}");
            }
        }

        private IEnumerator SwitchScene(IScene scene, params object[] param)
        {
            last = Current;
            current = scene;
            if(last != null)
                last.Stop();
            current.Start(param);
            var progress = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.Name());
            while (progress.isDone == false)
            {
                GameMain.ShowLoadingProgress(() =>
                {
                    return progress.progress;
                }, null);
                yield return null;
            }

            yield break;
        }

        public static void GotoScene(string name, params object[] param)
        {
            var scene = NameToScene(name);
            if (scene == null || Current == scene) return;
            Loom.StartCR(Instance.SwitchScene(scene, param));
        }

        private static IScene NameToScene(string name)
        {
            switch (name)
            {
                case "Init":
                {
                    return InitScene.Instance;
                    break;
                }
                case "Game":
                {
                    return GameScene.Instance;
                }
                default:
                {
                    Helper.LogError($"不存在名为{name}的场景");
                    break;
                }
            }

            return null;
        }
    }
}
