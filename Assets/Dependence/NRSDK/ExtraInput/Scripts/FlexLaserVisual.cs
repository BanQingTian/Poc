using NRKernal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// Represents a ray segment for a series of intersecting rays.
/// This is useful for Hybrid raycast mode, which uses two sequential rays.
public struct PointerRay
{
    /// The ray for this segment of the pointer.
    public Ray ray;

    /// The distance along the pointer from the origin of the first ray to this ray.
    public float distanceFromStart;

    /// Distance that this ray extends to.
    public float distance;
}

/// A flexible laser visual implementation that bends in a vertex shader.
[RequireComponent(typeof(MeshRenderer))]
public class FlexLaserVisual : MonoBehaviour
{
    [SerializeField]
    private NRPointerRaycaster m_Raycaster;

    public float defaultDistance = 1.2f;
    // Transform of the object being selected (overridden reticle position).
    [HideInInspector]
    public Transform SelectedObject;


    [Range(0f, 1f)]
    public float alphaValue = 1f;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock flexMaterialProperties;

    private bool hadSelection = false;
    private float timeSinceLastSelectionChange = 0;

    private int alphaID;
    private int lineJointID;
    private int lineNormalAxisID;

    private Vector3 controlEndPoint;
    private Vector3 controllerToInfluence;
    private Vector3 selectionToInfluence;
    private Vector3 targetPosition;
    private Vector4 shaderNormal;
    private Vector4[] shaderWorldPositions;

    private const int INFLUENCE_COUNT = 101;
    private const float SELECTION_LERP_SPEED = 2f;

    private void Awake()
    {
        defaultDistance = Mathf.Clamp(defaultDistance, m_Raycaster.NearDistance, m_Raycaster.FarDistance);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        shaderWorldPositions = new Vector4[INFLUENCE_COUNT];
        flexMaterialProperties = new MaterialPropertyBlock();
        alphaID = Shader.PropertyToID("_Alpha");
        lineJointID = Shader.PropertyToID("_LineJoint");
        lineNormalAxisID = Shader.PropertyToID("_LineNormalAxis");
    }

    protected virtual void LateUpdate()
    {
        if (!NRInput.LaserVisualActive)
        {
            meshRenderer.enabled = false;
            return;
        }
        meshRenderer.enabled = true;
        Vector3 laserPoint;
        if (SelectedObject == null)
        {
            RaycastResult hit = m_Raycaster.FirstRaycastResult();
            if (hit.isValid)
            {
                float dist = (hit.worldPosition - m_Raycaster.transform.position).magnitude;
                //距离越近，乘以系数越接近1，越远越接近0.3
                dist *= Math.Max(0.2f, (0.95f - 0.02f * dist));
                laserPoint = GetPointAlongPointer(dist);
            }
            else
            {
                laserPoint = m_Raycaster.transform.position + m_Raycaster.transform.forward * defaultDistance;
            }
        }
        else
        {
            float dist = (SelectedObject.position - m_Raycaster.transform.position).magnitude;
            laserPoint = GetPointAlongPointer(dist);
        }
        SetReticlePoint(laserPoint);
        UpdateVisual();
    }

    private Vector3 GetPointAlongPointer(float distance)
    {
        PointerRay pointerRay = GetRayForDistance();
        return pointerRay.ray.GetPoint(distance - pointerRay.distanceFromStart);
    }

    private PointerRay GetRayForDistance()
    {
        PointerRay result = new PointerRay();
        Transform pointerTransform = m_Raycaster.transform;
        if (pointerTransform == null)
        {
            Debug.LogError("Cannot calculate ray when pointerTransform is null.");
            return result;
        }

        result.distance = defaultDistance;
        result.ray = new Ray(pointerTransform.position, pointerTransform.forward);
        return result;
    }

    private void SetReticlePoint(Vector3 worldPosition)
    {
        controlEndPoint = worldPosition;
    }

    private void UpdateVisual()
    {
        float dt = Time.deltaTime;

        Vector3 currentPosition = transform.position;
        Vector3 endPosition = controlEndPoint;

        timeSinceLastSelectionChange += Time.deltaTime;

        // If there is a selection, set the target position for the end of the
        // laser line to the pivot of the selected object.
        if (SelectedObject != null)
        {
            // Update state if changed.
            if (!hadSelection)
            {
                timeSinceLastSelectionChange = 0;
                hadSelection = true;
            }
            targetPosition = Vector3.Lerp(targetPosition,
                                          SelectedObject.position + SelectedObject.rotation * Vector3.zero,
                                          SELECTION_LERP_SPEED * timeSinceLastSelectionChange);
            // If there is no selection, otherwise, lerp the target end point position
            // back to its default value.
        }
        else
        {
            // Update state if changed.
            if (hadSelection)
            {
                timeSinceLastSelectionChange = 0;
                hadSelection = false;
            }
            targetPosition = Vector3.Lerp(targetPosition,
                                          endPosition,
                                          SELECTION_LERP_SPEED * timeSinceLastSelectionChange);
        }

        // Update the position of each influence along the laser curve.
        for (int i = 0; i < INFLUENCE_COUNT; i++)
        {
            // Get the normalized position of the influence along the laser.
            float influencePos = (float)i / INFLUENCE_COUNT;

            // Get the vector from the controller transform to the control end point (reticle).
            controllerToInfluence = Vector3.Lerp(currentPosition, endPosition, influencePos);
            // Get the vector from the selection transform to this influence.
            selectionToInfluence = Vector3.Lerp(currentPosition, targetPosition, influencePos);

            shaderWorldPositions[i] = Vector3.Lerp(controllerToInfluence, selectionToInfluence, influencePos);
        }

        Vector3 a = (Vector3)shaderWorldPositions[INFLUENCE_COUNT / 2] - currentPosition;
        Vector3 b = targetPosition - (Vector3)shaderWorldPositions[INFLUENCE_COUNT / 2];
        Vector3 normal = Vector3.Cross(a, b);

        if (normal.sqrMagnitude > 0.01f)
        {
            normal.Normalize();
        }
        else
        {
            normal = transform.up;
        }

        shaderNormal = normal;

        // Update laser alpha from arm model preferred alpha.
        flexMaterialProperties.SetFloat(alphaID, alphaValue);
        // Pass per-influence properties to the flex laser shader.
        flexMaterialProperties.SetVectorArray(lineJointID, shaderWorldPositions);
        flexMaterialProperties.SetVector(lineNormalAxisID, shaderNormal);

        meshRenderer.SetPropertyBlock(flexMaterialProperties);
    }
}
