using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Unicar.Editor 
{
    [InitializeOnLoad]
    public static class Startup 
    {
        static Startup() 
        {
            EditorSceneManager.sceneOpened += LoadGameManager;
        }

        private static void LoadGameManager(Scene scene, OpenSceneMode mode) {

            if (mode == OpenSceneMode.Single && scene.buildIndex != 0) 
            {
                OpenSceneMode openSceneMode = scene.buildIndex == -1 ? OpenSceneMode.AdditiveWithoutLoading : OpenSceneMode.Additive;
                EditorSceneManager.OpenScene("Assets/Scenes/ScreenManager.unity", openSceneMode);
                SceneManager.SetActiveScene(scene);
            }
        }
    }
}