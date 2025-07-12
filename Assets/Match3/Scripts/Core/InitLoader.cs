using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.Core
{
    public class InitLoader: MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    } 
}

