﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This class obtains the runtime information of controller devices through a native controller plugin, and would use these info to update controller states.
    /// </summary>
    internal partial class NRControllerProvider : ControllerProviderBase
    {
        private NativeController m_NativeController;
        private int m_ProcessedFrame;
        private bool m_NeedInit = true;
        private bool m_NeedRecenter;
        private float[] homePressingTimerArr = new float[NRInput.MAX_CONTROLLER_STATE_COUNT];

        private const float HOME_LONG_PRESS_TIME = 1.1f;

        public NRControllerProvider(ControllerState[] states) : base(states)
        {

        }

        public override int ControllerCount
        {
            get
            {
                if (!Inited)
                    return 0;
                return m_NativeController.GetControllerCount();
            }
        }

        public string GetVersion(int index)
        {
            if (m_NativeController != null)
            {
                return m_NativeController.GetVersion(index);
            }
            return string.Empty;
        }

        public override void OnPause()
        {
            if (m_NativeController != null)
            {
                m_NativeController.Pause();
            }
        }

        public override void OnResume()
        {
            if (m_NativeController != null)
            {
                m_NativeController.Resume();
            }
        }

        public override void Update()
        {
            if (m_ProcessedFrame == Time.frameCount)
                return;
            m_ProcessedFrame = Time.frameCount;
            if (m_NeedInit)
            {
                InitNativeController();
                return;
            }
            if (!Inited)
                return;
            int availableCount = ControllerCount;
            if (availableCount > 0 && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION))
            {
                UpdateHeadPoseToController();
            }
            for (int i = 0; i < availableCount; i++)
            {
                UpdateControllerState(i);
            }
        }

        public override void OnDestroy()
        {
            if (m_NativeController != null)
            {
                m_NativeController.Destroy();
                m_NativeController = null;
            }
        }

        public override void TriggerHapticVibration(int index, float durationSeconds = 0.1f, float frequency = 1000f, float amplitude = 0.5f)
        {
            if (!Inited)
                return;
            if (states[index].controllerType == ControllerType.CONTROLLER_TYPE_PHONE)
            {
                PhoneVibrateTool.TriggerVibrate(durationSeconds);
            }
            else
            {
                if (m_NativeController != null && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE))
                {
                    Int64 durationNano = (Int64)(durationSeconds * 1000000000);
                    m_NativeController.TriggerHapticVibrate(index, durationNano, frequency, amplitude);
                }
            }
        }

        public override void Recenter()
        {
            base.Recenter();
            m_NeedRecenter = true;
        }

        private void InitNativeController()
        {
            m_NativeController = new NativeController();
            if (m_NativeController.Init())
            {
                Inited = true;
                NRDebugger.Log("NRControllerProvider Init Succeed");
            }
            else
            {
                m_NativeController = null;
                NRDebugger.LogError("NRControllerProvider Init Failed !!");
            }

#if !UNITY_EDITOR
            NRDebugger.Log("[NRInput] version:" + GetVersion(0));
#endif
            m_NeedInit = false;
        }

        private void UpdateControllerState(int index)
        {
            m_NativeController.UpdateState(index);
            states[index].controllerType = m_NativeController.GetControllerType(index);
#if UNITY_EDITOR
            if (NRInput.EmulateVirtualDisplayInEditor)
            {
                states[index].controllerType = ControllerType.CONTROLLER_TYPE_PHONE;
            }
#endif
            states[index].availableFeature = (ControllerAvailableFeature)m_NativeController.GetAvailableFeatures(index);
            states[index].connectionState = m_NativeController.GetConnectionState(index);
            states[index].rotation = m_NativeController.GetPose(index).rotation;
            states[index].position = m_NativeController.GetPose(index).position;
            states[index].gyro = m_NativeController.GetGyro(index);
            states[index].accel = m_NativeController.GetAccel(index);
            states[index].mag = m_NativeController.GetMag(index);
            states[index].touchPos = m_NativeController.GetTouch(index);
            states[index].isTouching = m_NativeController.IsTouching(index);
            states[index].recentered = false;
            states[index].isCharging = m_NativeController.IsCharging(index);
            states[index].batteryLevel = m_NativeController.GetBatteryLevel(index);
            states[index].buttonsState = (ControllerButton)m_NativeController.GetButtonState(index);
            states[index].buttonsDown = (ControllerButton)m_NativeController.GetButtonDown(index);
            states[index].buttonsUp = (ControllerButton)m_NativeController.GetButtonUp(index);

            IControllerStateParser stateParser = ControllerStateParseUtility.GetControllerStateParser(states[index].controllerType, index);
            if (stateParser != null)
            {
                stateParser.ParserControllerState(states[index]);
            }

            CheckRecenter(index);

            if (m_NeedRecenter)
            {
                for (int i = 0; i < ControllerCount; i++)
                {
                    states[i].recentered = true;
                    m_NativeController.RecenterController(i);
                }
                m_NeedRecenter = false;
            }
        }

        private void CheckRecenter(int index)
        {
            if (states[index].GetButton(ControllerButton.APP))
            {
                homePressingTimerArr[index] += Time.deltaTime;
                if (homePressingTimerArr[index] > HOME_LONG_PRESS_TIME)
                {
                    homePressingTimerArr[index] = float.MinValue;
                    Recenter();
                }
            }
            else
            {
                homePressingTimerArr[index] = 0f;
            }
        }

        private void UpdateHeadPoseToController()
        {
            if (m_NativeController != null && NRInput.CameraCenter)
            {
                m_NativeController.UpdateHeadPose(new Pose(NRInput.CameraCenter.position, NRInput.CameraCenter.rotation));
            }
        }
    }
}