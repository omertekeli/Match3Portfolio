using Cysharp.Threading.Tasks;
using Match3.Scripts.Core.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.Core
{
    public class SceneLoader : ISceneLoader
    {
        public SceneLoader() { }

        public async UniTask LoadSceneByIndexAsync(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"SceneLoader Error: Scene index {sceneIndex} is not valid or not in Build Settings.");
                return;
            }
            await SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single).ToUniTask();
            Debug.Log($"SceneLoader: Scene with build index {sceneIndex} loaded asynchronously.");
        }
    }
}