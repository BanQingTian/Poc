using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using UnityEngine.EventSystems;

public class NREventCenter
{
    private static Dictionary<ControllerAnchorEnum, NRPointerRaycaster> m_RaycasterDict;

    public static GameObject GetCurrentRaycastTarget()
    {
        var result = GetCurrentRaycastHit();
        if (result.isValid)
        {
            return result.gameObject;
        }
        return null;
    }

    public static RaycastResult GetCurrentRaycastHit()
    {
        NRPointerRaycaster raycaster = GetRaycaster();
        if (raycaster == null)
            return default;
        return raycaster.FirstRaycastResult();
    }

    public static NRPointerRaycaster GetRaycaster()
    {
        return GetRaycaster(NRInput.DomainHand);
    }

    public static NRPointerRaycaster GetRaycaster(ControllerHandEnum handEnum)
    {
        if (m_RaycasterDict == null)
        {
            m_RaycasterDict = new Dictionary<ControllerAnchorEnum, NRPointerRaycaster>();
            var gazeRaycaster = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.GazePoseTrackerAnchor).GetComponentInChildren<NRPointerRaycaster>(true);
            m_RaycasterDict.Add(ControllerAnchorEnum.GazePoseTrackerAnchor, gazeRaycaster);
            var rightHandRaycaster = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightLaserAnchor).GetComponentInChildren<NRPointerRaycaster>(true);
            m_RaycasterDict.Add(ControllerAnchorEnum.RightLaserAnchor, rightHandRaycaster);
            var leftHandRaycaster = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.LeftLaserAnchor).GetComponentInChildren<NRPointerRaycaster>(true);
            m_RaycasterDict.Add(ControllerAnchorEnum.LeftLaserAnchor, leftHandRaycaster);
        }

        NRPointerRaycaster raycaster = null;
        switch (NRInput.RaycastMode)
        {
            case RaycastModeEnum.Gaze:
                m_RaycasterDict.TryGetValue(ControllerAnchorEnum.GazePoseTrackerAnchor, out raycaster);
                break;
            case RaycastModeEnum.Laser:
                var raycasterAnchor = (handEnum == ControllerHandEnum.Right ? ControllerAnchorEnum.RightLaserAnchor : ControllerAnchorEnum.LeftLaserAnchor);
                m_RaycasterDict.TryGetValue(raycasterAnchor, out raycaster);
                break;
            default:
                break;
        }
        return raycaster;
    }
}
