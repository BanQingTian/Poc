using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using NRKernal;
using NRKernal.Record;

namespace NREAL.HOME.TAKEPICTURE
{
    public class TakePictureBehaviour : MonoBehaviour
    {
        public delegate void OnCapturedPhotoToMemoryDelegate(Texture2D texture);

        NRPhotoCapture photoCaptureObject = null;
        Texture2D targetTexture = null;

        Resolution cameraResolution;

        public bool IsOpen { get { return photoCaptureObject != null; } }
        public OnCapturedPhotoToMemoryDelegate onCapturedPhotoToMemoryDelegate;

        // Use this for initialization
        public void Open()
        {
            if (photoCaptureObject != null)
            {
                Debug.LogError("The NRPhotoCapture has already been created.");
                return;
            }

            Debug.Log("take photo: start to create photoCaptureObject");
            // Create a PhotoCapture object
            NRPhotoCapture.CreateAsync(false, delegate (NRPhotoCapture captureObject)
            {
                cameraResolution = NRPhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

                if (captureObject != null)
                {
                    photoCaptureObject = captureObject;
                }
                else
                {
                    Debug.LogError("Can not get a captureObject.");
                }

                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
                cameraParameters.blendMode = BlendMode.Blend;

                Debug.Log("take photo: start to activate rgb camera");
                // Activate the camera
                photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (NRPhotoCapture.PhotoCaptureResult result)
                {
                    Debug.Log("take photo: Start PhotoMode Async");

                    //var captureBehaviour = GameObject.FindObjectOfType<NRCaptureBehaviour>();
                    //captureBehaviour.CaptureCamera.gameObject.GetComponent<NRCameraInitializer>().SwitchToEyeParam(NativeEye.LEFT);
                });
            });
        }

        public void TakeAPhoto()
        {
            Debug.Log("take photo: start to take photo.");

            if (photoCaptureObject == null)
            {
                Debug.LogError("The NRPhotoCapture has not been created.");
                return;
            }

            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }

        void OnCapturedPhotoToMemory(NRPhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            Debug.Log("take photo: C");
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            onCapturedPhotoToMemoryDelegate?.Invoke(targetTexture);

            targetTexture = null;
        }

        public void Close()
        {
            if (photoCaptureObject == null)
            {
                Debug.LogError("The NRPhotoCapture has not been created.");
                return;
            }
            // Deactivate our camera
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        void OnStoppedPhotoMode(NRPhotoCapture.PhotoCaptureResult result)
        {
            if (photoCaptureObject == null)
            {
                Debug.LogError("The NRPhotoCapture has not been created.");
                return;
            }
            // Shutdown our photo capture resource
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }
    }
}