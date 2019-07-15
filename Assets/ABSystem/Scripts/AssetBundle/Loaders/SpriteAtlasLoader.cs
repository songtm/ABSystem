using UnityEngine;
using UnityEngine.U2D;

namespace Tangzx.ABSystem
{
    public class SpriteAtlasLoader : MonoBehaviour
    {
        private AssetBundleManager _bundleManager;

        void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= RequestAtlas;
        }

        void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
        {
            if (_bundleManager == null) _bundleManager = AssetBundleManager.Instance;
            _bundleManager.Load(tag + ".spriteatlas", info =>
            {
                var spriteAtlas = info.Require<SpriteAtlas>(this); //TODO:这里什么时候释放 AB 呢?
                callback(spriteAtlas);
            }, true);
        }
    }
}