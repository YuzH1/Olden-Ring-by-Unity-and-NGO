using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SG
{
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;

        [Header("AI Display Name")]
        public String aiDisplayName = "AI Character";

        [Header("NavMesh Agent")]
        public NavMeshAgent navMeshAgent;

        [Header("Current State")]
        [SerializeField] protected AIState currentState;

        [Header("States")]
        public AIIdleState idleState;
        public AIPursueTargetState pursueTargetState;
        public AICombatStanceState combatStanceState;
        public AIAttackState attackState;
        
        
        

        protected override void Awake()
        {
            base.Awake();
            aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
            aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();

            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsOwner)
            {
                //使用scriptable object实例化状态，确保每个AI角色都有独立的状态实例，避免不同角色之间状态共享导致的逻辑错误
                idleState = Instantiate(idleState);
                pursueTargetState = Instantiate(pursueTargetState);
                combatStanceState = Instantiate(combatStanceState);
                attackState = Instantiate(attackState);

                currentState = idleState;//初始状态设置为idleState
            }
        }

        protected override void Update()
        {
            base.Update();

            
            aiCharacterCombatManager.HandleActionRecovery(this);//在每个更新周期调用这个方法，确保AI角色的行动恢复逻辑能够及时处理和响应角色的状态变化
        }


        protected override void FixedUpdate()
        {
            base.FixedUpdate();


            if(IsOwner)
                ProcessStateMachine();//在每个物理更新周期调用这个方法，确保状态机的逻辑能够及时处理和响应角色的状态变化
        }

        private void ProcessStateMachine()//用来处理状态机的逻辑
        {
            AIState nextState = currentState?.Tick(this);//如果currentState不为null，则调用Tick方法，并将结果赋值给nextState

            if(nextState != null)
            {
                currentState = nextState;
            }

            //确保NavMeshAgent始终与角色保持在同一位置，避免由于物理碰撞或其他因素导致NavMeshAgent与角色分离，从而影响导航和路径计算的准确性
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;

            //更新与当前目标相关的战斗信息，如距离、角度和方向，这些信息对于AI角色的决策和行为选择至关重要
            if(aiCharacterCombatManager.currentTarget != null)
            {
                aiCharacterCombatManager.targetDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
                aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetDirection);
                aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
            }

            if(navMeshAgent.enabled)
            {
                // Vector3 agentDestination = navMeshAgent.destination;
                // float remainingDistance = Vector3.Distance(agentDestination, transform.position);

                // if(remainingDistance > navMeshAgent.stoppingDistance)
                // {
                //     aiCharacterNetworkManager.isMoving.Value = true;
                // }
                // else
                // {
                //     aiCharacterNetworkManager.isMoving.Value = false;
                // }

                bool isPathReady = navMeshAgent.hasPath && !navMeshAgent.pathPending;
                bool hasRemainingDistance = isPathReady && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
                bool hasAgentVelocity = navMeshAgent.velocity.sqrMagnitude > 0.0001f;

                // 只有在路径有效且确实在移动时才同步为“正在移动”，避免高台/不可达路径误判。
                aiCharacterNetworkManager.isMoving.Value = hasRemainingDistance && hasAgentVelocity;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false;
            }

            // AIState nextState = null;
            // if(currentState != null)
            // {
            //     nextState = currentState.Tick(this);
            // }
            // if(nextState != null)
            // {
            //     currentState = nextState;
            // }
        }

    }
}
