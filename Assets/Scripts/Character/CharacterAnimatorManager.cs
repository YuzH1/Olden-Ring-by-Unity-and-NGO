using UnityEngine;

namespace SG
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        float Vertical;
        float Horizontal;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            // Implementation for updating animator parameters
            //方法1：直接设置Animator参数，这种方法简单直接，但可能会导致动画切换不够平滑，特别是在输入值变化较大时
            //0.1f是阻尼时间，Time.deltaTime确保每帧都平滑过渡
            character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);

        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation, //要播放的目标动画的名称
            bool isPerformingAction, //这个参数可以用来控制角色在执行动作时不能移动或攻击等，确保动作的完整性和连贯性
            bool applyRootMotion = true, //是否启用根运动，默认为true，表示动画控制角色移动；如果为false，则允许代码控制角色移动
            bool canRotate = false, //这个参数可以用来控制角色在执行动作时是否可以旋转，默认为false，表示在执行动作时不允许旋转；如果为true，则允许旋转
            bool canMove = false) //这个参数可以用来控制角色在执行动作时是否可以移动，默认为false，表示在执行动作时不允许移动；如果为true，则允许移动
        {
            //Debug.Log("Playing action animation: " + targetAnimation + ", isPerformingAction: " + isPerformingAction);
            character.applyRootMotion = applyRootMotion;//如果正在执行动作，启用根运动，让动画控制角色移动；否则禁用根运动，允许代码控制角色移动
            character.animator.CrossFade(targetAnimation, 0.2f);//平滑过渡到目标动画，0.2f是过渡时间，可以根据需要调整
            //可以用于停止角色尝试移动或攻击等，确保动作的完整性和连贯性
            //比如在受到伤害时，角色会播放一个受击动画，这时我们不希望角色在动画播放过程中还能移动或攻击，
            //所以可以设置isPerformingAction为true，来禁止其他动作的执行，直到动画结束后再将isPerformingAction设置为false
            character.isPerformingAction = isPerformingAction;//更新角色的动作状态标志
            character.canRotate = canRotate;//更新角色的旋转能力标志
            character.canMove = canMove;//更新角色的移动能力标志

            //告诉server或host我们播放了动画，这样其他客户端就可以同步动画状态
        }

    }
}