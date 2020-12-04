
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using NRKernal;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(ARInteractiveItem))]
public class MoveablePhysicsObject : MonoBehaviour
{
    public bool resetPhysicsOnDrop = true;
    public float distanceIncrementOnSwipe = 3f;
    public float distanceFromControllerMin = 0.5f;
    public float distanceFromControllerMax = 15f;
    public float scaleOfOriginMin = 0.1f;
    public float scaleOfOriginMax = 10f;
    public float scaleSensitivity = 30f;

    [Range(MIN_MASS, MAX_MASS)]
    public float rigidbodyMass = 0.2f;
    public bool faceCameraOnDrag = true;
    public bool useReverseFoward = true;
    public bool enableScale = false;
    public bool enableRotate = false;
    public bool alwaysOnGround = false;
    public bool useWorldUpward = false;

    private Rigidbody rigidbodyCmp;
    private bool objectInverted;
    private float orientationSmoothingTime = 0.01f;
    private bool useGravityDefault;
    private bool isKinematicDefault;
    private float controlZDistance;
    private float controlZDistanceSpeed;
    private float targetYRotationFromInput;
    private float controlTension;
    private float weightScale;
    private float touchPosXRemap;
    private float touchPosYRemap;
    private Vector3 maxScale;
    private Vector3 minScale;
    private Vector3 controlTransformPosition;
    private Vector3 normalizedForward;
    private Quaternion objectStartRotation;
    private Quaternion targetOrientationDelta;
    private Quaternion orientationDelta;
    private Quaternion verifyRotation;
    private ARInteractiveItem interactiveItem;
    private float originScaleRate;
    private Quaternion rotationOnSelect;
    private Vector3 scaleOnSelect;
    private Vector3 awakeScale;
    private Transform controlTransform;
    private Transform camRoot;

    public Transform ControlTransform
    {
        get
        {
            if (controlTransform == null)
            {
                if (dragRaycaster)
                    controlTransform = dragRaycaster.transform;
            }
            return controlTransform;
        }
    }

    public Vector3 ControlPosition => ControlTransform.position;
    public Vector3 ControlForward => ControlTransform.forward;
    public Quaternion ControlRotation => ControlTransform.rotation;
    public Quaternion InverseControllerOrientation => inverseControllerOrientation;
    public static bool IsAnyObjectPickedUp { get; private set; }
    public float CurrentScaleRate { get; private set; } = 1f;

    private const float NORMALIZATION_EPSILON = 0.00001f;
    private const float MAX_ANGULAR_DELTA = 15f;
    private const float MIN_MASS = 0.01f;
    private const float MAX_MASS = 10f;

    private int lastStateChangeFrame = -1;
    private Quaternion initialControllerOrientation;
    private Quaternion inverseControllerOrientation;
    private Vector2 deltaTouch = Vector2.zero;

    void Awake()
    {
        interactiveItem = gameObject.GetComponent<ARInteractiveItem>();
        if (!interactiveItem)
            interactiveItem = gameObject.AddComponent<ARInteractiveItem>();
        interactiveItem.OnSelected += OnSelect;
        interactiveItem.OnDeselected += OnDeselect;
        camRoot = NRInput.CameraCenter;

        awakeScale = transform.localScale;
        minScale = awakeScale * scaleOfOriginMin;
        maxScale = awakeScale * scaleOfOriginMax;

        enableRotate = enableRotate && (!enableScale);
        faceCameraOnDrag = (!enableRotate) && faceCameraOnDrag;
    }

    void Update()
    {
        if (interactiveItem.IsBeingSelected)
        {
            OnDrag();
        }
        else
        {
            dragRaycaster = null;
        }
    }

    void FixedUpdate()
    {
        if (interactiveItem.IsBeingSelected && rigidbodyCmp)
        {
            DragRigidbody();
        }
    }

    void OnDestroy()
    {
        interactiveItem.OnSelected -= OnSelect;
        interactiveItem.OnDeselected -= OnDeselect;
    }

    private NRPointerRaycaster dragRaycaster;
    private FlexLaserVisual flexLaserVisual;

    protected void OnDrag()
    {
        if (dragRaycaster == null)
            return;
        UpdateControlTransform();
        if (faceCameraOnDrag)
        {
            //保持被拖拽物的Y轴在这个平面上：手柄与物体的连线与世界Y轴所在的平面
            Vector3 toward = ControlTransform.position - transform.position;
            Vector3 upward = useWorldUpward ? Vector3.up : Vector3.Cross(Vector3.ProjectOnPlane(ControlTransform.right, Vector3.up), toward);
            Vector3 targetPos = useReverseFoward ? (transform.position * 2f - camRoot.position) : camRoot.position;
            if (useWorldUpward)
                targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            transform.LookAt(targetPos, upward);
        }
        else
        {
            transform.rotation = objectStartRotation * verifyRotation;
        }
    }

    protected void OnSelect()
    {
        dragRaycaster = interactiveItem.SelectingRaycaster;
        if(dragRaycaster)
            flexLaserVisual = dragRaycaster.gameObject.GetComponentInChildren<FlexLaserVisual>(true);
        if (flexLaserVisual)
            flexLaserVisual.SelectedObject = transform;
        CurrentScaleRate = transform.localScale.x / awakeScale.x;
        originScaleRate = CurrentScaleRate;
        rotationOnSelect = transform.rotation;
        scaleOnSelect = transform.localScale;
        if (rigidbodyCmp == null)
            InitRigidbody();

        initialControllerOrientation = ControlTransform.rotation;

        // Store inverse initial controller quaternion, for performance.
        inverseControllerOrientation = Quaternion.Inverse(initialControllerOrientation);

        // Perform the transformation relative to control.
        Vector3 vectorToObject = transform.position - ControlPosition;
        float d = vectorToObject.magnitude;
        controlTransformPosition = transform.position;
        objectStartRotation = transform.rotation;
        verifyRotation = Quaternion.identity;

        // If the distance vector cannot be normalized, use the look vector.
        if (d > NORMALIZATION_EPSILON)
        {
            normalizedForward = vectorToObject / d;
        }
        else
        {
            d = 0;
            normalizedForward = ControlForward;
        }

        // Reset distance interpolation values to current values.
        controlZDistance = d;
        // Reset orientation interpolation values to 0.
        targetOrientationDelta = orientationDelta = Quaternion.identity;

        // Get the up vector for the object.
        Vector3 objectUp = transform.TransformDirection(Vector3.up);
        // Get the dot product of the object up vector and the world up vector.
        float dotUp = Vector3.Dot(objectUp, Vector3.up);
        // Mark whether the object is upside down or rightside up.
        objectInverted = dotUp < 0;

        lastStateChangeFrame = Time.frameCount;

        IsAnyObjectPickedUp = true;
    }

    protected void OnDeselect()
    {
        if (flexLaserVisual)
            flexLaserVisual.SelectedObject = null;
        flexLaserVisual = null;
        ResetRigidbody();
        lastStateChangeFrame = Time.frameCount;
        IsAnyObjectPickedUp = false;
    }

    private void InitRigidbody()
    {
        rigidbodyCmp = gameObject.GetComponent<Rigidbody>();
        if (rigidbodyCmp == null)
            rigidbodyCmp = gameObject.AddComponent<Rigidbody>();
        rigidbodyCmp.isKinematic = true;
        rigidbodyCmp.useGravity = false;
        rigidbodyCmp.mass = rigidbodyMass;
        rigidbodyCmp.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbodyCmp.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        // Store the rigidbody gravity setting.
        useGravityDefault = rigidbodyCmp.useGravity;
        // Store the rigidbody kinematic setting.
        isKinematicDefault = rigidbodyCmp.isKinematic;
    }

    //使用touchpad输入
    private void UpdateTouchPad()
    {
        if (NRInput.IsTouching())
        {
            deltaTouch = NRInput.GetDeltaTouch();
            float touchMoveY = Mathf.Abs(deltaTouch.x) < 2f * Mathf.Abs(deltaTouch.y) ? deltaTouch.y : 0f;
            controlZDistance += touchMoveY * distanceIncrementOnSwipe * Time.deltaTime * 50f;
            controlZDistance = Mathf.Clamp(controlZDistance, distanceFromControllerMin, distanceFromControllerMax);

            if (touchMoveY == 0f)
            {
                if (enableScale)
                {
                    CurrentScaleRate += scaleSensitivity * Time.deltaTime * deltaTouch.x;
                    CurrentScaleRate = Mathf.Clamp(CurrentScaleRate, scaleOfOriginMin, scaleOfOriginMax);
                    transform.localScale = awakeScale * CurrentScaleRate;
                }
                else if (enableRotate)
                {
                    touchPosXRemap = -deltaTouch.x * (objectInverted ? -1f : 1f);
                    verifyRotation = verifyRotation * Quaternion.AngleAxis(touchPosXRemap * Mathf.Rad2Deg, Vector3.up);
                }
            }
        }
    }

    private void UpdateControlTransform()
    {
        UpdateTouchPad();

        // Compute orientation delta from selection.
        targetOrientationDelta = ControlRotation * InverseControllerOrientation;

        // If we are smoothing orientation, do it!
        if (orientationSmoothingTime > 0)
        {
            // Adjust speed of smoothing based on the distance between the target offset, and current offset.
            float speed = Quaternion.Angle(orientationDelta, targetOrientationDelta);
            speed = Mathf.Clamp01(speed / MAX_ANGULAR_DELTA);
            float smoothedDeltaTime = (speed * Time.deltaTime) / orientationSmoothingTime;
            // Apply the delta.
            orientationDelta = Quaternion.Slerp(orientationDelta, targetOrientationDelta, smoothedDeltaTime);
            // Otherwise assign it directly.
        }
        else
        {
            orientationDelta = targetOrientationDelta;
        }

        // Compute orientation delta from selection.
        targetOrientationDelta = ControlRotation * InverseControllerOrientation;

        // Assign the position of the control transform.
        controlTransformPosition = controlZDistance * (orientationDelta * normalizedForward) + ControlPosition;

        if (alwaysOnGround)
        {
            BoxCollider bc = interactiveItem.GetComponent<BoxCollider>();
            if (bc)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + bc.center, Vector3.down, out hit, 500f))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.95f)
                    {
                        float y = hit.point.y + (bc.size.y / 2f - bc.center.y) * transform.localScale.y;
                        controlTransformPosition = new Vector3(controlTransformPosition.x, y, controlTransformPosition.z);
                    }
                }
            }
        }

        // Get the distance between the control transform and the controller transform.
        Vector3 targetToControl = ControlPosition - controlTransformPosition;

        // Increase tension when the control transform is closer to the controller transform.
        controlTension = Mathf.Clamp01((distanceFromControllerMax - distanceFromControllerMin) /
                                       (targetToControl.magnitude - distanceFromControllerMin + 0.0001f));

        // Modifies movement responsiveness based on the mass of the rigidbody.
        weightScale = Mathf.Clamp((MAX_MASS / rigidbodyCmp.mass), MIN_MASS, MAX_MASS);
    }

    // Update the rigidbody so it follows the control transform.
    private void DragRigidbody()
    {
        // Turn gravity off for this rigidbody.
        rigidbodyCmp.useGravity = false;
        // Make this rigidbody not kinematic.
        rigidbodyCmp.isKinematic = false;
        // Update the position for the rigidbody.
        MoveRigidbody();
    }

    // Sets the velocity of the rigidbody.
    private void MoveRigidbody()
    {
        // Get the vector from the control transform to the rigidbody.
        Vector3 forceDirection = controlTransformPosition - rigidbodyCmp.position;
        float distanceFromControlTransform = forceDirection.magnitude;

        // Normalize the rigidbody velocity when it is more than one unit from the target.
        if (distanceFromControlTransform > 1.0f)
        {
            forceDirection = forceDirection.normalized;
        }
        else // Otherwise, scale it by the distance to the target.
        {
            forceDirection = forceDirection * distanceFromControlTransform;
        }

        // Set the desired max velocity for the rigidbody.
        Vector3 targetVelocity = forceDirection * weightScale;
        if (targetVelocity.sqrMagnitude > 0.02f)
        {
            if (CheckOverlapColliders())
            {
                var proj = Vector3.Dot(targetVelocity, transform.forward) * transform.forward;
                var perp = targetVelocity - proj;
                targetVelocity = perp;
            }
        }

        // Have the rigidbody accelerate until it reaches target velocity.
        float timeStep = Mathf.Clamp01(Time.fixedDeltaTime * controlTension * 8.0f);
        rigidbodyCmp.velocity += timeStep * (targetVelocity - rigidbodyCmp.velocity);
    }

    // Clear all state and mark the object as ready for selection.
    public void OnReset()
    {
        ResetControlTransform();
        lastStateChangeFrame = -1;
        transform.position = awakeScale * originScaleRate;
        transform.rotation = rotationOnSelect;
        transform.localScale = scaleOnSelect;
    }

    // Reset the control transform.
    private void ResetControlTransform()
    {
        if (ControlTransform != null)
        {
            controlTransformPosition = ControlPosition;
        }
        targetYRotationFromInput = 0f;
        controlTension = 0f;
    }

    // Reset rigidbody properties.
    private void ResetRigidbody()
    {
        if (rigidbodyCmp == null)
            return;
        rigidbodyCmp.useGravity = useGravityDefault;
        rigidbodyCmp.isKinematic = isKinematicDefault;
        if (resetPhysicsOnDrop)
        {
            rigidbodyCmp.velocity = Vector3.zero;
            rigidbodyCmp.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        controlZDistance = Vector3.Distance(ControlTransform.position, transform.position);
    }

    private void OnCollisionExit(Collision other)
    {
        controlZDistance = Vector3.Distance(ControlTransform.position, transform.position);
    }

    private BoxCollider m_MovableCollider;
    private Collider[] m_OverlapedColliders = new Collider[32];
    private bool CheckOverlapColliders()
    {
        if (m_MovableCollider == null)
        {
            m_MovableCollider = interactiveItem.GetComponent<BoxCollider>();
        }

        Vector3 halfExtents = Vector3.Scale(m_MovableCollider.extents, transform.localScale);
        int count = Physics.OverlapBoxNonAlloc(m_MovableCollider.transform.position, halfExtents, m_OverlapedColliders, m_MovableCollider.transform.rotation);

        for (int i = 0; i < count; i++)
        {
            var collider = m_OverlapedColliders[i];
            if (!collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }
        return false;
    }
}
