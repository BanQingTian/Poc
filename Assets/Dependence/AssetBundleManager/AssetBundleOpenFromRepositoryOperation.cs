using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetBundles
{
    public class AssetBundleOpenFromRepositoryOperation : AssetBundleDownloadOperation
    {
        private AssetBundleRepository m_Repository;

        private UnityWebRequest m_WebRequest;

        private string m_AssetBundlePath;

        public AssetBundleOpenFromRepositoryOperation(string root, string assetBundleName, AssetBundleRepository repository)
            : base(assetBundleName)
        {
            m_Repository = repository;

            m_AssetBundlePath = m_Repository.SearchFile(root, assetBundleName);
            if (!string.IsNullOrEmpty(m_AssetBundlePath))
            {
                m_AssetBundlePath = m_AssetBundlePath.Replace('\\', '/');

#if UNITY_ANDROID
                if (!m_AssetBundlePath.StartsWith("jar:file://") && !m_AssetBundlePath.StartsWith("file://"))
                {
                    m_AssetBundlePath = "file://" + m_AssetBundlePath;
                }
#endif

                m_WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(m_AssetBundlePath);
                m_WebRequest.SendWebRequest();
            }
            else
            {
                this.error = "fail search";
            }
        }

        protected override bool downloadIsDone
        {
            get
            {
                return (m_WebRequest == null) || m_WebRequest.isDone;
            }
        }

        public override string GetSourceURL()
        {
            return m_AssetBundlePath;
        }

        protected override void FinishDownload()
        {
            this.error = m_WebRequest.error;
            if (!string.IsNullOrEmpty(this.error))
                return;

            var bundle = (m_WebRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            if (bundle == null)
            {
                this.error = string.Format("fail to load {0}", m_AssetBundlePath);
            }
            else
            {
                this.assetBundle = new LoadedAssetBundle(bundle);
            }

            m_WebRequest.Dispose();
            m_WebRequest = null;
        }
    }
}
