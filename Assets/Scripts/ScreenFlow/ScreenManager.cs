using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unicar.ScreenFlow
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField, Min(0)] private int initialScreenIndex;

        private int _activeScreenIndex = -1;
        private static ScreenManager screenManager;

        private void Awake()
        {
            screenManager = this;

            if (SceneManager.sceneCount == 1)
                LoadScreen(initialScreenIndex);
            else
                _activeScreenIndex = SceneManager.GetActiveScene().buildIndex;
        }

        public static void LoadRequestedScreen(int index)
        {
            screenManager.LoadScreen(index);
        }

        private void LoadScreen(int index)
        {
            if (_activeScreenIndex != -1)
                SceneManager.UnloadSceneAsync(_activeScreenIndex);
            
            SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            _activeScreenIndex = index;
        }
    }
}