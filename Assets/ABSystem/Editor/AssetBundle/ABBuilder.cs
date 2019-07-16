using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tangzx.ABSystem
{
    public class ABBuilder
    {
        protected AssetBundleDataWriter dataWriter = new AssetBundleDataBinaryWriter();
        protected AssetBundlePathResolver pathResolver;

        public ABBuilder() : this(new AssetBundlePathResolver())
        {

        }

        public ABBuilder(AssetBundlePathResolver resolver)
        {
            this.pathResolver = resolver;
            this.InitDirs();
            AssetBundleUtils.pathResolver = pathResolver;
        }

        void InitDirs()
        {
            new DirectoryInfo(pathResolver.BundleSavePath).Create();
//            new FileInfo(pathResolver.HashCacheSaveFile).Directory.Create();
        }

        public void Begin()
        {
            EditorUtility.DisplayProgressBar("Loading", "Loading...", 0.1f);
            AssetBundleUtils.Init();
        }

        public void End()
        {
//            AssetBundleUtils.SaveCache();
            AssetBundleUtils.ClearCache();
            EditorUtility.ClearProgressBar();
        }

        public virtual void Analyze()
        {
            var all = AssetBundleUtils.GetAll();
            foreach (AssetTarget target in all)
            {
                target.Analyze();
            }
            all = AssetBundleUtils.GetAll();
            foreach (AssetTarget target in all)
            {
                target.Merge();
            }
            all = AssetBundleUtils.GetAll();
            foreach (AssetTarget target in all)
            {
                target.BeforeExport();
            }
        }

        public virtual void Export()
        {
            this.Analyze();
        }

        public void AddRootTargets(DirectoryInfo bundleDir, PackMode fPackMode, string parttern = null,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (string.IsNullOrEmpty(parttern))
                parttern = "*.*";

            FileInfo[] prefabs = bundleDir.GetFiles(parttern, searchOption);

            foreach (FileInfo file in prefabs)
            {
                if (file.Extension.Contains("meta"))
                    continue;
                AssetTarget target = AssetBundleUtils.Load(file);
                target.packTag = AssetBundleUtils.GetPackTag(bundleDir, file, fPackMode, parttern);
                target.exportType = AssetBundleExportType.Root;
            }
        }

        protected void SaveDepAll(List<AssetTarget> all, AssetBundleManifest manifest)
        {
            string path = Path.Combine(pathResolver.BundleSavePath, pathResolver.DependFileName);

            if (File.Exists(path))
                File.Delete(path);

            List<AssetTarget> exportList = new List<AssetTarget>();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                    exportList.Add(target);
            }
            AssetBundleDataWriter writer = dataWriter;
            writer.Save(path, exportList.ToArray(), manifest);
        }

        public void SetDataWriter(AssetBundleDataWriter w)
        {
            this.dataWriter = w;
        }

        /// <summary>
        /// 删除未使用的AB，可能是上次打包出来的，而这一次没生成的
        /// </summary>
        /// <param name="all"></param>
        protected void RemoveUnused(List<AssetTarget> all)
        {
            HashSet<string> usedSet = new HashSet<string>();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                    usedSet.Add(target.bundleName);
            }

            DirectoryInfo di = new DirectoryInfo(pathResolver.BundleSavePath);
            FileInfo[] abFiles = di.GetFiles("*.ab");
            for (int i = 0; i < abFiles.Length; i++)
            {
                FileInfo fi = abFiles[i];
                if (usedSet.Add(fi.Name))
                {
                    Debug.Log("Remove unused AB : " + fi.Name);

                    fi.Delete();
                    //for U5X
                    File.Delete(fi.FullName + ".manifest");
                }
            }
        }
    }
}
