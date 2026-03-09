using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Item Action/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";//main = 主手
        [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02";


        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if(!playerPerformingAction.IsOwner)
            {
                return;
            }

            // 检查是否有阻碍

            // //与教程的不同处：bug已经通过别的方式修改
            // //检查是否正在执行其他动作（防止攻击动画被打断重置）
            // if(playerPerformingAction.isPerformingAction)
            // {
            //     return;
            // }

            if(playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
            {
                return;
            }
            if(!playerPerformingAction.isGrounded)
            {
                return;
            }

            PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);

        }

        private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //检查是否可以连击，前提是玩家正在执行攻击动作，并且当前攻击动作允许连击
            if(playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;//重置连击标志，防止无限连击

                //根据之前的攻击播放连击
                if(playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02,heavy_Attack_02, true); //播放右手重攻击动画2
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01,heavy_Attack_01, true); //播放右手重攻击动画1
                }
            }
            //如果不是正在攻击，或者当前攻击动作不允许连击，那么就执行普通的重攻击
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01,heavy_Attack_01, true); //播放右手重攻击动画
            }
            
            // //执行重攻击的具体逻辑，例如播放动画、应用伤害等
            // if(playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            // {
            //     playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01,heavy_Attack_01, true); //播放右手重攻击动画
            // }
            // if(playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            // {
                
            // }
        }
    }
}
