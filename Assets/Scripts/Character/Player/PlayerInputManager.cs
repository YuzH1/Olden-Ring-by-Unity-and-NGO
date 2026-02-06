using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;

        //分步骤实现目标
        //1.找到一个方法获取手柄或键盘的输入值
        //2.根据这些值来移动角色
        PlayerControls playerControls;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 movementInput;//存储输入的移动值
        public float verticalInput;//存储垂直输入值
        public float horizontalInput;//存储水平输入值
        public float moveAmount;//存储移动量

        [Header("Camera Movement Input")]
        [SerializeField] Vector2 cameraInput;//存储输入的摄像机控制值
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

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
            //确保在场景切换时不销毁此对象，dontDestroyOnLoad要在OnSceneChange之前调用，
            //因为如果在场景切换时销毁了对象，输入管理器就无法工作了
            DontDestroyOnLoad(gameObject); 

            SceneManager.activeSceneChanged += OnSceneChange;//订阅场景切换事件, 当场景切换时调用OnSceneChange方法
            instance.enabled = false;//初始时禁用输入管理器
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)//场景切换时调用, oldScene是旧场景，newScene是新场景
        {
            if(newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())//如果新场景是世界场景
            {
                instance.enabled = true;//启用输入管理器
            }
            //如果当前场景为非世界场景，禁用输入管理器
            //这样可以防止在菜单场景中角色移动
            else
            {
                instance.enabled = false;//禁用输入管理器
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
                
                //获取输入值，语法：+= ctx => { movement = ctx.ReadValue<Vector2>(); };lambda表达式
                //为什么用+=而不是=？因为这是事件订阅的语法，允许多个方法响应同一事件
                //为什么用performed？因为它表示输入操作完成时触发

                playerControls.Enable();

            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;//取消订阅场景切换事件，防止内存泄漏
        }

        private void OnApplicationFocus(bool focus)//应用程序获得或失去焦点时调用，焦点是什么？就是程序是否在前台运行
        {
            if(enabled)
            {
                if(focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
        }

        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            //计算移动量，范围0到1，移动量：水平和垂直输入的绝对值之和，最大值为1，
            //有什么用：控制角色的移动速度
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            if(moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f; //慢走
            }
            else if(moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1f; //快走或跑
            }
            //引用动画参数，根据输入值来控制动画的切换
            if(player == null)
                return;
            //这里将horizontalInput设为0，因为在角色未锁定目标前，应该只会处于前进、后退、原地等状态，不会有左右移动的动画
            //如果没有锁定，只使用moveAmount来控制前进、后退、原地等动画的切换，horizontalInput不参与动画参数的设置
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
            //如果有锁定目标，才使用horizontalInput来控制左右移动的动画切换

        }
    
        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
    }
}
