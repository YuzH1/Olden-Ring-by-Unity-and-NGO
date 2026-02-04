using UnityEngine;

namespace SG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;//为什么用Vector3？因为移动方向是三维的
        private Vector3 targetRotationDirection;

        [SerializeField] float walkingSpeed = 2f;
        [SerializeField] float runningSpeed = 5f;
        [SerializeField] float rotationSpeed = 15f;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {
            //Grounded movement
            HandleGroundedMovement();
            HandleRotation();
            //Aerial movement(jumping/falling) can be added later
        }

        private void GetverticalAndHorizontalInput()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
        }

        private void HandleGroundedMovement()
        {
            // Grounded movement logic for the player
            GetverticalAndHorizontalInput();

            //根据摄像机方向来移动
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            //这一步是为了让角色能左右移动，否则只能前后移动
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();//归一化，防止斜着走比直着走快
            moveDirection.y = 0;//确保y轴不变，防止角色飞起来

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
    
        private void HandleRotation()
        {
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
    }
}