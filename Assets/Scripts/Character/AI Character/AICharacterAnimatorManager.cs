using UnityEngine;

namespace SG
{
    public class AICharacterAnimatorManager : CharacterAnimatorManager
    {
        AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake();
            aiCharacter = GetComponent<AICharacterManager>();
        }

        private void OnAnimatorMove()
        {
            //HOST
            if(aiCharacter.IsOwner)
            {
                if(!aiCharacter.aiCharacterLocomotionManager.isGrounded)
                    return;

                Vector3 velocity = aiCharacter.animator.deltaPosition;

                aiCharacter.characterController.Move(velocity);
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;

            }

            //CLIENT 轻量客户端预测 + 平滑纠偏
            else
            {
                if(!aiCharacter.aiCharacterLocomotionManager.isGrounded)
                    return;

                Vector3 velocity = aiCharacter.navMeshAgent.velocity * Time.deltaTime;

                aiCharacter.characterController.Move(velocity);

                //把当前位置往网络同步位置 networkPosition 平滑拉近。这是“纠偏”，避免远端看到抖动或瞬移。
                aiCharacter.transform.position = Vector3.SmoothDamp(transform.position, 
                    aiCharacter.characterNetworkManager.networkPosition.Value, 
                    ref aiCharacter.characterNetworkManager.networkPositionVelocity, 
                    aiCharacter.characterNetworkManager.networkPositionSmoothTime);
                    
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }


    }
}
