using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SG
{
    public class PlayerESCMenuManager : MonoBehaviour
    {
        public static PlayerESCMenuManager instance;

        PlayerControls playerControls;


        [Header("ESC Menu")]
        [SerializeField] GameObject escMenuCanvas;
        [SerializeField] Button returnToGameButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button closeGameButton;
        [SerializeField] float toggleCooldown = 0.15f;

        float lastToggleTime = -999f;


        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
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


        private void OnEnable()
        {
            if(playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.UI.KeyBoard_ESC.started += i => ToggleESCMenu();
                playerControls.UI.GamePad_Menu.started += i => ToggleESCMenu();
            }
            playerControls.Enable();//启用玩家输入控制器
        }

        private void OnDisable()
        {
            playerControls.Disable();//禁用玩家输入控制器
        }

        public void ToggleESCMenu()
        {


            if(escMenuCanvas == null)
            {
                return;
            }

            if(Time.unscaledTime - lastToggleTime < toggleCooldown)
            {
                return;
            }

            lastToggleTime = Time.unscaledTime;

            bool willOpen = !escMenuCanvas.activeSelf;
            escMenuCanvas.SetActive(willOpen);
            

            if(willOpen)
            {
                Time.timeScale = 0f; //暂停游戏时间

                if(EventSystem.current != null && returnToGameButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(returnToGameButton.gameObject);
                }
            }
            else
            {
                Time.timeScale = 1f; //恢复游戏时间
            }
        }

        public void ReturnToGameButtonPressed()
        {
            escMenuCanvas.SetActive(false);
            Time.timeScale = 1f; //恢复游戏时间
        }

        public void SettingsButtonPressed()
        {
            //这里可以添加打开设置菜单的代码
            Debug.Log("Settings Button Pressed");
        }   

        public void QuitGame()
        {
            //这里可以添加退出游戏的代码
            Debug.Log("Quit Game Button Pressed");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}
