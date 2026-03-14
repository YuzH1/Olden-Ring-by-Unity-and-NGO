using UnityEngine;

namespace SG
{
    public class AIBossDurkCharacterManager : AIBossCharacterManager
    {
        [HideInInspector] public AIBossDurkSoundFXManager durkSoundFXManager;
        [HideInInspector] public AIBossDurkCombatManager durkCombatManager;

        protected override void Awake()
        {
            base.Awake();

            durkSoundFXManager = GetComponent<AIBossDurkSoundFXManager>();
            durkCombatManager = GetComponent<AIBossDurkCombatManager>();
        }
    }
}
