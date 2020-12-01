using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetBundles
{
    public class AssetBundleRepository
    {
        protected List<string> m_SearchPaths = new List<string>();

        public string SearchFile(string root, string fileName, bool ignoreStreamingAssets = false)
        {
            string result = SearchFileInSearchPaths(root, fileName);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            if (!ignoreStreamingAssets)
            {
                return Path.Combine(Utility.GetAssetBundlesStreamingAssetsPath(root), fileName);
            }

            return null;
        }

        protected string SearchFileInSearchPaths(string root, string fileName)
        {
            string filePath = null;

            for (int i = 0; i < m_SearchPaths.Count; i++)
            {
                filePath = string.Format("{0}/{1}/{2}", m_SearchPaths[i], root, fileName);

                filePath = filePath.Replace("\\", "/");
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }

        public bool AddSearchPath(string path, bool front = false)
        {
            int index = m_SearchPaths.IndexOf(path);
            if (index >= 0)
            {
                return false;
            }

            if (front)
            {
                m_SearchPaths.Insert(0, path);
            }
            else
            {
                m_SearchPaths.Add(path);
            }

            return true;
        }

        public bool RemoveSearchPath(string path)
        {
            int index = m_SearchPaths.IndexOf(path);
            if (index >= 0)
            {
                m_SearchPaths.RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}
