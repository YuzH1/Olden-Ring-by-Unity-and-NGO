using UnityEngine;

namespace SG
{
    public class ResetActionFlag : StateMachineBehaviour
    {
        CharacterManager character;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
           if(character == null)
            {
                character = animator.GetComponent<CharacterManager>();
            }
            character.isPerformingAction = false;//当进入一个新的动画状态时，重置动作标志，允许角色执行新的动作
            character.canRotate = true;//重置旋转标志，允许角色旋转
            character.canMove = true;//重置移动标志，允许角色移动
            //防止翻滚后角色被加速，因为翻滚动画可能会有根运动，
            // 如果不禁用根运动，角色可能会在翻滚结束后继续被动画推动，导致位置异常，
            // 所以在进入新状态时禁用根运动，让代码控制角色移动，确保角色位置正常
            //这是其中一种方法，另一种方法是在翻滚动画的末尾添加一个事件，
            // 在事件中调用一个方法来禁用根运动，这样可以更精确地控制何时禁用根运动，
            // 避免在翻滚动画的前半部分就禁用根运动，导致翻滚动画无法正常播放
            //character.animator.applyRootMotion = false;
            character.applyRootMotion = false;//禁用根运动，让代码控制角色移动，确保角色位置正常
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}