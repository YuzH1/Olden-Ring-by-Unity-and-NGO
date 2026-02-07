using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        [HideInInspector]public float verticalMovement;
        [HideInInspector]public float horizontalMovement;
        [HideInInspector]public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;//为什么用Vector3？因为移动方向是三维的
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2f;
        [SerializeField] float runningSpeed = 5f;
        [SerializeField] float sprintSpeed = 7f;
        [SerializeField] float rotationSpeed = 15f;

        [Header("Dodge Settings")]
        [SerializeField] Vector3 rollDirection;//为什么用Vector3？因为闪避方向也是三维的

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        override protected void Update()
        {
            base.Update();
            //可以在这里添加一些每帧更新的逻辑
            if(player.IsOwner)//只有拥有该对象的客户端才处理移动
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else//如果不是拥有者，则从网络变量中获取移动输入值
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;

                //如果没有锁定目标，则水平输入设为0，只使用moveAmount来控制前进、后退、原地等动画的切换
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
                //如果锁定目标，则传递正常的水平与垂直输入值
            }

        }

        public void HandleAllMovement()
        {
            //Grounded movement
            HandleGroundedMovement();
            HandleRotation();
            //Aerial movement(jumping/falling) can be added later
        }

        private void GetMovementValue()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }

        private void HandleGroundedMovement()
        {

            if(!player.canMove)
                return;//如果角色不能移动，直接返回，不执行移动逻辑
            // Grounded movement logic for the player
            GetMovementValue();

            //根据摄像机方向来移动
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            //这一步是为了让角色能左右移动，否则只能前后移动
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();//归一化，防止斜着走比直着走快
            moveDirection.y = 0;//确保y轴不变，防止角色飞起来

            if(player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
            }
            else
            {
                if(PlayerInputManager.instance.moveAmount > 0.5)
                {
                    //以奔跑速度移动
                    //moveDirection * runningSpeed * Time.deltaTime指的是每秒钟移动的距离
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if(PlayerInputManager.instance.moveAmount > 0 && PlayerInputManager.instance.moveAmount <= 0.5)
                {
                    //以行走速度移动
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
                
            }

        }
    
        private void HandleRotation()
        {
            if(!player.canRotate)
                return;//如果角色不能旋转，直接返回，不执行旋转逻辑
            targetRotationDirection = Vector3.zero;//目标方向初始化为零向量
            //根据摄像机的前方向乘以垂直输入来确定前后方向
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            //根据摄像机的右方向乘以水平输入来确定左右方向
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();//归一化，防止斜着走比直着走快
            targetRotationDirection.y = 0;//确保y轴不变，防止角色倾斜

            if(targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;//如果没有输入，则保持当前朝向
            }

            //计算目标旋转，使角色面向目标方向，四元数表示旋转
            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            //平滑旋转角色，使用插值函数
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;

        }

        public void AttemptToPerformDodge() //尝试闪避，因为闪避可能会被体力等条件限制。
        {
            if(player.isPerformingAction)
                return;//如果正在执行动作，不能闪避
            if(moveAmount > 0)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;//根据摄像机的前方向乘以垂直输入来确定前后方向
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;//根据摄像机的右方向乘以水平输入来确定左右方向

                rollDirection.y = 0;
                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);//计算玩家朝向，面向闪避方向
                player.transform.rotation = playerRotation;//旋转玩家，使其面向闪避方向
                
                //播放一个滚动动画
                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
            }
            else
            {
                //播放一个后撤步动画
                player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);
            }
        }

        public void HandleSprinting()
        {
            if(player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;//如果正在执行动作，不能冲刺，设置网络变量为false
                
            }
            //这里可以添加一些条件来限制冲刺，例如体力值、是否在地面等
            
            //如果满足条件，执行冲刺逻辑，例如增加移动速度、播放冲刺动画等
            //if在移动，set sprinting状态为true，增加移动速度，播放冲刺动画；
            if(moveAmount >= 0.5)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
            // 如果不在移动，set sprinting状态为false，
        }
    }
}