using System;
using UnityEngine;
using UnityEngine.EventSystems;
using NRKernal;

public class ARInteractiveItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public static bool forceDeselectOnce = false;

    public event Action OnHover;       
    public event Action OnOut;         
    public event Action OnClick; 
    public event Action OnUp;           
    public event Action OnDown;
    public event Action OnLongPress;
    public event Action OnSelected;
    public event Action OnDeselected;

    public bool IsBeingSelected { get { return SelectingRaycaster != null; } }
    public bool IsMoveable { get { return m_MoveableItem != null; } }
    public bool IsHovering { get; private set; }
    public NRPointerRaycaster SelectingRaycaster { get; private set; }
    public float LongPressInterval { get; protected set; } = 0.9f;

    protected MoveablePhysicsObject m_MoveableItem;
    protected float m_PressDownTime;
    protected bool m_LongPressValid;
    protected virtual float ClickInterval { get { return 0.5f; } }

    protected virtual void Awake()
    {
        m_MoveableItem = GetComponent<MoveablePhysicsObject>();
    }

    protected virtual void Update()
    {
        if (m_LongPressValid)
        {
            CheckLongPress();
        }
        JudgeDrop();
    }

    protected virtual void OnDisable()
    {
        Out();
        Deselected();
    }

    public virtual void Hover()
    {
        IsHovering = true;

        if (OnHover != null)
        {
            OnHover();
        }
    }

    public virtual void Out()
    {
        IsHovering = false;
        m_LongPressValid = false;
        if (OnOut != null)
        {
            OnOut();
        }
    }

    public virtual void Click()
    {
        if (OnClick != null)
        {
            OnClick();
        }
    }

    public virtual void Up()
    {
        m_LongPressValid = false;
        if (OnUp != null)
        {
            OnUp();
        }
    }

    public virtual void Down()
    {
        m_PressDownTime = Time.unscaledTime;
        m_LongPressValid = true;
        if (OnDown != null)
        {
            OnDown();
        }
    }

    public virtual void LongPress()
    {
        OnLongPress?.Invoke();
    }

    public void TriggerAutoSelect()
    {
        if (GetComponent<MoveablePhysicsObject>())
        {
            NRPointerRaycaster raycaster = NREventCenter.GetRaycaster();
            if (NRInput.RaycastMode == RaycastModeEnum.Laser && raycaster)
                Selected(raycaster);
        }
    }

    public void Selected(NRPointerRaycaster raycaster)
    {
        if (raycaster == null)
            return;
        SelectingRaycaster = raycaster;
        if (OnSelected != null)
            OnSelected();
        SelectingRaycaster.enabled = false;
        NRInput.ReticleVisualActive = false;
        forceDeselectOnce = false;
    }

    public void Deselected()
    {
        if(IsBeingSelected)
        {
            if (OnDeselected != null)
                OnDeselected();
            m_DeselectFrame = Time.frameCount;
            if(SelectingRaycaster != null)
            {
                SelectingRaycaster.enabled = true;
            }
            SelectingRaycaster = null;
            NRInput.ReticleVisualActive = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.unscaledTime > (m_PressDownTime + ClickInterval))
            return;
        Click();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Out();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Down();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Up();
        JudgePickup(eventData);
    }

    private float m_TriggerDownTime;
    private int m_DeselectFrame;
    private Vector2 m_TouchDownPos;
    private bool m_TouchMoved = false;

    private const float JUDGE_TOUCH_MOVED_DISTANCE = 0.15f;

    private void JudgeDrop()
    {
        if (!IsMoveable)
            return;
        if (!IsBeingSelected)
            return;
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            m_TriggerDownTime = Time.time;
            m_TouchDownPos = NRInput.GetTouch();
        }
        if (NRInput.GetButton(ControllerButton.TRIGGER) && !m_TouchMoved)
            m_TouchMoved = Vector2.Distance(NRInput.GetTouch(), m_TouchDownPos) > JUDGE_TOUCH_MOVED_DISTANCE;
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            if ((Time.time - m_TriggerDownTime) < NRInput.ClickInterval)
                forceDeselectOnce = true;
            if ((NRInput.GetControllerType() == ControllerType.CONTROLLER_TYPE_PHONE && m_TouchMoved))
                forceDeselectOnce = false;
            m_TouchMoved = false;
        }
        else if (NRInput.GetButtonUp(ControllerButton.HOME))
        {
            forceDeselectOnce = true;
        }
        if (forceDeselectOnce)
        {
            forceDeselectOnce = false;
            Deselected();
        }
    }

    private void JudgePickup(PointerEventData eventData)
    {
        if (!IsMoveable)
            return;
        if (IsBeingSelected)
            return;
        if (Time.frameCount == m_DeselectFrame)
            return;
        if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
            return;
        if (eventData is NRPointerEventData && eventData.pointerCurrentRaycast.gameObject == this.gameObject)
            Selected((eventData as NRPointerEventData).raycaster);
    }

    private void CheckLongPress()
    {
        if (Time.unscaledTime - m_PressDownTime > LongPressInterval)
        {
            m_LongPressValid = false;
            LongPress();
        }
    }
}