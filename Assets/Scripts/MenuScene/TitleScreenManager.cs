using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace SG //命名空间：组织代码，防止命名冲突，命名：SG代表项目名称
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance; //单例实例
        [Header("Menus")]
        [SerializeField] GameObject titleScreenMainMenu; //标题屏幕主菜单对象
        [SerializeField] GameObject titleScreenLoadMenu; //标题屏幕加载菜单对象

        [Header("Buttons")]
        [SerializeField] Button mainMenuNewGameButton; //新游戏按钮对象
        [SerializeField] Button loadMenuReturnButton; //返回按钮对象
        [SerializeField] Button mainMenuLoadGameButton; //主菜单加载游戏按钮对象

        [Header("Pop Ups")]
        [SerializeField] GameObject noFreeSlotsPopup; //没有空余槽位的弹出窗口对象
        [SerializeField] Button noFreeSlotsOkayButton; //没有空余槽位弹出窗口的确认按钮对象

        private void Awake()
        {
            if (instance == null)
            {
                instance = this; //设置单例实例
            }
            else
            {
                Destroy(gameObject); //如果已经存在实例，销毁当前对象，确保只有一个实例存在
            }
        }

        public void StartNetworkAsHost()
        {
            // Code to start the network as host
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.Instance.AttemptToCreateNewGame(); //创建一个新的游戏
            //这里为什么能调用WOrldSaveGameManager的协程方法？
            //因为WorldSaveGameManager是单例模式，可以通过Instance访问其公共方法
        }


        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false); //隐藏主菜单
            titleScreenLoadMenu.SetActive(true); //显示加载菜单

            //自动选择返回按键
            loadMenuReturnButton.Select();

        }

        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false); //隐藏加载菜单
            titleScreenMainMenu.SetActive(true); //显示主菜单

            //自动选择加载游戏按键
            mainMenuLoadGameButton.Select();
        }

        public void DisplayNoFreeSlotsPopup()
        {
            noFreeSlotsPopup.SetActive(true); //显示没有空余槽位的弹出窗口
            noFreeSlotsOkayButton.Select(); //自动选择确认按钮
        }

        public void CloseNoFreeSlotsPopup()
        {
            noFreeSlotsPopup.SetActive(false); //隐藏没有空余槽位的弹出窗口
            mainMenuNewGameButton.Select(); //自动选择新游戏按键
        }
    }
}
