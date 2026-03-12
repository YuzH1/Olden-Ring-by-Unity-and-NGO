using UnityEngine;

namespace SG
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        private AudioSource audioSource;

        [Header("Damage Grunts")]//受击呻吟音效
        [SerializeField] protected AudioClip[] damageGrunts;

        [Header("Attack Grunts")]//攻击呻吟音效
        [SerializeField] protected AudioClip[] attackGrunts;


        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 播放音效的函数，可以选择是否随机化音调以增加变化感，默认随机化范围为0.1f
        /// </summary>
        /// <param name="soundFX">要播放的音效</param>
        /// <param name="volume">音量，默认为1</param>
        /// <param name="randomizePitch">是否随机化音调，默认为true</param>
        /// <param name="pitchRandom">音调随机化范围，默认为0.1</param> <summary>
        public void PlaySoundFX(AudioClip soundFX, float volume = 1f, bool randomizePitch = true, float pitchRandom = 0.1f)
        {
            audioSource.PlayOneShot(soundFX, volume);
            //重置音调为默认值1，然后根据参数决定是否随机化音调
            audioSource.pitch = 1f;
            if(randomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
            }
        
            
        }

        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
        }

        public virtual void PlayDamageGrunts()
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }

        public virtual void PlayAttackGrunts()
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }

    }


}