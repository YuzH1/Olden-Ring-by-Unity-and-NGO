using UnityEngine;

namespace SG
{
    //玩家特效管理器，负责处理玩家的各种特效，如受伤、恢复、Buff等
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug工具，稍后删除")]
        [SerializeField] InstantCharacterEffect effectToTest; //测试用的即时效果
        [SerializeField] bool applyEffect = false; //是否应用测试效果

        private void Update()
        {
            if (applyEffect)
            {
                applyEffect = false;//每帧重置，避免重复应用测试效果
                //为什么要初始化一个copy的效果？因为如果直接使用effectToTest，可能会修改原始数据，导致后续测试不准确
                //InstantCharacterEffect effect = Instantiate(effectToTest); //创建一个copy的效果实例
                TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect; //创建一个copy的效果实例
                //effect.staminaDamage = 55; //设置测试效果的属性，例如造成10点耐力伤害
                ProcessInstantEffect(effect); //应用测试效果
            }

        }
    }
}