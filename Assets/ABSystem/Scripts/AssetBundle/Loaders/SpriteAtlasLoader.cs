using UnityEngine;
using UnityEngine.U2D;

namespace Tangzx.ABSystem
{
    public class SpriteAtlasLoader : MonoBehaviour
    {
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
//            var sa = Resources.Load<SpriteAtlas>(tag);
//            callback(sa);
            Debug.Log("-----"+tag);
        }
    }
}