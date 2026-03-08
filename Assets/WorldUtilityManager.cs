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

    }
}