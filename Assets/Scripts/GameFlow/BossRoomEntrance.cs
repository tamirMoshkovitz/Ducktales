using System;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace GameFlow
{
    public class BossRoomEntrance : MonoBehaviour
    {
        [SerializeField] private GameObject boss;
        [SerializeField] private Collider2D blockExit;

        private void OnEnable()
        {
            GameEvents.Instance().OnEnterBossRoomFinished += EnableBoss;
        }

        private void OnDisable()
        {
            GameEvents.Instance().OnEnterBossRoomFinished -= EnableBoss;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameEvents.Instance().OnEnterBossRoomStarted?.Invoke();
                Debug.Log("Enabling blockExit collider.");
                blockExit.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameEvents.Instance().OnEnterBossRoomFinished?.Invoke();
                this.gameObject.SetActive(false);
            }
        }

        private void EnableBoss()
        {
            boss.SetActive(true);
        }
    }
}