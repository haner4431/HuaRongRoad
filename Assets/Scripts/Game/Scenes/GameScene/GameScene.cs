using LFrame.Core.Audio;
using LFrame.Core.Scene;
using LFrame.Core.Tools;

namespace DigitalHuarongRoad
{
    public class GameScene : Scene
    {
        public static readonly GameScene Instance = new GameScene();

        private int gamelevel;
        GameScene()
        {
            name = "Game";
        }

        public override void Start(params object[] param)
        {
            base.Start(param);
            if (param.Length > 0)
            {
                gamelevel = (int)param[0];
            }


        }

        public override void Onload()
        {
            base.Onload();
            AudioManager.Instance.PlayMusic(new[] { "BGM_01", "BGM_02", "BGM_03", "BGM_04" });
            ChessBoard.Instance.StartGame(gamelevel);
        }

        public override void Stop()
        {
            base.Stop();
            AudioManager.Instance.StopAll();
        }
    }
}
