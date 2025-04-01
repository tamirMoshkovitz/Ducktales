using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Screens
{
    public class ScreensInputListener : MonoBehaviour
    {
        [SerializeField] private InputAction playButton;
        [SerializeField] private InputAction quitButton;

        private void OnEnable()
        {
            playButton.Enable();
            quitButton.Enable();
        }
        
        private void OnDisable()
        {
            playButton.Disable();
            quitButton.Disable();
        }

        private void Update()
        {
            if (playButton.IsPressed())
            {
                SceneManager.LoadScene("The Amazon");
            }

            if (quitButton.IsPressed())
            {
                QuitGame();
            }
        }
    
        public void QuitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor
            // so we use this instead
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Close the game!
        Application.Quit();
#endif
        }
    }
}
