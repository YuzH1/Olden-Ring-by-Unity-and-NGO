using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
        
        [Header("NavMesh Agent")]
        public NavMeshAgent navMeshAgent;

        [Header("Current State")]
        [SerializeField] AIState currentState;

        [Header("States")]
        public AIIdleState idleState;
        public AIPursueTargetState pursueTargetState;
        //战斗状态
        //攻击状态
        

        protected override void Awake()
        {
            base.Awake();
            aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
            aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();

            //使用scriptable object实例化状态，确保每个AI角色都有独立的状态实例，避免不同角色之间状态共享导致的逻辑错误
            idleState = Instantiate(idleState);
            pursueTargetState = Instantiate(pursueTargetState);

            currentState = idleState;//初始状态设置为idleState
        }



        protected override void FixedUpdate()
        {
            base.FixedUpdate();

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

            if(navMeshAgent.enabled)
            {
                Vector3 agentDestination = navMeshAgent.destination;
                float remainingDistance = Vector3.Distance(agentDestination, transform.position);

                if(remainingDistance > navMeshAgent.stoppingDistance)
                {
                    aiCharacterNetworkManager.isMoving.Value = true;
                }
                else
                {
                    aiCharacterNetworkManager.isMoving.Value = false;
                }
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
