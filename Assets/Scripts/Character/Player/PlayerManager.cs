using UnityEngine;


namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector]public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector]public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        protected override void Awake()
        {
            base.Awake();
            // PlayerManager specific initialization can go here
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
        }

        protected override void Update()
        {
            base.Update();
            // Player-specific update logic
            if(!IsOwner)//只有拥有该对象的客户端才处理移动
                return;
            
            //处理移动
            playerLocomotionManager.HandleAllMovement();

            //处理耐力恢复
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            if(!IsOwner)//只有拥有该对象的客户端才处理摄像机跟随等逻辑
                return;
            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions(); //调用摄像机的处理函数，确保在所有对象更新后执行摄像机相关逻辑
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // 如果是拥有者（本地玩家），可以执行一些特定的初始化逻辑，例如设置摄像机跟随等
            if(IsOwner)
            {
                PlayerCamera.instance.player = this; //将玩家管理器的引用传递给摄像机
                PlayerInputManager.instance.player = this; //将玩家管理器的引用传递给输入管理器

                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.playerUIHudManager.SetNewStaminaValue; //更新UI中的耐力值
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenerationTimer; //重置耐力恢复计时器

                //在SL时，这个会被移除
                playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value); //在玩家生成时中的最大耐力值
                playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
                PlayerUIManager.Instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value); //在玩家生成时更新UI中的最大耐力值
            }
        }
    }
}