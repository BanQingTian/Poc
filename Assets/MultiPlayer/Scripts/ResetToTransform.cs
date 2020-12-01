using UnityEngine;
using NRKernal;

public class ResetToTransform : MonoBehaviour
{
    public Transform camRoot;
    public Transform camRig;
    public Transform targetMarker;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetToTarget();
        }
    }

    private void ResetToTarget()
    {
        var rootPose = GetAlignedPose(camRig, targetMarker.position, targetMarker.rotation);
        camRoot.position = rootPose.position;
        camRoot.rotation = rootPose.rotation;
    }

    private Pose GetAlignedPose(Transform cameraRig, Vector3 markerPosition, Quaternion markerRrotation)
    {
        var marker_in_world = ConversionUtility.GetTMatrix(markerPosition, markerRrotation);
        var world_in_marker = Matrix4x4.Inverse(marker_in_world);
        var alignedPos = ConversionUtility.GetPositionFromTMatrix(world_in_marker);
        var alignedRot = ConversionUtility.GetRotationFromTMatrix(world_in_marker);
        return new Pose(alignedPos, alignedRot);
    }
}
