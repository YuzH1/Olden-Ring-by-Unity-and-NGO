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
        [SerializeField] LayerMask collideWithLayer;//摄像机碰撞检测的层

        //可以改变这些值来调整摄像机行为
        [Header("Camera Settings")]
        [SerializeField] private float cameraSmoothSpeed = 1;//摄像机平滑速度越大，跟随越快
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float minimumPivot = -30;//摄像机向下看的最大角度
        [SerializeField] float maximumPivot = 60;//摄像机向上看的最大角度
        [SerializeField] float cameraCollisionRadius = 0.2f;//摄像机与环境碰撞时的偏移量

        [Header("Camera Values")]
        [SerializeField] private Vector3 cameraVelocity;
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private Vector3 cameraObjectPosition;//存储摄像机位置，用于碰撞检测后移动到正确位置
        private float cameraDefaultZPosition;//摄像机默认位置,用于碰撞检测时恢复位置
        private float cameraTargetZPosition;//摄像机目标位置,用于碰撞检测时调整位置



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
            cameraDefaultZPosition = cameraObject.transform.localPosition.z;//记录摄像机的默认位置
        }

        public void HandleAllCameraActions()
        {
            //在这里添加处理摄像机动作的代码
            if(player != null)
            {
                //例如：跟随玩家、调整视角、与环境碰撞、摄像机抖动等
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
                
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

        private void HandleCollisions()
        {
            /// <summary>
            /// 处理摄像机与场景的碰撞并修正摄像机位置。
            ///
            /// 思路与步骤：
            /// 1. 摄像机相对于枢轴（`cameraPivotTransform`）有一个默认的局部 z 偏移 `cameraDefaultZPosition`，通常为负值（摄像机在枢轴的后方）。
            /// 2. 每帧先把目标 z 位置 `cameraTargetZPosition` 设为默认值，表示摄像机希望恢复到默认距离。
            /// 3. 计算从枢轴到摄像机的方向向量并归一化：
            ///      direction = normalize(cameraObject.position - cameraPivotTransform.position)
            ///    归一化后投射距离只由 `cameraTargetZPosition` 控制，避免原始向量长度影响检测结果。
            /// 4. 使用 SphereCast（以 `cameraPivotTransform.position` 为起点，以 `cameraCollisionRadius` 为半径）向该方向投射，最大距离为 `Mathf.Abs(cameraTargetZPosition)`，并使用 `collideWithLayer` 做层过滤。
            ///    SphereCast 比普通射线更适合摄像机碰撞，因为摄像机有体积（通过半径模拟），能更稳定地检测薄墙和角落。
            /// 5. 如果检测到碰撞：
            ///      distanceFromHitObject = distance(cameraPivotTransform.position, hit.point)
            ///      cameraTargetZPosition = -(distanceFromHitObject - cameraCollisionRadius)
            ///    说明：取负是因为局部 z 为负（摄像机在枢轴后方）；减去 `cameraCollisionRadius` 是为了把摄像机放在碰撞点之前一个安全距离，避免穿透。
            /// 6. 对目标 z 做最小距离保护：如果 `Mathf.Abs(cameraTargetZPosition)` 小于 `cameraCollisionRadius`，强制设置为 `-cameraCollisionRadius`，防止摄像机进入枢轴内部或与角色重叠。
            /// 7. 使用平滑插值将摄像机的当前 localPosition.z 缓慢过渡到目标 z（这里用 `Mathf.Lerp`，插值系数 0.2f 用于获得平滑的“推挤”效果）。
            /// 8. 将计算后的 `cameraObjectPosition` 应用到 `cameraObject.transform.localPosition`，从而实现碰撞响应。
            ///
            /// 注意事项：
            /// - SphereCast 的最大距离使用绝对值，因为 `cameraTargetZPosition` 可能为负值。SphereCast 需要正的距离参数。
            /// - 鼠标或镜头快速移动时，平滑插值有助于避免视觉抖动，但如果希望更硬的响应可以增大插值权重或直接设置目标位置。
            /// - 若要使摄像机更贴近墙面，可减小 `cameraCollisionRadius`；若要避免穿模问题，可增大该半径或调整插值速度。
            /// </summary>
            //在这里添加摄像机与环境碰撞的代码
            cameraTargetZPosition = cameraDefaultZPosition;
            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;//计算摄像机方向向量
            direction.Normalize();//归一化方向向量,防止距离影响检测

            //SphereCast从一个球体发出射线进行碰撞检测，适合摄像机这种需要一定体积的对象
            //参数：起点，半径，方向，输出碰撞信息，最大距离，层掩码
            if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(cameraTargetZPosition), collideWithLayer))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                cameraTargetZPosition = -(distanceFromHitObject - cameraCollisionRadius);//调整摄像机位置，避免穿透墙体
            }
            //如果摄像机目标位置过近，强制设置一个最小距离
            if(Mathf.Abs(cameraTargetZPosition) < cameraCollisionRadius)
            {
                cameraTargetZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, cameraTargetZPosition, 0.2f);//平滑过渡摄像机位置
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}
