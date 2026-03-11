using UnityEngine;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManager
    {

        [Header("Target Info")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 10f;
        public float minimumFOV = -35f;//最小检测角度，单位为度，表示AI角色能够检测到目标的视野范围
        public float maximumFOV = 35f;//最大检测角度，单位为度，表示AI角色能够检测到目标的视野范围

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
                    float angelOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if(angelOfPotentialTarget > minimumFOV && angelOfPotentialTarget < maximumFOV)
                    {
                        //检查是否有障碍物挡住视线
                        if(Physics.Linecast(
                            aiCharacter.transform.position, 
                            targetCharacter.characterCombatManager.lockOnTransform.position, 
                            WorldUtilityManager.instance.GetEnvironmentLayer()))
                        {
                            Debug.DrawLine(aiCharacter.transform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                        }
                        else
                        {
                            targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(aiCharacter.transform, targetDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }



            }
        }
    
        public void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            //播放一个基于目标视角的pivot动画
            if(aiCharacter.isPerformingAction)
                return;

            if(viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
            }
            else if(viewableAngle <= -20 && viewableAngle >= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
            }
            else if(viewableAngle > 60 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if(viewableAngle < -60 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            else if(viewableAngle > 110 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
            }
            else if(viewableAngle < -110 && viewableAngle >= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
            }
            else if(viewableAngle > 145 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if(viewableAngle < -145 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }

        }
    
    
    
    }
}
