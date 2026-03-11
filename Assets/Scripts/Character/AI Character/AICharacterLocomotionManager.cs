using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        public void RotateTowrdsAgent(AICharacterManager aiCharacter)
        {
            if(aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                //将角色的旋转设置为NavMeshAgent的旋转，这样角色就会朝向NavMeshAgent的前进方向，从而实现朝向目标移动的效果
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
    }
}
