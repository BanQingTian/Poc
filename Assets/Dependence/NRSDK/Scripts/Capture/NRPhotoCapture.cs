﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;

    public class NRPhotoCapture : IDisposable
    {
        private static IEnumerable<Resolution> m_SupportedResolutions;

        /// <summary>
        /// A list of all the supported device resolutions for taking pictures.
        /// </summary>
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                if (m_SupportedResolutions == null)
                {
                    var resolutions = new List<Resolution>();
                    var resolution = new Resolution();
                    resolution.width = NRRgbCamera.Resolution.width;
                    resolution.height = NRRgbCamera.Resolution.height;
                    resolutions.Add(resolution);
                    m_SupportedResolutions = resolutions;
                }
                return m_SupportedResolutions;
            }
        }

        private FrameCaptureContext m_CaptureContext;

        public Texture PreviewTexture
        {
            get
            {
                return m_CaptureContext?.PreviewTexture;
            }
        }

        public static void CreateAsync(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback)
        {
            NRPhotoCapture photocapture = new NRPhotoCapture();
            photocapture.m_CaptureContext = FrameCaptureContextFactory.Create();
            onCreatedCallback?.Invoke(photocapture);
        }

        /// <summary>
        /// Dispose must be called to shutdown the PhotoCapture instance.
        /// </summary>
        public void Dispose()
        {
            if (m_CaptureContext != null)
            {
                m_CaptureContext.Release();
                m_CaptureContext = null;
            }
        }

        /// <summary>
        /// Provides a COM pointer to the native IVideoDeviceController.
        /// A native COM pointer to the IVideoDeviceController.
        /// </summary>
        public IntPtr GetUnsafePointerToVideoDeviceController()
        {
            NRDebugger.LogWarning("[NRPhotoCapture] Interface not supported...");
            return IntPtr.Zero;
        }

        public void StartPhotoModeAsync(CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback)
        {
            PhotoCaptureResult result = new PhotoCaptureResult();
            try
            {
                setupParams.camMode = CamMode.PhotoMode;
                m_CaptureContext.StartCaptureMode(setupParams);
                m_CaptureContext.StartCapture();

                NRKernalUpdater.Instance.StartCoroutine(OnPhotoModeStartedReady(() =>
                {
                    result.resultType = CaptureResultType.Success;
                    onPhotoModeStartedCallback?.Invoke(result);
                }));
            }
            catch (Exception)
            {
                result.resultType = CaptureResultType.UnknownError;
                onPhotoModeStartedCallback?.Invoke(result);
                throw;
            }
        }

        public System.Collections.IEnumerator OnPhotoModeStartedReady(Action callback)
        {
            while (!this.m_CaptureContext.GetFrameProvider().IsFrameReady())
            {
                NRDebugger.LogFormat("Wait for the frame ready!");
                yield return new WaitForEndOfFrame();
            }
            callback?.Invoke();
        }

        public void StopPhotoModeAsync(OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
        {
            PhotoCaptureResult result = new PhotoCaptureResult();
            try
            {
                m_CaptureContext.StopCaptureMode();
                result.resultType = CaptureResultType.Success;
                onPhotoModeStoppedCallback?.Invoke(result);
            }
            catch (Exception)
            {
                result.resultType = CaptureResultType.UnknownError;
                onPhotoModeStoppedCallback?.Invoke(result);
                throw;
            }
        }

        public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
        {
            try
            {
                var capture = m_CaptureContext.GetBehaviour();
                ((NRCaptureBehaviour)capture).Do(filename, fileOutputFormat);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void TakePhotoAsync(OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
        {
            try
            {
                var capture = m_CaptureContext.GetBehaviour();
                ((NRCaptureBehaviour)capture).DoAsyn(onCapturedPhotoToMemoryCallback);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Contains the result of the capture request.
        /// </summary>
        public enum CaptureResultType
        {
            /// <summary>
            /// Specifies that the desired operation was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Specifies that an unknown error occurred.
            /// </summary>            
            UnknownError = 1
        }

        /// <summary>
        /// A data container that contains the result information of a photo capture operation.
        /// </summary>
        public struct PhotoCaptureResult
        {
            /// <summary>
            /// A generic result that indicates whether or not the PhotoCapture operation succeeded.
            /// </summary>
            public CaptureResultType resultType;

            /// <summary>
            /// The specific HResult value.
            /// </summary>
            public long hResult;

            /// <summary>
            /// Indicates whether or not the operation was successful.
            /// </summary>
            public bool success { get; }
        }

        /// <summary>
        ///  Called when a PhotoCapture resource has been created.
        /// </summary>
        /// <param name="captureObject">The PhotoCapture instance.</param>
        public delegate void OnCaptureResourceCreatedCallback(NRPhotoCapture captureObject);

        /// <summary>
        /// Called when photo mode has been started.
        /// </summary>
        /// <param name="result">Indicates whether or not photo mode was successfully activated.</param>
        public delegate void OnPhotoModeStartedCallback(PhotoCaptureResult result);

        /// <summary>
        /// Called when photo mode has been stopped.
        /// </summary>
        /// <param name="result">Indicates whether or not photo mode was successfully deactivated.</param>
        public delegate void OnPhotoModeStoppedCallback(PhotoCaptureResult result);

        /// <summary>
        /// Called when a photo has been saved to the file system.
        /// </summary>
        /// <param name="result">Indicates whether or not the photo was successfully saved to the file system.</param>
        public delegate void OnCapturedToDiskCallback(PhotoCaptureResult result);

        /// <summary>
        /// Called when a photo has been captured to memory.
        /// </summary>
        /// <param name="result">Indicates whether or not the photo was successfully captured to memory.</param>
        /// <param name="photoCaptureFrame">Contains the target texture.If available, the spatial information will be accessible through this structure as well.</param>
        public delegate void OnCapturedToMemoryCallback(PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);
    }
}
