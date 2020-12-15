using System.Collections;
using System.IO;
using UnityEngine;
using NRKernal;
using TMPro;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace NREAL.HOME.TAKEPICTURE
{
    public class TakePictureView : MonoBehaviour
    {
        public GameObject number;
        public MeshRenderer photoRenderer;

        public GameObject toast;

        private TakePictureBehaviour m_TakePictureBehaviour;

        private Vector3 m_CachedPhotoScale;

        private bool m_IsPlaying = false;

        private AudioSource m_KacaSfxSource;

        private Transform m_Corners;

        private bool m_TempLaserVisibleState;

        private bool m_TempReticleVisibleState;

        private void Start()
        {
            Debug.Log("TakePictureView start.");
            m_TakePictureBehaviour = GetComponent<TakePictureBehaviour>();
            m_TakePictureBehaviour.onCapturedPhotoToMemoryDelegate += OnCapturedPhotoToMemory;

            m_KacaSfxSource = GetComponent<AudioSource>();

            number.SetActive(false);
            toast.SetActive(false);
            photoRenderer.gameObject.SetActive(false);
            m_CachedPhotoScale = photoRenderer.transform.localScale;

            m_Corners = transform.Find("Corners");
            m_Corners.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventCenter.Instance.AddEventListener(ZConstant.Event__Capture__, TakingPhoto);
        }

        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener(ZConstant.Event__Capture__, TakingPhoto);
        }

        private void TakingPhoto(object sender, EventCenter.Args args)
        {
            Debug.Log("TakePhoto: take photo event triggered");
            if (!m_IsPlaying)
            {
                m_IsPlaying = true;

                m_Corners.gameObject.SetActive(true);

#if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Debug.Log("TakePhoto: request camera permission");
                    Permission.RequestUserPermission(Permission.Camera);
                }
#endif

				StartCoroutine(CoCapturePhoto());
            }
        }

        IEnumerator CoCapturePhoto()
        {
            Debug.Log("TakePhoto: PhotoCapture object isCreated " + m_TakePictureBehaviour.IsOpen);
            if (!m_TakePictureBehaviour.IsOpen)
            {
                m_TakePictureBehaviour.Open();
            }

            Debug.Log("TakePhoto: 3 seconds countdown start");
            yield return StartCoroutine(CoCountDown());

            m_Corners.gameObject.SetActive(false);
            PauseControllerVisual();
            yield return new WaitForSeconds(0.1f);

            // Now take a photo
            m_TakePictureBehaviour.TakeAPhoto();

            //m_KacaSfxSource.Play();
            // SEManager.Instance.Play("Function_Snapshot.wav");
            //NREAL.NATIVES.NativeMediaActionSound.Play(NREAL.NATIVES.NativeMediaActionSound.SHUTTER_CLICK);
        }

        void FinishCapturePhoto()
        {
            if (m_TakePictureBehaviour.IsOpen)
            {
                m_TakePictureBehaviour.Close();
            }

            var texture = photoRenderer.material.GetTexture("_MainTex");
            photoRenderer.material.SetTexture("_MainTex", null);

            if (texture != null)
            {
                Destroy(texture);
            }
            ResumeControllerVisual();
            m_IsPlaying = false;
        }

        void OnCapturedPhotoToMemory(Texture2D targetTexture)
        {
            Debug.Log("take photo: F");
            if (targetTexture == null)
            {
                Debug.LogError("captured targetTexture is null");
                FinishCapturePhoto();
                return;
            }

            Debug.Log("captured targetTexture width:" + targetTexture.width);
            photoRenderer.gameObject.SetActive(true);
            photoRenderer.material.SetTexture("_MainTex", targetTexture);

            // resize the photo display
            //float height = targetTexture.height * targetTexture.width / 800f;
            //photoRenderer.transform.localScale = new Vector3(800f, height, 1f);

            // save photo to storage
            string guid = System.Guid.NewGuid().ToString(); ;
            string fullPath = BasePath + guid + ".png"; ;
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            targetTexture.name = guid;
            SaveTextureAsPNG(targetTexture, fullPath);
            //NREAL.NATIVES.NativeGalleryHelper.AddPictureToGallery(fullPath);

            //LeanTween.delayedCall(gameObject, 1.0f, () =>
            //{
            //    EventCenter.Instance.DispatchEvent(Constant.Event_TakePictureModule_SnapShot_Finish, this);
            //});

            PhotoDrop();
        }

        IEnumerator CoCountDown()
        {
            var text = number.GetComponent<TextMeshProUGUI>();
            number.gameObject.SetActive(true);

            for (int t = 3; t > 0; t--)
            {
                text.text = t.ToString();
                yield return new WaitForSeconds(1.0f);
            }

            number.gameObject.SetActive(false);

            // This prevents number being renderred into next frame
            yield return null;
        }

        void PauseControllerVisual()
        {
            m_TempLaserVisibleState = NRInput.LaserVisualActive;
            m_TempReticleVisibleState = NRInput.ReticleVisualActive;
        }

        void ResumeControllerVisual()
        {
            NRInput.LaserVisualActive = m_TempLaserVisibleState;
            NRInput.ReticleVisualActive = m_TempReticleVisibleState;
        }

        void PhotoDrop()
        {
            var photo = photoRenderer.transform;
            photo.localPosition = Vector3.zero;
            photo.localRotation = Quaternion.identity;
            photo.localScale = m_CachedPhotoScale;

            LeanTween.delayedCall(gameObject, 1.5f, () =>
            {
                LeanTween.scale(photo.gameObject, Vector3.zero, 1f);
                LeanTween.rotateLocal(photo.gameObject, Quaternion.Euler(0f, 0f, 90f).eulerAngles, 1f);

                var to = photo.localPosition + new Vector3(0f, -10f, 0f);
                var moveTween = LeanTween.moveLocal(photo.gameObject, to, 1f);
                moveTween.setEase(LeanTweenType.easeInQuad);

                // show toast
                // toast.SetActive(true);
                LeanTween.delayedCall(gameObject, 2f, () =>
                {
                    // toast.SetActive(false);
                    FinishCapturePhoto();
                });
            });
        }

        public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
        {
            byte[] _bytes = _texture.EncodeToPNG();
            File.WriteAllBytes(_fullPath, _bytes);
            Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
        }

        public static string BasePath
        {
            get
            {
#if UNITY_EDITOR
                string photoPath = Application.dataPath + "/../NRResource/NrealShots/";
#else
                string photoPath = NRKernal.NRTools.GetSdcardPath() + "DCIM/Camera/NrealShots/";
#endif
                photoPath = photoPath.Replace("file://", "");
                return photoPath;
            }
        }
    }
}
