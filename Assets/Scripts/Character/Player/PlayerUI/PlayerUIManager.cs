using Unity.Netcode;
using UnityEngine;

namespace SG
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager Instance;

        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;

        public void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if(startGameAsClient)
            {
                startGameAsClient = false;
                //必须先关闭当前的网络管理器，因为在title screen中已经以host模式启动了网络管理器
                NetworkManager.Singleton.Shutdown();
                //重新启动网络管理器，以客户端模式加入游戏
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
