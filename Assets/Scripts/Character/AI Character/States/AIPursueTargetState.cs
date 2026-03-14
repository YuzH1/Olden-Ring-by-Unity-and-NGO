using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class AIPursueTargetState : AIState
    {
        

        public override AIState Tick(AICharacterManager aiCharacter)
        {

            //如果我们正在执行动作，什么都不做直到动作完成
            if(aiCharacter.isPerformingAction)
                return this;//如果我们正在执行动作，什么都不做直到动作完成

            //如果当前目标为null，返回空闲状态
            if(aiCharacter.characterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idleState);

            //确保navmesh代理没有被破坏
            if(!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;


            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);//旋转朝向目标

            // if(aiCharacter.aiCharacterCombatManager.enablePivot)
            // {
                
            //     //如果目标在视野范围之外，转向目标
            //     if(aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV 
            //         || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
            //     {
            //         aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            //     }
                
            // }

            //如果到了目标位置的战斗范围，切换到战斗状态
            // if(aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStanceState.maximumEngagementRadius)
            // {
            //     return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            // }
            if(aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            {
                return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            }

            //如果目标不可到达，或目标距离过远，回到老巢

            //追踪目标
            //选择1：直接追踪目标位置
            // aiCharacter.navMeshAgent.SetDestination(aiCharacter.characterCombatManager.currentTarget.transform.position);
            //选择2：计算路径并设置路径，确保路径是可行的
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}
