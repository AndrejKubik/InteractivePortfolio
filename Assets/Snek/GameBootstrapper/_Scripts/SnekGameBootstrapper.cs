using System.IO;
using Snek.Utilities;
using UnityEngine.SceneManagement;

namespace Snek.GameBootstrapper
{
    [UseSnekInspector]
    public class SnekGameBootstrapper : SnekMonoBehaviour
    {
        public string StartSceneName;

        protected override void Validate()
        {
            if (string.IsNullOrEmpty(StartSceneName))
                FailValidation("No starting scene assigned, aborting launch.");
            else if(!IsStartSceneInBuild())
                FailValidation("Assigned start scene is missing or not enabled in build settings");

            if (StartSceneName == SceneManager.GetActiveScene().name)
                FailValidation("Cannot use bootstrap scene as starting scene, aborting launch.");
        }

        protected override void OnInitializationSuccess()
        {
            SceneManager.LoadScene(StartSceneName);
        }

        private bool IsStartSceneInBuild()
        {
            int count = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < count; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = Path.GetFileNameWithoutExtension(path);

                if (name == StartSceneName)
                    return true;
            }

            return false;
        }
    }
}
