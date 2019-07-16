using System;
using System.Collections.Generic;
using System.IO;

namespace Tangzx.ABSystem
{
    public class AssetBundleData
    {
        public string shortName;
        public string bundleName;
        public string hash;
        public string debugName;
        public AssetBundleExportType compositeType;
        public string[] dependencies;
        public bool isAnalyzed;
        public AssetBundleData[] dependList;
    }

    /// <summary>
    /// 文本文件格式说明
    /// *固定一行字符串ABDT
    /// 循环 { AssetBundleData
    ///     *名字(string)
    ///     *短名字(string)
    ///     *Hash值(string)
    ///     *类型(AssetBundleExportType)
    ///     *依赖文件个数M(int)
    ///     循环 M {
    ///         *依赖的AB文件名(string)
    ///     }
    /// }
    /// </summary>
    public class AssetBundleDataReader
    {
        public Dictionary<string, AssetBundleData> infoMap = new Dictionary<string, AssetBundleData>();

        protected Dictionary<string, string> shortName2FullName = new Dictionary<string, string>();//这里 FullName 为 abname
        protected Dictionary<string, string> resName2ABName = new Dictionary<string, string>();

        public virtual void Read(Stream fs)
        {
            StreamReader sr = new StreamReader(fs);
            var header = sr.ReadLine();
            if (header != "ABDT")
                return;

            while (true)
            {
                string bundleName= sr.ReadLine();
                if (string.IsNullOrEmpty(bundleName))
                    break;
                string hash = sr.ReadLine();
                string resCountStr = sr.ReadLine();
                int resCount = Convert.ToInt32(resCountStr);
                string oneResName = "";
                for (int i = 0; i < resCount; i++)
                {
                    string resName = sr.ReadLine();
                    oneResName = resName;
                    if (!resName2ABName.ContainsKey(resName))
                        resName2ABName.Add(resName, bundleName);
                }



                int depsCount = Convert.ToInt32(sr.ReadLine());
                string[] deps = new string[depsCount];

                for (int i = 0; i < depsCount; i++)
                {
                    deps[i] = sr.ReadLine();
                }
                sr.ReadLine(); // skip <------------->

                AssetBundleData info = new AssetBundleData();
                info.debugName = resCount==1 ? oneResName : oneResName+"...";
                info.hash = hash;
                info.bundleName = bundleName;
                info.dependencies = deps;
                infoMap[bundleName] = info;
            }
            sr.Close();
        }

        /// <summary>
        /// 分析生成依赖树
        /// </summary>
        public void Analyze()
        {
            var e = infoMap.GetEnumerator();
            while (e.MoveNext())
            {
                Analyze(e.Current.Value);
            }
        }

        void Analyze(AssetBundleData abd)
        {
            if (!abd.isAnalyzed)
            {
                abd.isAnalyzed = true;
                abd.dependList = new AssetBundleData[abd.dependencies.Length];
                for (int i = 0; i < abd.dependencies.Length; i++)
                {
                    AssetBundleData dep = this.GetAssetBundleInfo(abd.dependencies[i]);
                    abd.dependList[i] = dep;
                    this.Analyze(dep);
                }
            }
        }

        public string GetABName(string debugname)
        {
            resName2ABName.TryGetValue(debugname, out var name);
            return name;
        }
        public string GetFullName(string shortName)
        {
            string fullName = null;
            shortName2FullName.TryGetValue(shortName, out fullName);
            return fullName;
        }

        public AssetBundleData GetAssetBundleInfoByShortName(string shortName)
        {
            string fullName = GetFullName(shortName);
            if (fullName != null && infoMap.ContainsKey(fullName))
                return infoMap[fullName];
            return null;
        }

        public AssetBundleData GetAssetBundleInfo(string fullName)
        {
            if (fullName != null)
            {
                if (infoMap.ContainsKey(fullName))
                    return infoMap[fullName];
            }
            return null;
        }
    }
}