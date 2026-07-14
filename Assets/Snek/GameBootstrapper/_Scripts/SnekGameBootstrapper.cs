using System.Collections.Generic;
using System.IO;
using Snek.Utilities;
using UnityEngine.SceneManagement;

namespace Snek.GameBootstrapper
{
    [UseSnekInspector]
    public class SnekGameBootstrapper : SnekMonoBehaviour
    {
        public string StartSceneName;
        public List<SnekMonoBehaviour> PreLaunchServices = new();

        protected override void Validate()
        {
            if (string.IsNullOrEmpty(StartSceneName))
                FailValidation("No starting scene assigned, aborting launch.");
            else if (!IsStartSceneInBuild())
                FailValidation("Assigned start scene is missing or not enabled in build settings");

            if (StartSceneName == SceneManager.GetActiveScene().name)
                FailValidation("Cannot use bootstrap scene as starting scene, aborting launch.");

            if (!IsEveryPreLaunchServiceValid())
                FailValidation("Null or missing references present in Pre-Launch Services list.");
        }

        private bool IsEveryPreLaunchServiceValid()
        {
            foreach (SnekMonoBehaviour service in PreLaunchServices)
                if (service == null)
                    return false;

            return true;
        }

        protected override void OnInitializationSuccess()
        {
            InitializePreLaunchServices();

            SceneManager.LoadScene(StartSceneName);
        }

        private void InitializePreLaunchServices()
        {
            foreach (SnekMonoBehaviour service in PreLaunchServices)
            {
                SnekMonoBehaviour serviceInstance = Instantiate(service);
                serviceInstance.name = service.name;
            }
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
