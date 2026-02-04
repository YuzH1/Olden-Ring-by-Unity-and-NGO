using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{   
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance;//创建单例，原因：方便其他脚本访问此类中的方法和属性

        [SerializeField] int worldSceneIndex = 1; //场景索引，用于加载世界场景

        private void Awake()
        {
            if(Instance == null) //如果实例为空，将当前对象赋值给实例，否则销毁当前对象，确保单例模式
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
            DontDestroyOnLoad(gameObject); //确保在场景切换时不销毁此对象
        }

        public IEnumerator LoadNewGame()//协程方法，用于加载新游戏场景
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex); //异步加载新游戏场景，原因：避免卡顿

            yield return null; //等待一帧，确保加载操作开始
        }
    }
}
