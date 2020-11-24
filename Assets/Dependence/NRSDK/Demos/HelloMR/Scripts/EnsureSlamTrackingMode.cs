using UnityEngine;

namespace NRKernal.NRExamples
{
    public class EnsureSlamTrackingMode : MonoBehaviour
    {
        [SerializeField]
        private NRHMDPoseTracker.TrackingType m_TrackingType = NRHMDPoseTracker.TrackingType.Tracking6Dof;

        void Start()
        {
            switch (m_TrackingType)
            {
                case NRHMDPoseTracker.TrackingType.Tracking6Dof:
                    NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo6Dof();
                    break;
                case NRHMDPoseTracker.TrackingType.Tracking3Dof:
                    NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo3Dof();
                    break;
                case NRHMDPoseTracker.TrackingType.Tracking0Dof:
                    NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo0Dof();
                    break;
                default:
                    break;
            }
        }
    }
}