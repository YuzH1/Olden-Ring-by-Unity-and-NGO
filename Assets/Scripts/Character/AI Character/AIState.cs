using UnityEngine;

namespace SG
{
    public class AIState : ScriptableObject
    {
        //这个方法是状态机的核心逻辑所在，每个状态都会实现这个方法来定义在该状态下角色应该执行的行为，并且可以根据条件返回下一个状态，实现状态之间的切换
        public virtual AIState Tick(AICharacterManager aICharacter)
        {


            return this;//默认情况下返回当前状态，表示保持在当前状态不变
        }


        //这个方法是一个辅助方法，用于在状态之间切换。它接受一个新的状态作为参数，并将当前状态切换到新的状态。这个方法可以在Tick方法中被调用，当满足某些条件时需要切换到另一个状态时使用
        protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
        {
            ResetStateFlags(aiCharacter);//在切换状态之前，调用ResetStateFlags方法来重置当前状态的标志位或状态变量，确保新状态能够正确地初始化和运行d
            return newState;
        }

        protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
        {
            //在这里重置当前状态的标志位或状态变量，以确保新状态能够正确地初始化和运行
        }
    }

}
