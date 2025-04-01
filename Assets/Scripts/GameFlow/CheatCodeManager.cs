using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utilities;

namespace GameFlow
{
    public class CheatCodeManager : MonoBehaviour
    {
        [SerializeField] private InputAction resetLevel;
        [SerializeField] private InputAction resetGame;
        [SerializeField] private InputAction resetEnemies;
        [SerializeField] private InputAction resetPlayer;


        private void OnEnable()
        {
            resetLevel.Enable();
            resetGame.Enable();
            resetEnemies.Enable();
            resetPlayer.Enable();
        }
        
        private void OnDisable()
        {
            resetLevel.Disable();
            resetGame.Disable();
            resetEnemies.Disable();
            resetPlayer.Disable();
        }
        
        private void Update()
        {
            if (resetLevel.IsPressed())
                SceneManager.LoadScene("The Amazon");
            

            if (resetGame.IsPressed())
                SceneManager.LoadScene("Opening scene");

            if (resetEnemies.IsPressed())
                GameEvents.Instance().OnResetAllEnemies?.Invoke();
            if (resetPlayer.IsPressed())
                GameEvents.Instance().OnResetPlayer?.Invoke();
        }
    }
}
