using System.Buffers.Text;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Item Action/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";//main = 主手
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";//main = 主手


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
    }
}
