#if UNITY_5 || UNITY_2017_1_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Tangzx.ABSystem
{
    public class AssetBundleBuilder5x : ABBuilder
    {
        public AssetBundleBuilder5x(AssetBundlePathResolver resolver)
            : base(resolver)
        {

        }

        public override void Export()
        {
            SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget, false);
            base.Export();//分析依赖关系
            ExportImp();
        }


        public void ExportImp()
        {

            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);

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
                    build.assetNames = new string[] { target.assetPath };
                    list.Add(build);
                }
            }
            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);
            //开始打包
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DisableWriteTypeTree;
            BuildPipeline.BuildAssetBundles(pathResolver.BundleSavePath, list.ToArray(), buildOptions, EditorUserBuildSettings.activeBuildTarget);
            AssetBundleManager.Log("building... cur Time " + Time.realtimeSinceStartup);
#if UNITY_5_1 || UNITY_5_2
            AssetBundle ab = AssetBundle.CreateFromFile(pathResolver.BundleSavePath + "/AssetBundles");
#else
            AssetBundle ab = AssetBundle.LoadFromFile(pathResolver.BundleSavePath + "/AssetBundles");
#endif
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