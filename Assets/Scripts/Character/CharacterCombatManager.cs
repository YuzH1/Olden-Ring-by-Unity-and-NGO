using UnityEngine;
using UnityEngine.TextCore.Text;
using Unity.Netcode;

namespace SG
{
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;

        [Header("Last Attack Animation Performed")]
        public string lastAttackAnimationPerformed;

        [Header("Attack Target")]
        public CharacterManager currentTarget;

        [Header("Attack Type")]
        public AttackType currentAttackType;//当前攻击类型，轻攻击，重攻击，战技等

        [Header("Lock On Transform")]
        public Transform lockOnTransform;//锁定目标的Transform，用于调整角色朝向和攻击方向

        [Header("Attack Flags")]
        public bool canPerformRollingAttack = false;//是否可以执行滚动攻击，滚动攻击是一种特殊的攻击类型，只有在特定条件下才能执行，比如在翻滚的过程中或者翻滚结束后的短时间内
        public bool canPerformBackStepAttack = false;

        protected virtual void Awake() 
        {
            character = GetComponent<CharacterManager>();
            
        }

        public virtual void SetTarget(CharacterManager newTarget)
        {
            if(character.IsOwner)
            {
                if(newTarget != null)
                {
                    currentTarget = newTarget;
                    //告诉网络管理器当前锁定的目标
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null;
                }
            }
        }
   
        public void EnableIsInvulnerable()
        {
            if(character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;
        }

        public void DisableIsInvulnerable()
        {
            if(character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = false;
        }


        public virtual void EnableCanDoCombo()
        {
            
        }
        public virtual void DisableCanDoCombo()
        {
            
        }

        public void EnableCanDoRollingAttack()
        {
            if(character.IsOwner)
                canPerformRollingAttack = true;
            
        }

        public void DisableCanDoRollingAttack()
        {
            if(character.IsOwner)
                canPerformRollingAttack = false;
            
        }

        public void EnableCanDoBackStepAttack()
        {
            if(character.IsOwner)
                canPerformBackStepAttack = true;
            
        }

        public void DisableCanDoBackStepAttack()
        {
            if(character.IsOwner)
                canPerformBackStepAttack = false;
            
        }
   
    }
    
    
}
