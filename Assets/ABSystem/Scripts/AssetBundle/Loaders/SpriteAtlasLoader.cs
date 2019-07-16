using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Tangzx.ABSystem
{
    public class SpriteAtlasLoader : MonoBehaviour
    {
        private AssetBundleManager _bundleManager;
        private int i = 0;

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
//            Debug.Log(Time.realtimeSinceStartup + " "+Time.frameCount + " " + tag);
            _bundleManager.Load(tag + ".spriteatlas", info =>
            {
//                i++;
//                Debug.Log(Time.realtimeSinceStartup + " "+Time.frameCount + " " + i);
                var spriteAtlas = info.Require<SpriteAtlas>(this); //TODO:这里什么时候释放 AB 呢?
                callback(spriteAtlas);
//                StartCoroutine(Delay(0.9f + i, callback, spriteAtlas));
            });
        }

        IEnumerator Delay(float sec, System.Action<SpriteAtlas> callback, SpriteAtlas atlas)
        {
            yield return  new WaitForSeconds(sec);
            callback(atlas);
        }
    }
}