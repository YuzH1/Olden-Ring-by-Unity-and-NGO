using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Item Action/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [Header("Light Attack Animations")]
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";//main = 主手
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";//main = 主手

        [Header("Running Attack Animations")]
        [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";//main = 主手

        [Header("Roll Attack Animations")]
        [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";//main = 主手

        [Header("Back Step Attack Animations")]
        [SerializeField] string backStep_Attack_01 = "Main_Backstep_Attack_01";//main = 主手


        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if(!playerPerformingAction.IsOwner)
            {
                return;
            }

            // 检查是否有阻碍

            //与教程的不同处：bug已经通过别的方式修改
            //检查是否正在执行其他动作（防止攻击动画被打断重置）
            // if(playerPerformingAction.isPerformingAction)
            // {
            //     return;
            // }

            if(playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
            {
                return;
            }
            if(!playerPerformingAction.playerLocomotionManager.isGrounded)
            {
                return;
            }

            // 检查是否正在冲刺，如果正在冲刺则执行冲刺攻击，否则执行普通轻攻击
            if(playerPerformingAction.characterNetworkManager.isSprinting.Value)
            {
                PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            // 检查是否在滚动状态
            if(playerPerformingAction.characterCombatManager.canPerformRollingAttack)
            {
                PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            if(playerPerformingAction.characterCombatManager.canPerformBackStepAttack)
            {
                PerformBackStepAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);

        }

        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //检查是否可以连击，前提是玩家正在执行攻击动作，并且当前攻击动作允许连击
            if(playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;//重置连击标志，防止无限连击

                //根据之前的攻击播放连击
                if(playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02,light_Attack_02, true); //播放右手轻攻击动画2
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,light_Attack_01, true); //播放右手轻攻击动画1
                }
            }
            //如果不是正在攻击，或者当前攻击动作不允许连击，那么就执行普通的轻攻击
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,light_Attack_01, true); //播放右手轻攻击动画
            }
            // //执行轻攻击的具体逻辑，例如播放动画、应用伤害等
            // if(playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            // {
            //     playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,light_Attack_01, true); //播放右手轻攻击动画
            // }
            // if(playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            // {
                
            // }
        }
    
        private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // 检查是否是双持，如果双持武器播放双持武器冲刺攻击动画

            // 如果不是则播放单持武器冲刺攻击动画
            // BUG:如果在冲刺途中连续点击攻击键，会一直重复攻击动画前几帧
            if(playerPerformingAction.isPerformingAction)
            {
                return;
            }

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RunningAttack01, run_Attack_01, true); //播放右手冲刺攻击动画


        }
    
        private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // 检查是否是双持，如果双持武器播放双持武器滚动攻击动画

            // 如果不是则播放单持武器滚动攻击动画
            // BUG:如果在翻滚途中连续点击攻击键，会一直重复攻击动画前几帧
            // if(playerPerformingAction.isPerformingAction)
            // {
            //     return;
            // }
            playerPerformingAction.characterCombatManager.canPerformRollingAttack = false;//重置滚动攻击标志，防止无限滚动攻击
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RollingAttack01, roll_Attack_01, true); //播放右手滚动攻击动画
        }

        private void PerformBackStepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // 检查是否是双持，如果双持武器播放双持武器后跳攻击动画

            // 如果不是则播放单持武器后跳攻击动画
            playerPerformingAction.characterCombatManager.canPerformBackStepAttack = false;//重置后跳攻击标志，防止无限后跳攻击
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.BackStepAttack01, backStep_Attack_01, true); //播放右手后跳攻击动画
        }
    }
}

        
