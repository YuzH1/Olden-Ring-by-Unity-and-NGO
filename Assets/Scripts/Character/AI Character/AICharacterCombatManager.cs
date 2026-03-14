using UnityEngine;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;

        [Header("Action Recovery")]
        public float actionRecoveryTimer = 0f;//这个计时器用来控制AI角色在执行攻击动作后需要等待多久才能进行下一次行动，这样可以避免AI角色连续执行攻击动作而没有任何间隔，增加了战斗的节奏感和策略性

        [Header("Pivot")]
        public bool enablePivot = true;//这个布尔变量用来控制AI角色是否启用pivot功能，如果启用，AI角色在追逐目标时会根据目标的位置和角度来选择合适的pivot动画进行转向，这样可以让AI角色在追逐过程中表现得更加自然和智能
        
        [Header("Target Info")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 10f;
        public float minimumFOV = -35f;//最小检测角度，单位为度，表示AI角色能够检测到目标的视野范围
        public float maximumFOV = 35f;//最大检测角度，单位为度，表示AI角色能够检测到目标的视野范围

        [Header("Attack Rotation")]
        public float attackRotationSpeed = 25f;

        protected override void Awake()
        {
            base.Awake();

            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform; //在AI角色的子对象中找到一个带有LockOnTransform组件的对象，并将其transform赋值给lockOnTransform变量，这样战斗管理器就可以使用这个transform来进行目标锁定和旋转等操作
            aiCharacter = GetComponent<AICharacterManager>();
        }

        public void FindATargetVialineOfSight(AICharacterManager aiCharacter)
        {
            if(currentTarget != null)
            {
                return;
            }

            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayer());

            for(int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].GetComponent<CharacterManager>();

                if(targetCharacter == null)
                    continue;
                if(targetCharacter == aiCharacter)
                    continue;
                if(targetCharacter.isDead.Value)
                    continue;

                //友军还是敌人？
                if(WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    //检查是否在视线范围内
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angelOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if(angelOfPotentialTarget > minimumFOV && angelOfPotentialTarget < maximumFOV)
                    {
                        //检查是否有障碍物挡住视线
                        if(Physics.Linecast(
                            aiCharacter.transform.position, 
                            targetCharacter.characterCombatManager.lockOnTransform.position, 
                            WorldUtilityManager.instance.GetEnvironmentLayer()))
                        {
                            Debug.DrawLine(aiCharacter.transform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                        }
                        else
                        {
                            targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(aiCharacter.transform, targetDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                            if(enablePivot)
                                PivotTowardsTarget(aiCharacter);
                        }
                    }
                }



            }
        }
    
        public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            //播放一个基于目标视角的pivot动画
            if(aiCharacter.isPerformingAction)
                return;

            if(viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
            }
            else if(viewableAngle <= -20 && viewableAngle >= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
            }
            else if(viewableAngle > 60 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if(viewableAngle < -60 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            else if(viewableAngle > 110 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
            }
            else if(viewableAngle < -110 && viewableAngle >= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
            }
            else if(viewableAngle > 145 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if(viewableAngle < -145 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }

        }
    
        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if(actionRecoveryTimer > 0)
            {
                if(!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if(aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter)//在攻击的时候调用
        {
            if(currentTarget == null)
                return;
            
            // 1.检查是否能旋转
            if(!aiCharacter.aiCharacterLocomotionManager.canRotate)
                return;
            
            //如果正在执行攻击动作，才允许旋转朝向目标，这样可以确保在攻击过程中角色能够正确地对准目标
            if(!aiCharacter.isPerformingAction)
                return;
            // 2.以特殊的速度在特殊的帧中旋转朝向目标
            Vector3 direction = currentTarget.transform.position - aiCharacter.transform.position;
            direction.y = 0;
            direction.Normalize();

            if(direction == Vector3.zero)
            {
                direction = aiCharacter.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            aiCharacter.transform.rotation = Quaternion.Slerp(
                aiCharacter.transform.rotation, 
                targetRotation, 
                attackRotationSpeed * Time.deltaTime);

        }
    }
}
