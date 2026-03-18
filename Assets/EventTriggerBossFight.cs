using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class EventTriggerBossFight : MonoBehaviour
    {
        [SerializeField] int BossID;

        private void OnTriggerEnter(Collider other)
        {
            AIBossCharacterManager boss = WorldAIManager.instance.GetBossByID(BossID);
            if (!boss.hasBeenDefeated.Value && boss != null)
            {
                boss.WakeBoss(); //当玩家进入这个触发器时
            }
        }
    }
}
