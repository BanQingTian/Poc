using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NREAL.AR.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public enum TargetType { CAMERA, TRANSFORM, }

        public TargetType targetType = TargetType.CAMERA;

        public Transform targetTransform;

        private Transform NRCameraRig;

        private Transform target;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            NRCameraRig = GameObject.Find("NRCameraRig").transform;
            FindTarget();
        }

        private void FindTarget()
        {
            switch (targetType)
            {
                case TargetType.CAMERA:
                    target = NRCameraRig;
                    break;
                case TargetType.TRANSFORM:
                    target = targetTransform;
                    break;
            }
        }

        void LateUpdate()
        {
            if (target != null)
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
				Debug.Log("position = " + target.position.ToString("F4"));
            }
            else
            {
                FindTarget();
            }
        }
    }
}
