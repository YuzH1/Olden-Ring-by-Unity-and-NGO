using System.Diagnostics;
using UnityEngine;
using UnityEngine.TextCore.Text;

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