using UnityEngine;


namespace SG
{
    public class PlayerManager : CharacterManager
    {
        PlayerLocomotionManager playerLocomotionManager;
        protected override void Awake()
        {
            base.Awake();
            // PlayerManager specific initialization can go here
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        }

        override protected void Update()
        {
            base.Update();
            // Player-specific update logic
            if(!IsOwner)//只有拥有该对象的客户端才处理移动
                return;
            playerLocomotionManager.HandleAllMovement();
        }
    }
}