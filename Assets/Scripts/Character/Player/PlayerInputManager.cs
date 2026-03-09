using System;
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

        [Header("Lock On Input")]
        [SerializeField] bool lockOnInput = false;//存储锁定输入状态
        [SerializeField] bool lockOnLeftInput = false;//存储锁定左边目标输入状态
        [SerializeField] bool lockOnRightInput = false;//存储锁定右边目标输入状态
        private Coroutine lockOnCoroutine;//存储锁定协程的引用，用于在切换目标时停止当前的锁定协程

        [Header("Player Action Input")]
        [SerializeField] bool dodgeInput = false;//存储闪避输入状态
        [SerializeField] bool sprintInput = false;//存储冲刺输入状态
        [SerializeField] bool jumpInput = false;//存储跳跃输入状态

        [Header("Switch Inputs")]
        [SerializeField] bool switchRightWeaponInput = false;//存储切换右手武器输入状态
        [SerializeField] bool switchLeftWeaponInput = false;//存储切换左手武器输入状态

        [Header("Light Attack Inputs")]
        [SerializeField] bool lightAttackInput = false;//存储右手轻攻击输入状态

        [Header("Heavy Attack Inputs")]
        [SerializeField] bool HeavyAttackInput = false;//存储重攻击输入状态
        [SerializeField] bool ChargeHeavyAttackInput = false;//存储重攻击按住输入状态

        

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
            if(playerControls != null)
            {
                playerControls.Disable();//初始时禁用输入系统，防止在菜单场景中角色移动
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)//场景切换时调用, oldScene是旧场景，newScene是新场景
        {
            if(newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())//如果新场景是世界场景
            {
                instance.enabled = true;//启用输入管理器

                if(playerControls != null)
                {
                    playerControls.Enable();//启用输入系统，允许在世界场景中角色移动
                }
            }
            //如果当前场景为非世界场景，禁用输入管理器
            //这样可以防止在菜单场景中角色移动
            else
            {
                instance.enabled = false;//禁用输入管理器

                if(playerControls != null)
                {
                    playerControls.Disable();//禁用输入系统，防止在菜单场景中角色移动
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                //订阅输入事件，使用lambda表达式来获取输入值并存储在相应的变量中

                //移动输入
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

                //动作输入
                playerControls.PlayerActions.Dodge.performed += ctx => dodgeInput = true;
                playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true;

                //切换输入
                playerControls.PlayerActions.SwitchRightWeapon.performed += ctx => switchRightWeaponInput = true;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += ctx => switchLeftWeaponInput = true;

                //攻击输入
                    //轻攻击
                playerControls.PlayerActions.RightLightAttack.performed += ctx => lightAttackInput = true;
                    //重攻击
                playerControls.PlayerActions.RightHeavyAttack.performed += ctx => HeavyAttackInput = true;
                    //重攻击蓄力
                playerControls.PlayerActions.RightChargeHeavyAttack.performed += ctx => ChargeHeavyAttackInput = true;
                playerControls.PlayerActions.RightChargeHeavyAttack.canceled += ctx => ChargeHeavyAttackInput = false; 

                //锁定输入
                playerControls.PlayerActions.LockOn.performed += ctx => lockOnInput = true;
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += ctx => lockOnLeftInput = true;
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += ctx => lockOnRightInput = true;

                playerControls.PlayerActions.Sprint.performed += ctx => sprintInput = true;
                playerControls.PlayerActions.Sprint.canceled += ctx => sprintInput = false;
                
                //获取输入值，语法：+= ctx => { movement = ctx.ReadValue<Vector2>(); };lambda表达式
                //为什么用+=而不是=？因为这是事件订阅的语法，允许多个方法响应同一事件
                //为什么用performed？因为它表示输入操作完成时触发
                //为什么用canceled？因为它表示输入操作取消时触发，例如松开按键时触发

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
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            //如果角色已死亡，不处理输入
            if(player.isDead.Value)
            {
                return;
            }
            
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintingInput();
            HandleJumpInput();
            HandleLockOnInput();
            HandleLockOnSwitchTargetInput();
            HandleLightAttackInput();
            HandleHeavyAttackInput();
            HandleChargeHeavyAttackInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchLeftWeaponInput();

        }   

        #region 移动
     
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
            if(!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value) //如果没有锁定目标，或者正在冲刺
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount); //如果没有锁定目标，horizontalInput设为0，只有moveAmount参与动画参数的设置
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput); //如果有锁定目标，horizontalInput参与动画参数的设置               
            }

            //如果有锁定目标，才使用horizontalInput来控制左右移动的动画切换

        }
    
        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
    
        #endregion

        #region 锁定

        private void HandleLockOnInput()
        {
            //如果当前已经锁定目标，检查目标是否已死亡，如果已死亡，解锁
            if(player.playerNetworkManager.isLockedOn.Value)
            {
                if(player.playerCombatManager.currentTarget == null)
                    return;
                
                if(player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockedOn.Value = false;//如果当前目标已死亡，解锁
                    if(lockOnCoroutine != null)
                    {
                        StopCoroutine(lockOnCoroutine); //如果已经有一个锁定协程在运行，先停止它，防止多个协程同时运行导致冲突
                    }

                    lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget()); //启动一个新的协程来寻找锁定目标   //尝试寻找新的锁定目标
                }

             

            }

            if(lockOnInput && player.playerNetworkManager.isLockedOn.Value)
            {
                //取消锁定
                lockOnInput = false;
                PlayerCamera.instance.ClearLockOnTargets();
                player.playerCombatManager.SetTarget(null);//清除当前目标并触发摄像机高度恢复
                player.playerNetworkManager.isLockedOn.Value = false;//更新网络变量，通知所有客户端取消锁定
                return;
            }

            if(lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
            {
                lockOnInput = false;
                
                //如果正在使用需要瞄准的武器，不锁定
                
                //启用锁定
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if(PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    //设置此目标位当前锁定目标
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;//更新网络变量，通知所有客户端当前处于锁定状态
                }
            }
        }

        private void HandleLockOnSwitchTargetInput()
        {
            if(lockOnLeftInput)
            {
                lockOnLeftInput = false;

                if(player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();//切换到左边目标

                    if(PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        //设置此目标位当前锁定目标
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }

            if(lockOnRightInput)
            {
                lockOnRightInput = false;

                if(player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();//切换到右边目标

                    if(PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        //设置此目标位当前锁定目标
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }

        #endregion

        #region 动作
  
        private void HandleDodgeInput()
        {
            //这里可以根据需要添加闪避输入的处理逻辑，例如监听特定按键的按下事件来触发闪避动作
            if(dodgeInput)
            {
                dodgeInput = false; //重置闪避输入状态，防止持续触发闪避动作

                //如果在menu或者ui界面，不触发闪避动作（return）

                //如果在游戏中，触发闪避动作（调用玩家的闪避方法）
                player.playerLocomotionManager.AttemptToPerformDodge();

            }
        }
    
        private void HandleSprintingInput()
        {
            //这里可以根据需要添加冲刺输入的处理逻辑，例如监听特定按键的按下事件来触发冲刺动作
            if(sprintInput)
            {
                //如果在menu或者ui界面，不触发冲刺动作（return）

                //如果在游戏中，触发冲刺动作（调用玩家的冲刺方法）
                player.playerLocomotionManager.HandleSprinting();

            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;//如果没有按下冲刺键，确保网络变量isSprinting为false
            }
        }
    
        private void HandleJumpInput()
        {
            if(jumpInput)
            {
                jumpInput = false; //重置跳跃输入状态，防止左脚踩右脚

                //如果在menu或者ui界面，不触发跳跃动作（return）
                
                //如果在游戏中，尝试触发跳跃动作（调用玩家的跳跃方法）
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }
    
        #endregion

        #region 切换

        private void HandleSwitchRightWeaponInput()
        {
            if(switchRightWeaponInput)
            {
                switchRightWeaponInput = false;
                player.playerEquipmentManager.SwitchRightWeapon();
            }
        }

        private void HandleSwitchLeftWeaponInput()
        {
            if(switchLeftWeaponInput)
            {
                switchLeftWeaponInput = false;
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
        }
            
        #endregion

        #region 攻击

        private void HandleLightAttackInput()
        {
            if(lightAttackInput)
            {
                lightAttackInput = false; //重置攻击输入状态，防止持续触发攻击动作

                //TODO:如果在menu或者ui界面，不触发攻击动作（return）

                //如果在游戏中，尝试触发攻击动作（调用玩家的攻击方法）
                player.playerNetworkManager.SetCharacterActionHand(true); //设置当前使用右手动作，更新网络变量，通知所有客户端当前使用右手动作

                //TODO:如果双手握持武器，该怎样做

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.OH_RB_Action, player.playerInventoryManager.currentRightHandWeapon); //调用玩家战斗管理器的函数，传入右手武器的攻击动作和右手武器数据，执行攻击动作
            }
        }

        private void HandleHeavyAttackInput()
        {
            if(HeavyAttackInput)
            {
                HeavyAttackInput = false; //重置攻击输入状态，防止持续触发攻击动作

                //TODO:如果在menu或者ui界面，不触发攻击动作（return）

                //如果在游戏中，尝试触发攻击动作（调用玩家的攻击方法）
                player.playerNetworkManager.SetCharacterActionHand(true); //设置当前使用右手动作，更新网络变量，通知所有客户端当前使用右手动作

                //TODO:如果双手握持武器，该怎样做

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.OH_RT_Action, player.playerInventoryManager.currentRightHandWeapon); //调用玩家战斗管理器的函数，传入右手武器的攻击动作和右手武器数据，执行攻击动作
            }
        }

        private void HandleChargeHeavyAttackInput()
        {
            //只当我们在需要蓄力的重攻击动作中，并且按住攻击键时，才执行蓄力逻辑
            if(player.isPerformingAction)
            {
                if(player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = ChargeHeavyAttackInput; //更新网络变量，通知所有客户端当前是否正在蓄力重攻击
                }
            }
            
        }
        #endregion

    }
}
