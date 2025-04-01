using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Cameras
{
    public class CamerasManager : MonoSingleton<CamerasManager>
    {
        [Serializable]
        private class CameraSwitcher
        {
            [SerializeField] private CinemachineCamera cameraOne;
            [SerializeField] private CinemachineCamera cameraTwo;
            [SerializeField] private TriggerChecker trigger;
            
            public TriggerChecker Trigger => trigger;

            public void SwitchCameras()
            {
                cameraOne.gameObject.SetActive(!cameraOne.gameObject.activeSelf);
                cameraTwo.gameObject.SetActive(!cameraTwo.gameObject.activeSelf);
            }
        }
        
        [SerializeField] CinemachineBrain cinemachineBrain;
        [SerializeField] private CameraSwitcher[] cameraSwitchers;
        [SerializeField] private CinemachineCamera firstPlatformCamera;
        [SerializeField] private CinemachineCamera[] otherPlatformCameras;

        readonly WaitForSeconds _waitForCameraChange = new WaitForSeconds(1f);

        private void OnEnable()
        {
            GameEvents.Instance().OnSwitchCameras += SwitchCameras;
            GameEvents.Instance().OnPlayerLostLife += ResetCameras;
        }

        private void OnDisable()
        {
            GameEvents.Instance().OnSwitchCameras -= SwitchCameras;
            GameEvents.Instance().OnPlayerLostLife += ResetCameras;
        }

        private void SwitchCameras(Transform target, float duration)
        {
            foreach (var cameraSwitcher in cameraSwitchers)
            {
                if (cameraSwitcher.Trigger.IsTriggered)
                {
                    cameraSwitcher.SwitchCameras();
                    Debug.Log("Switch Cameras");
                }
            }
        }

        private void ResetCameras()
        {
            StartCoroutine(ResetCamerasLogic());
        }
        
        private IEnumerator ResetCamerasLogic()
        {
            if (cinemachineBrain != null)
            {
                var originalBlend = cinemachineBrain.DefaultBlend;

                var cutBlend = new CinemachineBlendDefinition();
                cutBlend.Style = CinemachineBlendDefinition.Styles.Cut;
                cutBlend.Time = 0f;
                cinemachineBrain.DefaultBlend = cutBlend;

                firstPlatformCamera.gameObject.SetActive(true);

                foreach (var platformCamera in otherPlatformCameras)
                {
                    platformCamera.gameObject.SetActive(false);
                }

                yield return _waitForCameraChange;
                cinemachineBrain.DefaultBlend = originalBlend;
            }
        }
    }
}
