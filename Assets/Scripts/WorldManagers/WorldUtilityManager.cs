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

    }
}
                        