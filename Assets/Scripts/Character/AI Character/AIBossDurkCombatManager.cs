using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class AIBossDurkCombatManager : AICharacterCombatManager
    {
        [Header("Damage Colliders")]
        [SerializeField] DurkClubDamageCollider DurksClubDamageCollider;
        
        [Header("Damage")]
        [SerializeField] int baseDamage = 25;
        [SerializeField] float attack01DamageModifier = 1f;
        [SerializeField] float attack02DamageModifier = 1.4f;
        [SerializeField] float attack03DamageModifier = 2f;

        #region 动画事件调用
        public void SetAttack01Damage()
        {
            DurksClubDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            DurksClubDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            
        }

        public void SetAttack03Damage()
        {
            DurksClubDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        }

        public void OpenClubDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunts(); //播放攻击呻吟音效
            DurksClubDamageCollider.EnableDamageCollider();
            
        }

        public void CloseClubDamageCollider()
        {
            DurksClubDamageCollider.DisableDamageCollider();
        }

        public void ActivateDurkStomp()
        {
            
        }
        

        #endregion
    }
}
