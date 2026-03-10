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
    }
}
