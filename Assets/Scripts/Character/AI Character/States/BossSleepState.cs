using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/BossSleepState")]
    public class BossSleepState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            return base.Tick(aiCharacter);
        }
    }
}
