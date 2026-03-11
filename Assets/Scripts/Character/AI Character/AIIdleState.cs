using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Idle")]
    public class AIIdleState : AIState
    {
        public override AIState Tick(AICharacterManager aICharacter)
        {
            // Idle state logic here
            if(aICharacter.characterCombatManager.currentTarget != null)
            {
                return SwitchState(aICharacter, aICharacter.pursueTargetState);   
            }
            else
            {
                aICharacter.aiCharacterCombatManager.FindATargetVialineOfSight(aICharacter);
                Debug.Log("AI is looking for a target");
                return this;
            }


        }

        private void FindTargetVialineOfSight(AICharacterManager aICharacter)
        {
            // Implement logic to find target within line of sight
        }
    }
}
