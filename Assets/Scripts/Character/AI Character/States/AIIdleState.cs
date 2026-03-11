using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Idle")]
    public class AIIdleState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // Idle state logic here
            if(aiCharacter.characterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);   
            }
            else
            {
                aiCharacter.aiCharacterCombatManager.FindATargetVialineOfSight(aiCharacter);
                // Debug.Log("AI is looking for a target");
                return this;
            }


        }

        private void FindTargetVialineOfSight(AICharacterManager aiCharacter)
        {
            // Implement logic to find target within line of sight
        }
    }
}
