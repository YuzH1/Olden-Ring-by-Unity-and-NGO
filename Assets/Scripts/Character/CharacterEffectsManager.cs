using UnityEngine;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        //即时处理效果（如受伤、恢复等）

        //持续处理效果（如中毒、燃烧等）

        //静态处理效果（如增加或移除Buff等）
        CharacterManager character;

        

        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;
        

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }


        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)//处理即时效果
        {
            effect.ProcessEffect(character);
        }
            
        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            //如果有人物血迹特效，播放这个版本，
            if(bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            // 否则播放默认的血迹特效
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.Instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }



    }
    
}
