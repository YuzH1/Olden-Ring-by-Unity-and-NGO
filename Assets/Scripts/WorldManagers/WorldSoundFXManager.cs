using UnityEngine;

namespace SG
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager instance;

        [Header("Damage SFX")]
        public AudioClip[] physicalDamageSFX;
        

        [Header("Action SFX")]
        public AudioClip rollSFX;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public AudioClip ChooseRandomSFXFromArray(AudioClip[] sfxArray)
        {
            if(sfxArray.Length == 0)
            {
                return null;
            }
            int randomIndex = Random.Range(0, sfxArray.Length);
            return sfxArray[randomIndex];
        }
    }
}