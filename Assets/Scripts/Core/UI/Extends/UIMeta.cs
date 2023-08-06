namespace LFrame.Core.UI
{
    public class UIMeta: IUIMeta
    {
        private string path;
        private int renderLayer;
        private bool focus;
        private bool cached;
        private bool multiple;

        public UIMeta(string path,int renderLayer = 20000,bool cached = true,bool focus = false,bool multiple = false)
        {
            this.path = path;
            this.renderLayer = renderLayer;
            this.focus = focus;
            this.cached = cached;
            this.multiple = multiple;
        }
        public string Path()
        {
            return path;
        }

        public int RenderLayer()
        {
            return renderLayer;
        }

        public bool Focus()
        {
            return focus;
        }

        public bool Cached()
        {
            return cached;
        }

        public bool Multiple()
        {
            return multiple;
        }
    }
}
