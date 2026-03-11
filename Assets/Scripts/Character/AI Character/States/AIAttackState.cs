using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Attack")]
    public class AIAttackState : AIState
    {
        [HideInInspector] public AICharacterAttackAction currentAttack;
        [HideInInspector] public bool willPerformCombo = false;

        [Header("State Flags")]
        protected bool hasPerformedAttack = false;
        protected bool hasPerformedCombo = false;

        [Header("Pivot After Attack")]
        [SerializeField] protected bool pivotAfterAttack = false;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if(aiCharacter.aiCharacterCombatManager.currentTarget == null) 
                return SwitchState(aiCharacter, aiCharacter.idleState);
            if(aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value) 
                return SwitchState(aiCharacter, aiCharacter.idleState);
            
            //攻击时旋转朝向目标
            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0);
            aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);

            // 将movement的值设为0，停止移动

            // 执行连击
            if(willPerformCombo && !hasPerformedCombo)
            {
                if(currentAttack.comboAction != null)
                {
                    // 如果能combo
                    // hasPerformedCombo = true;
                    // currentAttack.comboAction.AttemptToPerformAttack(aiCharacter);
                }
            }

            if(!hasPerformedAttack)
            {
                if(aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                    return this;
                
                if(aiCharacter.isPerformingAction)
                    return this;

                PerformAttack(aiCharacter);

                // 返回当前状态，继续执行攻击逻辑，直到满足切换状态的条件
                return this;
            }

            if(pivotAfterAttack)
            {
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }

            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }

        protected void PerformAttack(AICharacterManager aiCharacter)
        {
            hasPerformedAttack = true;
            currentAttack.AttemptToPerformAttack(aiCharacter);
            aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasPerformedAttack = false;
            hasPerformedCombo = false;
        }

    }
}
