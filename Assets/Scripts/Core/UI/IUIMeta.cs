namespace LFrame.Core.UI
{
    public interface IUIMeta
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        string Path();
        
        /// <summary>
        /// 渲染层级
        /// </summary>
        int RenderLayer();
        
        /// <summary>
        /// 是否始终聚焦
        /// </summary>
        bool Focus();
        
        /// <summary>
        /// 是否缓存
        /// </summary>
        bool Cached();
        
        /// <summary>
        /// 是否多实例
        /// </summary>
        bool Multiple();
    }
}
