using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
    public class AICombatStanceState : AIState
    {
        //1.选择一个攻击动作进入攻击状态，由相对于目标的角度和距离来决定
        //2.在这里处理战斗逻辑，当等待攻击动作时（格挡，侧移，闪避等等）
        //3.如果目标离开攻击范围，切换回追逐状态
        //4.如果目标不存在(死亡，距离太远，不可到达等等)，切换回空闲状态

        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks; //AI角色可以使用的攻击动作列表
        private List<AICharacterAttackAction> potentialAttacks;//在此状态中生成的潜在攻击列表，根据当前与目标的距离和角度来筛选出适合的攻击动作
        private AICharacterAttackAction choosenAttack;//当前选择的攻击动作
        private AICharacterAttackAction previousAttack;//上一个攻击动作
        protected bool hasAttack = false;//指示AI角色是否已经执行了攻击动作的标志

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false;//指示AI角色是否可以执行连击的标志
        [SerializeField] protected int chanceToPerformCombo = 25;//AI角色执行连击的概率，范围从0到100
        protected bool hasRolledForComboChance = false;//指示AI角色是否已经为当前攻击动作掷骰子决定是否执行连击的标志

        [Header("Engagement Distance")]
        [SerializeField] protected float maximumEngagementRadius = 5f;//AI角色与目标之间的最大仇恨半径，超过这个距离AI角色将不再追逐目标


        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if(aiCharacter.isPerformingAction)
                return this;
            
            if(!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            if(!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if(aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
                
            }
            // 旋转朝向目标

            //如果目标不再出现，回到空闲状态
            if(aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idleState);

            if(!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                // 检测恢复时间
                // 传递攻击到攻击状态
                // 如果可以执行连击，掷骰子决定是否执行连击
                // 切换状态
            }

            // 如果目标离开攻击范围，切换回追逐状态
            if(aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementRadius)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;

        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            foreach(var potentialAttack in aiCharacterAttacks)//标记，此处tuto有错误
            {
                //1.检索所有可能得攻击
                //2.移除所有在此情况下不能使用的攻击（基于距离和角度）

                // 如果目标距离太近或太远,在攻击的距离范围之外,则跳过这个攻击动作
                if(potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;
                if(potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;
                // 如果目标在攻击的角度范围之外,则跳过这个攻击动作
                if(potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                if(potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                //3.将剩余的所有动作加入列表    
                potentialAttacks.Add(potentialAttack);
            }
            if(potentialAttacks.Count == 0)
                return;

            //4.随机选择一个攻击动作作为当前攻击动作（基于权重）
            var totalWeight = 0;

            foreach(var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight+1);
            var processWeight = 0;

            foreach(var attack in potentialAttacks)
            {
                processWeight += attack.attackWeight;

                if(randomWeightValue <= processWeight)
                {
                    //5.选择此动作然后切换到攻击状态
                    previousAttack = choosenAttack;
                    choosenAttack = attack;
                    hasAttack = true;
                }
            }

        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;

            int randomPercentage = Random.Range(0, 100);

            if(randomPercentage < outcomeChance)
            {
                outcomeWillBePerformed = true;
            }

            return outcomeWillBePerformed;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasRolledForComboChance = false;//重置hasRolledForComboChance标志，确保在每次进入战斗状态时都能重新评估是否执行连击
            hasAttack = false;//重置hasAttack标志，确保在每次进入战斗状态时都能重新评估是否已经执行了攻击动作
        }
    }
}
