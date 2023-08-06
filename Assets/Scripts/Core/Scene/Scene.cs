using LFrame.Core.Tools;

namespace LFrame.Core.Scene
{
    public class Scene: IScene
    {
        protected string name;
        
        public virtual string Name()
        {
            return name;
        }

        public virtual void Onload()
        {
            Helper.LogWarning("{0} Onload",Name());
        }
        public virtual void Start(params object[] param)
        {
            Helper.LogWarning("{0} Start",Name());
        }

        public virtual void Stay()
        {
          
        }

        public virtual void Stop()
        {
            Helper.LogWarning("{0} Stop",Name());
        }
    }
}
