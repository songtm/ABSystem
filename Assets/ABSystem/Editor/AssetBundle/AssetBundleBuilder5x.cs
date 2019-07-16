#if UNITY_5 || UNITY_2017_1_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tangzx.ABSystem
{
    public class AssetBundleBuilder5x : ABBuilder
    {
        public AssetBundleBuilder5x(AssetBundlePathResolver resolver)
            : base(resolver)
        {
        }

        private void SetSpriteAtlasInBuild(List<AssetTarget> all, bool include)
        {
#if USE_SPRITEATLAS
            for (int i = 0; i < all.Count; i++)
            {
                var target = all[i];
                if (target.assetPath.EndsWith(".spriteatlas"))
                {
                    var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(target.assetPath);
                    spriteAtlas.SetIncludeInBuild(include);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public override void Export()
        {
#if USE_SPRITEATLAS
            SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget, false);
#endif
            base.Export(); //分析依赖关系
            var all = AssetBundleUtils.GetAll();
            try
            {
                SetSpriteAtlasInBuild(all, false);
                ExportImp();
            }
            finally
            {
                SetSpriteAtlasInBuild(all, true);
            }
        }


        public void ExportImp()
        {
            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);

            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            //标记所有 asset bundle name
            var all = AssetBundleUtils.GetAll();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = target.bundleName;
                    build.assetNames = new string[] {target.assetPath};
                    list.Add(build);
                }
            }

            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);
            //开始打包
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle |
                                                   BuildAssetBundleOptions.ChunkBasedCompression |
                                                   BuildAssetBundleOptions.DisableWriteTypeTree;
            BuildPipeline.BuildAssetBundles(pathResolver.BundleSavePath, list.ToArray(), buildOptions,
                EditorUserBuildSettings.activeBuildTarget);
            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);
            AssetBundle ab = AssetBundle.LoadFromFile(pathResolver.BundleSavePath + "/AssetBundles");
            AssetBundleManifest manifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            //hash
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                {
                    Hash128 hash = manifest.GetAssetBundleHash(target.bundleName);
                    target.bundleCrc = hash.ToString();
                }
            }

            this.SaveDepAll(all);
            ab.Unload(true);
            this.RemoveUnused(all);
            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);
        }
    }
}
#endif