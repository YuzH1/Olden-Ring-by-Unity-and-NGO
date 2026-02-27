using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace SG
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;

        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //角色名字，默认值为"Player"，所有客户端可读，只有拥有者可写

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality); //根据体质等级计算新的最大生命值，并更新网络变量
            PlayerUIManager.Instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value); //更新玩家UI中生命值数据条的最大值，确保它显示正确的最大生命值
            currentHealth.Value = maxHealth.Value; //当最大生命值变化时，重置当前生命值为新的最大生命值，确保玩家不会因为体质等级的提升而立即死亡

        }
    
        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance); //根据耐力等级计算新的最大耐力值，并更新网络变量
            PlayerUIManager.Instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value); //更新玩家UI中耐力值数据条的最大值，确保它显示正确的最大耐力值
            currentStamina.Value = maxStamina.Value; //当最大耐力值变化时，重置当前耐力值为新的最大耐力值，确保玩家不会因为耐力等级的提升而立即无法使用技能
        }
    }
}