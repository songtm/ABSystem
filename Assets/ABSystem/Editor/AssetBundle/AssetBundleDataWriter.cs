using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tangzx.ABSystem
{
    public class AssetBundleDataWriter
    {
        public void Save(string path, AssetTarget[] targets)
        {
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            Save(fs, targets);
            SaveRelationMap(Path.GetDirectoryName(path) + "/00dep.dot" , targets);
        }

        private void SaveRelationMap(string path, AssetTarget[] targets)
        {
            string header = @"digraph dep {
    fontname = ""Microsoft YaHei"";
    fontsize = 12;
    node [ fontname = ""Microsoft YaHei"", fontsize = 12, shape = ""record"" color=""skyblue""];
    edge [ fontname = ""Microsoft YaHei"", fontsize = 12 , color=""blue""];";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(header);
            foreach (var assetTarget in targets)
            {
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                assetTarget.GetDependencies(deps);
                builder.Append("\t");
                builder.Append('"'+assetTarget.bundleShortName+'"');
                if (assetTarget.exportType == AssetBundleExportType.Standalone)
                    builder.Append(" [color=\"red\", fontcolor=\"red\" shape=\"ellipse\"]");
                else if(assetTarget.exportType == AssetBundleExportType.Root)
                    builder.Append(" [color=\"blue\", fontcolor=\"blue\"]");

                builder.AppendLine();
            }

            foreach (var assetTarget in targets)
            {
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                assetTarget.GetDependencies(deps);
                foreach (var target in deps)
                {
                    builder.Append("\t");
                    builder.AppendLine('"'+assetTarget.bundleShortName + "\"->\"" + target.bundleShortName+'"');
                }

            }
            builder.AppendLine("}");
            File.WriteAllText(path, builder.ToString());
        }

        public virtual void Save(Stream stream, AssetTarget[] targets)
        {
            StreamWriter sw = new StreamWriter(stream);
            //写入文件头判断文件类型用，ABDT 意思即 Asset-Bundle-Data-Text
            sw.WriteLine("ABDT");

            for (int i = 0; i < targets.Length; i++)
            {
                AssetTarget target = targets[i];
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                target.GetDependencies(deps);

                //debug name
                sw.WriteLine(target.assetPath);
                //bundle name
                sw.WriteLine(target.bundleName);
                //File Name
                sw.WriteLine(target.bundleShortName);
                //hash
                sw.WriteLine(target.bundleCrc);
                //type
                sw.WriteLine((int)target.compositeType);
                //写入依赖信息
                sw.WriteLine(deps.Count);

                foreach (AssetTarget item in deps)
                {
                    sw.WriteLine(item.bundleName);
                }
                sw.WriteLine("<------------->");
            }
            sw.Close();
        }
    }
}