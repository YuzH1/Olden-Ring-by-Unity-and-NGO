using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        [Header("Lock On Settings")]
        [SerializeField] private float lockOnRadius = 20f;
        [SerializeField] private float minimumViewableAngle = -50f;//锁定目标的最小视角，超过这个角度的目标不能被锁定
        [SerializeField] private float maximumViewableAngle = 50f;//锁定目标的最大视角，超过这个角度的目标不能被锁定
        [SerializeField] private float lockOnTargetFlollowSpeed = 0.2f;//锁定目标时摄像机跟随的速度
        [SerializeField] private float setCameraHeightSpeed = 0.05f;//调整摄像机高度的速度
        [SerializeField] float unlockCameraHeight = 1.65f;//取消锁定时摄像机的高度
        [SerializeField] float lockOnCameraHeight = 2f;//锁定目标时摄像机的高度
        private Coroutine cameraLockOnHeightCoroutine;//用于调整锁定目标时摄像机高度的协程
        private List<CharacterManager> availableTargets = new List<CharacterManager>();//存储当前可锁定的目标列表
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockOnTarget;
        public CharacterManager rightLockOnTarget;


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
            //TODO:如果锁定目标，强制摄像机朝向目标旋转 ✅️
            if(player.playerNetworkManager.isLockedOn.Value)
            {
                //主要玩家相机朝向锁定目标的方向旋转
                Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;//保持水平旋转

                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFlollowSpeed);

                //计算从摄像机枢轴到锁定目标的方向
                rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
                rotationDirection.Normalize();

                targetRotation = Quaternion.LookRotation(rotationDirection);
                cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFlollowSpeed);

                //保存当前的旋转角度，以便在取消锁定后继续使用
                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = transform.eulerAngles.x;
            
            
            }
            //否则正常旋转
            //普通旋转
            else
            {
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
    
        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity; //用于记录最近目标的距离
            float shortestDistanceOfRightTarget = Mathf.Infinity;//用于记录锁定目标右边的最近距离目标（+）
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;//用于记录锁定目标左边的最近距离（-）

            //每次重新扫描前先清空，避免旧帧目标残留导致切换异常
            ClearLockOnTargets();

            //TODO:使用layermask
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayer());//在玩家周围一定范围内检测可锁定目标

            for(int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if(lockOnTarget != null)
                {
                    //检查是否在视野中
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                    if(lockOnTarget.isDead.Value)//如果目标已死亡，跳过
                        continue;
                    
                    if(lockOnTarget.transform.root == player.transform.root)//如果目标是玩家自己，跳过
                        continue;

                    
                    //如果目标在视野之外或被环境阻挡。检查下一个潜在目标
                    if(viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                    {
                        RaycastHit hit;

                        //检查目标是否被墙体等遮挡
                        //TODO:添加layermask检查环境 ✅️
                        if(Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                            lockOnTarget.characterCombatManager.lockOnTransform.position, 
                            out hit,
                            WorldUtilityManager.instance.GetEnvironmentLayer()))
                        {
                            //射线击中了某个物体，看不到锁定目标
                            continue;
                        }
                        else
                        {
                            //将目标加入潜在目标列表
                            availableTargets.Add(lockOnTarget);//将符合条件的目标加入列表
                        }
                    }
                }
            }

            //在潜在目标列表中找到最近的目标，并锁定
            for(int j = 0; j < availableTargets.Count; j++)
            {
                // 处理可用目标
                if(availableTargets[j] != null)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[j].transform.position);
                    

                    if(distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[j];
                    }
                    //如果已经有一个最近目标了，继续寻找右边和左边的目标
                    if(player.playerNetworkManager.isLockedOn.Value)
                    {
                        //计算目标相对于玩家的相对位置
                        Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[j].transform.position);
                        //相对位置的x值，负数表示在左边，正数表示在右边
                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        //如果这个目标就是当前锁定的目标，跳过
                        if(availableTargets[j] == player.playerCombatManager.currentTarget)
                            continue;

                        if(relativeEnemyPosition.x <= 0 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availableTargets[j];
                        }
                        else if(relativeEnemyPosition.x >= 0 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availableTargets[j];
                        }
                        
                    }
                }
                else
                {
                    ClearLockOnTargets();
                    player.playerNetworkManager.isLockedOn.Value = false;//如果没有可锁定目标，确保锁定状态为false
                }
            }

        }

        public void SetLockCameraHeight()
        {
            if(cameraLockOnHeightCoroutine != null)
            {
                StopCoroutine(cameraLockOnHeightCoroutine);

            }
            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }

        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
            availableTargets.Clear();
        }

        public IEnumerator WaitThenFindNewTarget()
        {
            while(player.isPerformingAction)
            {
                yield return null;
            }
            ClearLockOnTargets();
            HandleLocatingLockOnTargets();

            if(nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }

            yield return null;
        }

        public IEnumerator SetCameraHeight()
        {
            float duration = 1;
            float timer = 0;

            Vector3 velocity = Vector3.zero;
            Vector3 newLockedCameraHeight = new Vector3(cameraObject.transform.localPosition.x, lockOnCameraHeight);
            Vector3 newUnLockedCameraHeight = new Vector3(cameraObject.transform.localPosition.x, unlockCameraHeight);

            while(timer < duration)
            {
                timer += Time.deltaTime;

                if(player != null)
                {
                    if(player.playerCombatManager.currentTarget != null)
                    {
                        cameraPivotTransform.transform.localPosition = 
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                        // cameraPivotTransform.transform.localRotation = 
                        //     Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFlollowSpeed);
                    }
                    else
                    {
                        cameraPivotTransform.transform.localPosition = 
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                        
                    }
                }
                
                yield return null;
            }

            if(player != null)
            {
                if(player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                    // cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newUnLockedCameraHeight;
                    
                }
            }
            yield return null;
        }

    }
}
