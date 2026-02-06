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
    }
}