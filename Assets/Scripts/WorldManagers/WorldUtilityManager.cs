using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager instance;

        [Header("Layers")]
        [SerializeField] LayerMask characterLayer;
        [SerializeField] LayerMask environmentLayer;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public LayerMask GetCharacterLayer()
        {
            return characterLayer;
        }

        public LayerMask GetEnvironmentLayer()
        {
            return environmentLayer;
        }

        public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            if(attackingCharacter == CharacterGroup.Team01)
            {
                switch(targetCharacter)
                {
                    case CharacterGroup.Team01: return false;
                    case CharacterGroup.Team02: return true;
                    default:
                        break;
                }
            }
            else if(attackingCharacter == CharacterGroup.Team02)
            {
                switch(targetCharacter)
                {
                    case CharacterGroup.Team01: return true;
                    case CharacterGroup.Team02: return false;
                    default:
                        break;
                }
            }

            return false;
        }

        public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
        {
            targetDirection.y = 0;
            float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);//计算角色朝向目标的角度，使用Vector3.Angle方法计算角色的前方向与目标方向之间的夹角，得到一个表示视野范围内目标的角度值
            Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);//计算角色朝向目标的方向，使用Vector3.Cross方法计算角色的前方向与目标方向之间的叉积，得到一个向量，该向量的方向可以用来判断目标是在角色的左侧还是右侧

            if(cross.y < 0)
                viewableAngle = -viewableAngle;

            return viewableAngle;
        }

    }
}
                        