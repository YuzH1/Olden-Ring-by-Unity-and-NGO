using UnityEngine;

namespace SG
{
    public class PlayerAnimatorManager : CharacterAnimatorManager
    {
        PlayerManager player;

        override protected void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        private void OnAnimatorMove()
        {
            if(player.applyRootMotion)
            {
                //如果启用根运动，让动画控制角色移动
                //使用动画的位移来移动角色，这样可以确保角色的位置与动画的根运动一致，避免位置漂移等问题
                player.characterController.Move(player.animator.deltaPosition);
                player.transform.rotation *= player.animator.deltaRotation;
            }
        }

        #region 动画事件调用

        public override void EnableCanDoCombo()//动画事件调用
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainHandWeapon = true;
            }
            else
            {
                //启用副手武器的连击能力，前提是玩家装备了副手武器
            }

        }

        public override void DisableCanDoCombo()//动画事件调用
        {
            player.playerCombatManager.canComboWithMainHandWeapon = false;
            //禁用副手武器的连击能力
            //player.playerCombatManager.canComboWithOffHandWeapon = false;
        }

        #endregion
    }
}