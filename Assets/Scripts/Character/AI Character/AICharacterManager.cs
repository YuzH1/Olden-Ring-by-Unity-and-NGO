using UnityEngine;

namespace SG
{
    public class AICharacterManager : CharacterManager
    {
        public AICharacterCombatManager aiCharacterCombatManager;

        protected override void Awake()
        {
            base.Awake();
            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        }

        [Header("Current State")]
        [SerializeField] AIState currentState;


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
