using System.Buffers.Text;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Item Action/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";//main = 主手


        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if(!playerPerformingAction.IsOwner)
            {
                return;
            }

            //检查是否有阻碍
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

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);

        }

        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            
            //执行轻攻击的具体逻辑，例如播放动画、应用伤害等
            if(playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,light_Attack_01, true); //播放右手轻攻击动画
            }
            if(playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            {
                
            }
        }
    }
}
