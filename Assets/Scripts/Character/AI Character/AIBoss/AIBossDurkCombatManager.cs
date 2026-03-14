using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class AIBossDurkCombatManager : AICharacterCombatManager
    {
        AIBossDurkCharacterManager aiDurkManager;

        [Header("Damage Colliders")]
        [SerializeField] DurkClubDamageCollider durkClubDamageCollider;
        [SerializeField] DurkStompDamageCollider durkStompDamageCollider;
        
        public float stompAttackRadius = 0.5f;
        
        [Header("Damage")]
        [SerializeField] int baseDamage = 25;
        [SerializeField] float attack01DamageModifier = 1f;
        [SerializeField] float attack02DamageModifier = 1.4f;
        [SerializeField] float attack03DamageModifier = 2f;
        public float stompDamage = 20f;

        [Header("VFX")]
        public GameObject groundHitVFX;

        protected override void Awake()
        {
            base.Awake();

            aiDurkManager = GetComponent<AIBossDurkCharacterManager>();
        }

        #region 动画事件调用
        public void SetAttack01Damage()
        {
            durkClubDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            durkClubDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            
        }

        public void SetAttack03Damage()
        {
            durkClubDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        }

        public void OpenClubDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntsSFX(); //播放攻击呻吟音效
            durkClubDamageCollider.EnableDamageCollider();
            aiDurkManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(aiDurkManager.durkSoundFXManager.clubWhooshes));
        }

        public void CloseClubDamageCollider()
        {
            durkClubDamageCollider.DisableDamageCollider();
        }

        public void ActivateDurkStomp()
        {
            
            
            durkStompDamageCollider.StompAttack();

            
        }


        #endregion

        public override void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            //播放一个基于目标视角的pivot动画
            if(aiCharacter.isPerformingAction)
                return;

            
            if(viewableAngle > 60 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if(viewableAngle < -60 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            
            else if(viewableAngle > 145 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if(viewableAngle < -145 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }

        }
    }
}
