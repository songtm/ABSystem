using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tangzx.ABSystem
{
    public class AssetBundleDataBinaryWriter : AssetBundleDataWriter
    {
        public override void Save(Stream stream, AssetTarget[] targets, AssetBundleManifest manifest, Dictionary<string, HashSet<AssetTarget>> abAssets)
        {
            BinaryWriter sw = new BinaryWriter(stream);
            //写入文件头判断文件类型用，ABDB 意思即 Asset-Bundle-Data-Binary
            sw.Write(new char[] { 'A', 'B', 'D', 'B' });

            List<string> bundleNames = new List<string>(manifest.GetAllAssetBundles());

            //写入文件名池
            sw.Write(bundleNames.Count);
            for (int i = 0; i < bundleNames.Count; i++)
            {
                sw.Write(bundleNames[i]);
            }

            foreach (var bundleName in bundleNames)
            {
                //bundle name
                sw.Write(bundleNames.IndexOf(bundleName));
                //hash
                sw.Write(manifest.GetAssetBundleHash(bundleName).ToString());

                var resCount = abAssets[bundleName].Count;
                sw.Write(resCount);
                foreach (var target in abAssets[bundleName])
                {
                    sw.Write(AssetBundleUtils.ConvertToABName(target.assetPath));
                }

                //type
//                sw.Write((int)target.compositeType);
                //写入依赖信息
                var allDependencies = manifest.GetAllDependencies(bundleName);
                sw.Write(allDependencies.Length);
                foreach (var dependency in allDependencies)
                {
                    sw.Write(bundleNames.IndexOf(dependency));
                }
            }
            sw.Close();
        }
    }
}