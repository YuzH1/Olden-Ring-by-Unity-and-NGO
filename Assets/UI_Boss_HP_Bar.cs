using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SG
{
    public class UI_Boss_HP_Bar : UI_StatBar
    {
        [SerializeField] AIBossCharacterManager aiBoss;
        [SerializeField] TextMeshProUGUI bossNameText;

        public void EnableBossHPBar(AIBossCharacterManager aiBoss)
        {
            this.aiBoss = aiBoss;
            aiBoss.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnBossHPChanged;
            SetMaxStat(aiBoss.aiCharacterNetworkManager.maxHealth.Value);
            SetStat(aiBoss.aiCharacterNetworkManager.currentHealth.Value);
            bossNameText.text = aiBoss.aiDisplayName;
        }

        private void OnDestroy()
        {
            
            aiBoss.aiCharacterNetworkManager.currentHealth.OnValueChanged -= OnBossHPChanged;
            
        }

        private void OnBossHPChanged(int previousValue, int newValue)
        {
            SetStat(newValue);

            if(newValue <= 0)
            {
                RemoveHPBar(3f);
            }
        }

        public void RemoveHPBar(float time)
        {
            Destroy(gameObject, time);
            Destroy(bossNameText.gameObject, time);
        }
    }
}
