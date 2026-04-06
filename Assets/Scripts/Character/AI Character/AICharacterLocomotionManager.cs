using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if(aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                //将角色的旋转设置为NavMeshAgent的旋转，这样角色就会朝向NavMeshAgent的前进方向，从而实现朝向目标移动的效果
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        public void SnapToGround(AICharacterManager aiCharacter)
        {
            if(aiCharacter == null || aiCharacter.characterController == null)
                return;

            if(!isGrounded)
                return;

            Vector3 rayOrigin = aiCharacter.transform.position + Vector3.up * 1.5f;
            if(Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 5f, groundLayer))
            {
                float groundOffset = hit.point.y - aiCharacter.transform.position.y;
                if(Mathf.Abs(groundOffset) > 0.01f)
                {
                    aiCharacter.characterController.Move(Vector3.up * groundOffset);
                }
            }
        }
    }
}
