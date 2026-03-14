using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    public class DurkStompDamageCollider : DamageCollider
    {
        AIBossDurkCharacterManager aiDurkManager;

        protected override void Awake()
        {
            base.Awake();

            aiDurkManager = GetComponentInParent<AIBossDurkCharacterManager>();
        }

        public void StompAttack()
        {
            GameObject stompVFX = Instantiate(aiDurkManager.durkCombatManager.groundHitVFX, transform);

            Collider[] colliders = Physics.OverlapSphere(transform.position, aiDurkManager.durkCombatManager.stompAttackRadius, WorldUtilityManager.instance.GetCharacterLayer());
            List<CharacterManager> charactersDamaged = new List<CharacterManager>(); //已经被这个攻击造成过伤害的角色列表，用于避免重复伤害
        
            foreach(var collider in colliders)
            {
                // 处理碰撞体
                CharacterManager targetCharacter = collider.GetComponentInParent<CharacterManager>();

                if(targetCharacter != null)
                {
                    if(targetCharacter == aiDurkManager)
                        continue;
                    
                    if(targetCharacter.isDead.Value)
                        continue;

                    if(charactersDamaged.Contains(targetCharacter))
                        continue;

                    charactersDamaged.Add(targetCharacter); //将目标添加到已经造成过伤害的角色列表中


                    if(targetCharacter.IsOwner)
                    {
                        
                        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect); //创建一个伤害效果实例
                        damageEffect.physicalDamage = aiDurkManager.durkCombatManager.stompDamage;
                        damageEffect.poiseDamage = aiDurkManager.durkCombatManager.stompDamage;
                        

                        targetCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect); //将伤害效果应用到目标角色身上
                    }
                }
            }
        }
    }
}
