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

   
    }
    
    
}
