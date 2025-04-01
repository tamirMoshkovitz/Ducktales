using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace GameFlow
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void OnEnable()
        {
            GameEvents.Instance().OnFinishLevel += CutToEndScene;
            GameEvents.Instance().OnPlayerDeath += CutToEndScene;
        }
        
        private void OnDisable()
        {
            GameEvents.Instance().OnFinishLevel -= CutToEndScene;
            GameEvents.Instance().OnPlayerDeath -= CutToEndScene;
        }

        private void CutToEndScene()
        {
            StartCoroutine(EndGame());
        }
        
        private IEnumerator EndGame()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("Ending scene");
        }
    }
}