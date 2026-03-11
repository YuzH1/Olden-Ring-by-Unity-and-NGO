using UnityEngine;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        [Header("Detection")]
        [SerializeField] float detectionRadius = 10f;
        [SerializeField] float minimumDetectionAngle = -35f;//最小检测角度，单位为度，表示AI角色能够检测到目标的视野范围
        [SerializeField] float maximumDetectionAngle = 35f;//最大检测角度，单位为度，表示AI角色能够检测到目标的视野范围

        public void FindATargetVialineOfSight(AICharacterManager aiCharacter)
        {
            if(currentTarget != null)
            {
                return;
            }

            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayer());

            for(int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].GetComponent<CharacterManager>();

                if(targetCharacter == null)
                    continue;
                if(targetCharacter == aiCharacter)
                    continue;
                if(targetCharacter.isDead.Value)
                    continue;

                //友军还是敌人？
                if(WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    //检查是否在视线范围内
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if(viewableAngle > minimumDetectionAngle && viewableAngle < maximumDetectionAngle)
                    {
                        //检查是否有障碍物挡住视线
                        if(Physics.Linecast(
                            aiCharacter.transform.position, 
                            targetCharacter.characterCombatManager.lockOnTransform.position, 
                            WorldUtilityManager.instance.GetEnvironmentLayer()))
                        {
                            Debug.DrawLine(aiCharacter.transform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                            Debug.Log("AI cannot see the target due to an obstacle in the way");
                        }
                        else
                        {
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            Debug.Log("AI has found a target: " + targetCharacter.name);
                        }
                    }
                }



            }
        }
    }
}
