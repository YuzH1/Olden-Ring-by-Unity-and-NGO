using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class CharacterFootStepSFXMaker : MonoBehaviour
    {
       CharacterManager character;

       AudioSource audioSource;
       GameObject steppedOnObject;

       private bool hasTouchedGround = false; //这个布尔变量用来跟踪角色是否已经接触地面，防止在空中多次触发脚步音效
       private bool hasPlayedFootstepSFX = false; //这个布尔变量用来跟踪是否已经播放过脚步音效，防止在同一帧内多次播放

       [SerializeField] float distanceToGround = 0.05f;

        private void Awake() 
        {
            character = GetComponentInParent<CharacterManager>();
            audioSource = GetComponent<AudioSource>();

        }

        private void FixedUpdate()
        {
            CheckForFootSteps();
        }

        private void CheckForFootSteps()
        {
            if(character == null)
                return;
            if(character.isDead.Value)
                return;
            
            if(!character.characterNetworkManager.isMoving.Value)
                return;


            RaycastHit hit;
            if(Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.instance.GetEnvironmentLayer()))
            {
                hasTouchedGround = true;

                if(!hasPlayedFootstepSFX)
                {
                    steppedOnObject = hit.collider.gameObject;
                    
                }
            }
            else
            {
                hasTouchedGround = false;
                hasPlayedFootstepSFX = false; //重置脚步音效播放状态，以便在下一次接触地面时能够正确播放音效
                steppedOnObject = null;
            }

            if(hasTouchedGround && !hasPlayedFootstepSFX)
            {
                hasPlayedFootstepSFX = true; //设置脚步音效已播放，防止在同一帧内多次播放
                PlayFootstepSFX();
            }
        }

        private void PlayFootstepSFX()
        {
            if(steppedOnObject == null)
                return;

            //这里可以根据steppedOnObject的材质或者标签来选择不同的脚步音效，目前是随机选择一个
            //audioSource.PlayOneShot(WorldSoundFXManager.instance.ChooseRandomFootStepSFXBasedOnGround(steppedOnObject, character));

            // 随机选一个
            character.characterSoundFXManager.PlayFootStepSFX();
        }


    }
}
