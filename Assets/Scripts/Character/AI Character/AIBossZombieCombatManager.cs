using UnityEngine;

namespace SG
{
    public class AIBossZombieCombatManager : AICharacterCombatManager
    {
        [Header("Damage Colliders")]
        [SerializeField] UnDeadHandDamageCollider rightHandDamageCollider;
        [SerializeField] UnDeadHandDamageCollider leftHandDamageCollider;

        [Header("Damage")]
        [SerializeField] int baseDamage = 15;
        [SerializeField] float attack01DamageModifier = 1f;
        [SerializeField] float attack02DamageModifier = 1.4f;
        [SerializeField] float attack03DamageModifier = 1.8f;
        [SerializeField] float attack04DamageModifier = 2f;

        #region 动画事件调用
        public void SetAttack01Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        }

        public void SetAttack03Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        }

        public void SetAttack04Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack04DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack04DamageModifier;
        }

            
        public void OpenBossZombieRightHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunts(); //播放攻击呻吟音效
            rightHandDamageCollider.EnableDamageCollider();
            
        }

        public void DisableBossZombieRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void OpenBossZombieLeftHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunts(); //播放攻击呻吟音效
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void DisableBossZombieLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }

        #endregion
    }
}
