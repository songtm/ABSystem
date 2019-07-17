using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Tangzx.ABSystem
{
    public class AssetBundleDataWriter
    {
        public void Save(string path, AssetTarget[] targets, AssetBundleManifest manifest,
            Dictionary<string, HashSet<AssetTarget>> abAssets)
        {
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            Save(fs, targets, manifest, abAssets);
            SaveRelationMap(Path.GetDirectoryName(path) + "/00dep.dot" , targets);
        }

        private void SaveRelationMap(string path, AssetTarget[] targets)
        {
            string header = @"digraph dep {
    fontname = ""Microsoft YaHei"";
    label = ""AssetBundle 依赖关系""
    nodesep=0.5
    rankdir = ""LR""
    fontsize = 12;
    node [ fontname = ""Microsoft YaHei"", fontsize = 12, shape = ""record"" color=""skyblue""];
    edge [ fontname = ""Microsoft YaHei"", fontsize = 12 , color=""coral""];";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(header);
            foreach (var assetTarget in targets)
            {
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                assetTarget.GetDependencies(deps);
                builder.Append("\t");
                builder.Append('"'+assetTarget.abDebugNameShort+'"');
                if (assetTarget.exportType == AssetBundleExportType.Standalone)
                    builder.Append(" [color=\"red\", fontcolor=\"red\", shape=\"ellipse\", fillcolor=\"lightblue1\", style=\"filled\"]");
                else if (assetTarget.exportType == AssetBundleExportType.Root)
                {
                    builder.Append(
                        $" [color=\"blue\", fontcolor=\"blue\", label=\"{{<f0> {assetTarget.abDebugNameShort} |<f1> {deps.Count} }}\"]");
                }


                builder.AppendLine();
            }

            foreach (var assetTarget in targets)
            {
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                assetTarget.GetDependencies(deps);
                foreach (var target in deps)
                {
                    builder.Append("\t");
                    builder.AppendLine('"'+assetTarget.abDebugNameShort + "\"->\"" + target.abDebugNameShort+'"');
                }

                builder.AppendLine();

            }
            builder.AppendLine("}");
            File.WriteAllText(path, builder.ToString());
        }

        public virtual void Save(Stream stream, AssetTarget[] targets, AssetBundleManifest manifest, Dictionary<string, HashSet<AssetTarget>> abAssets)
        {
            StreamWriter sw = new StreamWriter(stream);
            //写入文件头判断文件类型用，ABDT 意思即 Asset-Bundle-Data-Text
            sw.WriteLine("ABDT");

            var allAssetBundles = manifest.GetAllAssetBundles();
            foreach (var bundleName in allAssetBundles)
            {
                //bundle name
                sw.WriteLine(bundleName);
                //hash
                sw.WriteLine(manifest.GetAssetBundleHash(bundleName));

                var allDependencies = manifest.GetAllDependencies(bundleName);
                int resCount = abAssets[bundleName].Count;
                sw.WriteLine(resCount); //AB包中资源数量
                foreach (var target in abAssets[bundleName])
                {
                    sw.WriteLine(AssetBundleUtils.ConvertToABName(target.assetPath));
                }

                //type
//                sw.WriteLine((int)target.compositeType);
                //写入依赖信息
                sw.WriteLine(allDependencies.Length);
                foreach (var dep in allDependencies)
                {
                    sw.WriteLine(dep);
                }
                sw.WriteLine("<------------->");

            }
            sw.Close();
        }
    }
}