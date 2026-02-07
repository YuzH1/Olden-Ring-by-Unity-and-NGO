using UnityEngine;


namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector]public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector]public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        protected override void Awake()
        {
            base.Awake();
            // PlayerManager specific initialization can go here
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
        }

        protected override void Update()
        {
            base.Update();
            // Player-specific update logic
            if(!IsOwner)//只有拥有该对象的客户端才处理移动
                return;
            playerLocomotionManager.HandleAllMovement();
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
            }
        }
    }
}