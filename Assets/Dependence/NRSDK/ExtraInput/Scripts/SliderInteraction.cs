using System;
using UnityEngine;

namespace NRKernal
{
    public class SliderInteraction : MonoBehaviour
    {

        private float m_TouchDownTime;
        private Vector2 m_TouchDownPos;
        private Vector2 m_TouchCurrentPos;
        private const float MIN_SWIPE_INTERVAL = 0.6f;
        private const float MIN_SWIPE_DISTANCE = 0.02f;

        private bool m_IsSliderDown = false;
        public bool IsSliderDown => m_IsSliderDown;
        private bool m_IsSliderLeft = false;
        public bool IsSliderLeft => m_IsSliderLeft;
        private bool m_IsSliderRight = false;
        public bool IsSliderRight => m_IsSliderRight;

        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                GameObject target = NREventCenter.GetCurrentRaycastTarget();
                // if (target != <slider 对象>)
                // {
                    m_IsSliderDown = true;
                // }

                m_TouchDownTime = Time.unscaledTime;
                m_TouchDownPos = NRInput.GetTouch();
            }
            if (NRInput.GetButton(ControllerButton.TRIGGER))
            {
                if (m_IsSliderDown)
                {
                    m_TouchCurrentPos = NRInput.GetTouch();

                    var deltaMoveX = m_TouchCurrentPos.x - m_TouchDownPos.x;
                    var deltaMoveY = m_TouchCurrentPos.y - m_TouchDownPos.y;
                    var deltaDistanceX = Mathf.Abs(deltaMoveX);
                    var deltaDistanceY = Mathf.Abs(deltaMoveY);
                    
                    if (deltaDistanceX > deltaDistanceY && deltaDistanceX > MIN_SWIPE_DISTANCE)
                    {
                        if (deltaMoveX > 0)
                        {
                            m_IsSliderRight = true;
                            m_IsSliderLeft = false;
                        }
                        else
                        {
                            m_IsSliderRight = false;
                            m_IsSliderLeft = true;
                        }
                    }
                    else
                    {
                        m_IsSliderRight = false;
                        m_IsSliderLeft = false;
                    }

                    // if (m_IsSliderLeft)
                    // {
                    //     Debug.Log("slider left");
                    // }
                    // if (m_IsSliderRight)
                    // {
                    //     Debug.Log("slider right");
                    // }

                    m_TouchCurrentPos = NRInput.GetTouch();
                }
            }
            else if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
            {
                m_IsSliderDown = false;
            }
        }
    }
}
