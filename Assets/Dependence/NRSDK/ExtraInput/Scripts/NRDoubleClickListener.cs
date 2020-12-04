using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NRKernal
{
    public class NRDoubleClickListener
    {
        private ControllerHandEnum m_HandEnum;
        private ControllerButton m_ControllerButton;
        private float m_DoubleClickInterval;
        private Action m_DoubleClickCallback;
        private float m_ClickCount;
        private float m_LastClickTime;

        public NRDoubleClickListener(ControllerHandEnum controllerHandEnum, ControllerButton controllerButton, float doubleClickInterval, Action callback)
        {
            m_HandEnum = controllerHandEnum;
            m_ControllerButton = controllerButton;
            m_DoubleClickInterval = Mathf.Max(doubleClickInterval, 0.1f);
            m_DoubleClickCallback = callback;
            NRInput.AddClickListener(m_HandEnum, m_ControllerButton, OnButtonClick);
        }

        private void OnButtonClick()
        {
            if(m_ClickCount == 0)
            {
                m_ClickCount++;
                m_LastClickTime = Time.unscaledTime;
            }
            else if(m_ClickCount == 1)
            {
                if(Time.unscaledTime - m_LastClickTime < m_DoubleClickInterval)
                {
                    m_DoubleClickCallback?.Invoke();
                }
                m_ClickCount = 0;
                m_LastClickTime = 0f;
            }
        }

        public void Destroy()
        {
            NRInput.RemoveClickListener(m_HandEnum, m_ControllerButton, OnButtonClick);
            m_DoubleClickCallback = null;
        }
    }
}
