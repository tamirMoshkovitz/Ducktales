using System;
using UnityEngine;

namespace Utilities
{
    public class GameEvents
    {
        private static GameEvents _instance;

        public static GameEvents Instance()
        {
            if (_instance == null)
                _instance = new GameEvents();
            return _instance;
        }
        
        public Action OnMovementDataChanged;
        public Action OnPlayerDeath;
        public Action OnPlayerHurt;
        public Action OnPlayerLostLife;
        public Action OnEarthquakeStarted;
        public Action OnEarthquakeFinished;
        public Action OnEnterBossRoomStarted;
        public Action OnEnterBossRoomFinished;
        public Action OnPuttSuccess;
        public Action OnPuttFailed;
        public Action OnBounceHit;
        public Action OnLandOnGround;
        public Action OnFinishLevel;
        public Action OnClimbing;
        public Action OnStopedClimbing;
        public Action OnResetAllEnemies;
        public Action OnResetPlayer;
        public Action<Transform, float> OnSwitchCameras;
    }
}