﻿using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class NRHomeMenu : MonoBehaviour
    {
        public Button confirmBtn;
        public Button cancelBtn;

        private static NRHomeMenu m_Instance;
        private static bool m_IsShowing = false;
        private static string m_MenuPrefabPath = "NRUI/NRHomeMenu";

        private Transform _MainCamera;
        public Transform mainCamera
        {
            get
            {
                if (_MainCamera == null)
                {
                    if (Camera.main != null)
                    {
                        _MainCamera = Camera.main.transform;
                    }
                    else if (NRSessionManager.Instance.NRHMDPoseTracker != null)
                    {
                        _MainCamera = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform;
                    }
                }

                return _MainCamera;
            }
        }

        void Start()
        {
            confirmBtn.onClick.AddListener(OnComfirmButtonClick);
            cancelBtn.onClick.AddListener(OnCancelButtonClick);
        }

        void Update()
        {
            if (m_IsShowing && NRInput.RaycastMode == RaycastModeEnum.Laser)
                FollowCamera();
        }

        private void OnComfirmButtonClick()
        {
            Hide();
            AppManager.QuitApplication();
        }

        private void OnCancelButtonClick()
        {
            Hide();
        }

        private void FollowCamera()
        {
            if (m_Instance && mainCamera)
            {
                m_Instance.transform.position = mainCamera.transform.position;
                m_Instance.transform.rotation = mainCamera.transform.rotation;
            }
        }

        private static void CreateMenu()
        {
            GameObject menuPrefab = Resources.Load<GameObject>(m_MenuPrefabPath);
            if (menuPrefab == null)
            {
                Debug.LogError("Can not find prefab: " + m_MenuPrefabPath);
                return;
            }
            GameObject menuGo = Instantiate(menuPrefab);
            m_Instance = menuGo.GetComponent<NRHomeMenu>();
            if (m_Instance)
                DontDestroyOnLoad(menuGo);
            else
                Destroy(menuGo);
        }

        public static void Toggle()
        {
            if (m_IsShowing)
                Hide();
            else
                Show();
        }

        public static void Show()
        {
            if (m_Instance == null)
                CreateMenu();
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(true);
                m_IsShowing = true;
                if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
                    m_Instance.FollowCamera();
            }
        }

        public static void Hide()
        {
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(false);
                m_IsShowing = false;
            }
        }
    }
}
