using UnityEngine;

namespace SG
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;


        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2f;//每秒恢复的耐力值，这个数值可以根据需要调整
        private float staminaRegenerationTimer = 0f;//这个计时器可以用来控制耐力恢复的频率，避免每帧都恢复耐力导致过快恢复
        private float staminaTickTimer = 0f;//这个计时器可以用来控制耐力恢复的频率，避免每帧都恢复耐力导致过快恢复
        [SerializeField] private float staminaRegenerationDelay = 2f;//这个参数可以用来设置在使用耐力后多久开始恢复耐力，增加游戏的策略性和挑战性


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)//根据体力等级计算生命值
        {
            float health = 0;

            //制作一个equation来计算生命值，
            health = vitality * 10;

            return Mathf.RoundToInt(health);
        }
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)//根据耐力等级计算耐力值
        {
            float stamina = 0;

            //制作一个equation来计算耐力值，
            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()//这个函数可以在玩家休息或使用某些道具时调用，来恢复耐力值
        {
            //只有拥有该对象的客户端才处理耐力恢复
            if(!character.IsOwner)
                return;
            
            //在使用体力时，不回复体力
            if(character.characterNetworkManager.isSprinting.Value)
                return;
            if(character.isPerformingAction)
                return;
            
            staminaRegenerationTimer += Time.deltaTime;//增加计时器

            if(staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if(character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;//增加耐力恢复的计时器
                    if(staminaTickTimer >= 0.1f) //每0.1秒恢复一次耐力，这个数值可以根据需要调整
                    {
                        staminaTickTimer = 0f; //重置计时器
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;//恢复耐力
                    }
                    
                }
                //当计时器达到恢复延迟时，恢复耐力值
            }
        }
        public virtual void ResetStaminaRegenerationTimer(float previousStaminaAmount, float currentStaminaAmount)//这个函数可以在玩家使用体力时调用，来重置耐力恢复的计时器
        {
            if(currentStaminaAmount < previousStaminaAmount)//只有当新的耐力值小于旧的耐力值时才重置计时器，避免在恢复耐力时重置计时器导致无法继续恢复
                staminaRegenerationTimer = 0;//重置计时器，这样在使用体力后需要等待一段时间才能开始恢复耐力
            
        }
   
    }
}