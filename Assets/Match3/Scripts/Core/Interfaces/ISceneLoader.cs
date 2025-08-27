using Cysharp.Threading.Tasks;

namespace Match3.Scripts.Core.Interfaces
{
    public interface ISceneLoader : IService
    {
        UniTask LoadSceneByIndexAsync(int sceneIndex);
        //UniTask LoadSceneByNameAsync(string sceneName);
    }
}