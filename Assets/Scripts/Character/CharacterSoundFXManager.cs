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

        [Header("Footstep SFX")]//脚步音效
        public AudioClip[] footstepSFX;//默认脚步音效，如果没有根据地面类型区分的脚步音效，就使用这个数组中的音效来播放脚步声

        // 暂时先不使用根据地面类型区分的脚步音效数组，后续如果需要再添加这个功能
        
        // public AudioClip[] footstepSFXOnDirt;//在泥土上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnStone;//在石头上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnWood;//在木头上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnGrass;//在草地上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnWater;//在水面上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnSand;//在沙地上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnSnow; //在雪地上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnMetal;//在金属上行走时播放的脚步音效数组
        // public AudioClip[] footstepSFXOnGravel;//在碎石上行走时播放的脚步音效数组


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

        public virtual void PlayDamageGruntsSFX()
        {
            if(damageGrunts.Length == 0)
                return;
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }

        public virtual void PlayAttackGruntsSFX()
        {
            if(attackGrunts.Length == 0)
                return;
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }

        public virtual void PlayFootStepSFX()
        {
            if(footstepSFX.Length == 0)
                return;
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footstepSFX));
        }

    }


}