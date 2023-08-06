namespace  LFrame.Core.Scene
{
    public interface IScene
    {
        string Name();

        void Onload();
        void Start(params object[] param);

        void Stay();

        void Stop();
    }
}
