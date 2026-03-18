using UnityEngine;

namespace SG
{
    public class AIBossNetworkManager : AICharacterNetworkManager
    {
        AIBossCharacterManager aiBossCharacter;

        protected override void Awake()
        {
            base.Awake();
            aiBossCharacter = GetComponent<AIBossCharacterManager>();
        }

        public override void CheckHP(int oldValue, int newValue)
        {
            base.CheckHP(oldValue, newValue);

            if(aiBossCharacter.IsOwner)
            {
                if(currentHealth.Value <= 0)
                {
                    if(currentHealth.Value <= 0)
                    {
                        return;
                        
                    }
                }

                float healthNeededForShift = maxHealth.Value * (aiBossCharacter.minimumHealthPrcentageToShift / 100f); //计算阶段转换所需的生命值，基于Boss的最大生命值和阶段转换的最低生命值百分比
                if(currentHealth.Value <= healthNeededForShift)
                {
                    aiBossCharacter.PhaseShift(); //当Boss的当前生命值低于阶段转换所需的生命值时，触发阶段转换
                }
                
            }
        }
    }
}
