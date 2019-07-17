using System.Collections.Generic;
using UnityEngine;

namespace Tangzx.ABSystem
{
    public class AssetBundleBuildConfig : ScriptableObject
    {
        public enum Format
        {
            Text,
            Bin
        }

        public Format depInfoFileFormat = Format.Bin;

        public List<AssetBundleFilter> filters = new List<AssetBundleFilter>();
    }

    public enum PackMode
    {
        Indepent,
        AllInOne,
        PerDir,
        PerSubDir
    }
    [System.Serializable]
    public class AssetBundleFilter
    {
        public bool valid = true;
        public string path = string.Empty;
        public string filter = "*.prefab";
        public PackMode packMode = PackMode.Indepent;
    }
}