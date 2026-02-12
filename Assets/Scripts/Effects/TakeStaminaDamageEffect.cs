using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage Effect")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage; //体力伤害数值

        public override void ProcessEffect(CharacterManager character)
        {
            //处理体力伤害的逻辑
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            //计算体力伤害的逻辑
            if(character.IsOwner)
            {
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
    
}
