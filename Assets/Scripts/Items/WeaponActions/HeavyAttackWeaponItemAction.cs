using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Item Action/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";//main = 主手


        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if(!playerPerformingAction.IsOwner)
            {
                return;
            }

            // 检查是否有阻碍

            //与教程的不同处：
            //检查是否正在执行其他动作（防止攻击动画被打断重置）
            if(playerPerformingAction.isPerformingAction)
            {
                return;
            }

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
            
            //执行重攻击的具体逻辑，例如播放动画、应用伤害等
            if(playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01,heavy_Attack_01, true); //播放右手重攻击动画
            }
            if(playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            {
                
            }
        }
    }
}
