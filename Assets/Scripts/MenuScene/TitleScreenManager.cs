using UnityEngine;
using Unity.Netcode;

namespace SG //命名空间：组织代码，防止命名冲突，命名：SG代表项目名称
{
    public class TitleScreenManager : MonoBehaviour
    {
        public void StartNetworkAsHost()
        {
            // Code to start the network as host
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            StartCoroutine(WorldSaveGameManager.Instance.LoadNewGame());
            //这里为什么能调用WOrldSaveGameManager的协程方法？
            //因为WorldSaveGameManager是单例模式，可以通过Instance访问其公共方法
        }

    }
}
