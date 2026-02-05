using UnityEngine;
using UnityEngine.InputSystem;

namespace SG
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;//单例模式，确保全局只有一个PlayerCamera实例
        public PlayerManager player;//存储玩家管理器的引用
        public Camera cameraObject;//存储摄像机组件的引用
        [SerializeField] Transform cameraPivotTransform;//摄像机枢轴，用于上下旋转

        //可以改变这些值来调整摄像机行为
        [Header("Camera Settings")]
        [SerializeField] private float cameraSmoothSpeed = 1;//摄像机平滑速度越大，跟随越快
        [SerializeField] private float leftAndRightRotationSpeed = 220;
        [SerializeField] private float upAndDownRotationSpeed = 220;
        [SerializeField] private float minimumPivot = -30;//摄像机向下看的最大角度
        [SerializeField] private float maximumPivot = 60;//摄像机向上看的最大角度

        [Header("Camera Values")]
        [SerializeField]private Vector3 cameraVelocity;
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;


        private void Awake()
        {
            if (instance == null)
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
            DontDestroyOnLoad(gameObject); //确保在场景切换时不销毁此对象
        }

        public void HandleAllCameraActions()
        {
            //在这里添加处理摄像机动作的代码
            if(player != null)
            {
                //例如：跟随玩家、调整视角、与环境碰撞、摄像机抖动等
                HandleFollowTarget();
                HandleRotations();
                
            }
        }

        private void HandleFollowTarget()
        {
            //在这里添加摄像机跟随目标的代码
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            //如果锁定目标，强制摄像机朝向目标旋转
            //否则正常旋转

            //普通旋转
            leftAndRightLookAngle += PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
            upAndDownLookAngle -= PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed * Time.deltaTime;
            //限制上下视角
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);//创建目标旋转四元数
            transform.rotation = targetRotation;//应用旋转

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.transform.localRotation = targetRotation;
        }
    }
}
