using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager instance;
        [Header("Boss Track")]
        [SerializeField] AudioSource bossIntroPlayer;
        [SerializeField] AudioSource bossLoopPlayer;
        [SerializeField] AudioSource bossDefeatedPlayer;

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

        public void PlayBossTrack(AudioClip ibntroTrack, AudioClip loopTrack)
        {
            bossIntroPlayer.volume = 1; //确保引入曲目的音量设置为1，这样它在播放时能够正确地听到
            bossIntroPlayer.clip = ibntroTrack;
            bossLoopPlayer.loop = false;
            bossIntroPlayer.Play();
            
            bossLoopPlayer.volume = 1; //确保循环曲目的音量设置为1，这样它在播放时能够正确地听到
            bossLoopPlayer.clip = loopTrack;
            bossLoopPlayer.loop = true;
            bossLoopPlayer.PlayDelayed(ibntroTrack.length); //在引入曲目播放完后开始播放循环曲目，确保音乐的无缝衔接
        }

        public void PlayBossDefeatedTrack(AudioClip defeatedTrack)
        {
            bossDefeatedPlayer.volume = 1; //确保Boss被击败曲目的音量设置为1，这样它在播放时能够正确地听到
            bossDefeatedPlayer.clip = defeatedTrack;
            bossDefeatedPlayer.loop = false;
            bossDefeatedPlayer.Play();
        }
        public void StopBossTrack()
        {
            StartCoroutine(FadeOutBossTrackOverTime(2)); //在2秒内淡出Boss的背景音乐，确保音乐的平滑过渡
        }

        private IEnumerator FadeOutBossTrackOverTime(float duration)
        {

            while(bossLoopPlayer.volume > 0)
            {
                bossIntroPlayer.volume -= Time.deltaTime / duration; //在duration秒内将引入曲目的音量从1平滑过渡到0，达到淡出的效果
                bossLoopPlayer.volume -= Time.deltaTime / duration; //在duration秒内将循环曲目的音量从1平滑过渡到0，达到淡出的效果
                yield return null;
            }

            bossIntroPlayer.Stop();
            bossLoopPlayer.Stop();
        }

        // 根据角色当前脚下的地面类型选择合适的脚步音效，如果没有对应地面类型的音效，就使用默认的脚步音效数组来播放 
        // 暂时不根据地面类型区分脚步音效，后续如果需要再添加这个功能
        // public AudioClip ChooseRandomFootStepSFXBasedOnGround(GameObject steppedOnObject, CharacterManager character)
        // {
        //     return steppedOnObject.tag switch
        //     {
        //         "Untagged" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnDirt),
        //         "Dirt" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnDirt),
        //         "Stone" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnStone),
        //         "Wood" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnWood),
        //         "Grass" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnGrass),
        //         "Water" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnWater),
        //         "Sand" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnSand),
        //         "Snow" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnSnow),
        //         "Metal" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnMetal),
        //         "Gravel" => ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepSFXOnGravel),
        //         _ => null
        //     };
        // }
    }
}