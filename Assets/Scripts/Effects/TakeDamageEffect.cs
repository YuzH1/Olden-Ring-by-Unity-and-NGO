using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage Effect")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; //造成伤害的角色，如果伤害来自别的角色，会存储在这里

        [Header("Damage")]
        public float physicalDamage = 0;//未来会拓展为普通物理伤害、重击伤害、切割伤害、穿刺伤害等不同类型的物理伤害
        public float magicDamage = 0;//魔法伤害
        public float fireDamage = 0;//火焰伤害
        public float lightningDamage = 0;//闪电伤害
        public float holyDamage = 0;//神圣伤害

        //TO DO
        //BUILD UPS
        //未来会添加更多类型的伤害，比如毒素伤害、流血伤害、精神伤害等

        [Header("Final Damage")]
        public int finalDamageDealt = 0; //最终伤害值，经过各种计算后得到的结果，将会应用到角色身上

        [Header("Poise")]
        [Tooltip("韧性一个衡量角色抵抗被打断的能力的属性，当韧性值降低到一定程度时，角色会被打断，无法继续当前的动作，必须等待一段时间才能恢复韧性并继续行动")]
        public float poiseDamage = 0;//韧性伤害值
        public bool poiseIsBroken = false;//角色的韧性是否被打破，如果被打破了，角色会进入被打断的状态，无法继续当前的动作，必须等待一段时间才能恢复韧性并继续行动

        [Header("Animation")]
        public bool playDamageAnimation = true; //是否播放受击动画
        public bool manuallySelectDamageAnimation = false; //是否手动选择受击动画，默认为false，即根据伤害类型自动选择受击动画
        public string damageAnimation;//手动选择的受击动画的名称，只有当manuallySelectDamageAnimation为true时才会使用这个字段
        
        [Header("Sound FX")]
        public bool willPlayDamageSFX = true; //是否播放受击音效
        public AudioClip elementalDamageSFX; //根据伤害类型播放不同的受击音效，比如火焰伤害播放火焰受击音效，闪电伤害播放闪电受击音效等

        [Header("Direction Damage Taken From")]
        public float angleHitFrom; //伤害来源的角度，用于根据攻击方向播放不同的受击动画，比如从前面攻击播放前受击动画，从后面攻击播放后受击动画等
        public Vector3 contactPoint; //伤害接触点的位置，可以用来在角色身上生成受击特效，比如血液飞溅、火焰爆炸等

        public override void ProcessEffect(CharacterManager character)
        {
            if(character.characterNetworkManager.isInvulnerable.Value)
                return; //如果角色处于无敌状态，就不处理伤害效果了，避免出现无敌状态下受伤的情况

            base.ProcessEffect(character);

            //如果角色已经死亡了，就不再处理伤害效果了，避免出现死后受伤的情况
            if(character.isDead.Value)
                return;

            //(TO DO)检查是否是无敌状态，如果是无敌状态，就不再处理伤害效果了，避免出现无敌状态下受伤的情况

            //计算伤害
            CalculateDamage(character);
            //检查伤害方向
            //根据伤害类型播放不同的受击动画
            PlayDirectionalBasedDamageAnimation(character);
            //检查额外效果，中毒流血等
            //播放受击特效，比如血液飞溅、火焰爆炸等
            PlayDamageVFX(character);

            //播放受击音效
            PlayDamageSFX(character);
            // if(willPlayDamageSFX)
            // {
            //     PlayDamageSFX(character);
            // }

            //如果敌人为AI，寻找下一个目标锁定

        }

        private void CalculateDamage(CharacterManager character)
        {
            if(!character.IsOwner)
            {
                return; //只有拥有该对象的客户端才计算伤害，避免多个客户端同时计算伤害导致的冲突和不一致
            }
            if(characterCausingDamage != null)
            {
                //检查伤害修正项，并修正伤害（Buff，Debuff，装备属性，敌人弱点等）
            }
            //检查角色对伤害固定减免（如神圣，火，电，冰等伤害会带有固定的伤害减免）

            //检查角色护甲值，护甲值可以根据伤害类型提供不同的减免

            //将所有伤害类型加在一起，得到最终伤害值
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage - /*减免项*/0);

            if(finalDamageDealt <= 0)
            {
                finalDamageDealt = 1; //避免出现回血的情况
            }

            Debug.Log("Final Damage Dealt: " + finalDamageDealt);
            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt; //将最终伤害值应用到角色身上，减少角色的当前生命值
        
            //计算韧性伤害，检查是否打破韧性
        }
    
        private void PlayDamageVFX(CharacterManager character)
        {
            //根据不同伤害类型播放不同效果
            //火焰伤害播放火焰特效，闪电伤害播放闪电特效等

            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint); //播放血迹飞溅特效，传入伤害接触点的位置

        }

        private void PlayDamageSFX(CharacterManager character)
        {
            //从物理伤害音效数组中随机选择一个音效
            AudioClip phsicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX); 

            character.characterSoundFXManager.PlaySoundFX(phsicalDamageSFX);
            character.characterSoundFXManager.PlayDamageGruntsSFX(); //播放受击呻吟音效
            //根据不同伤害类型播放不同音效
            //火焰伤害播放火焰受击音效，闪电伤害播放闪电受击音效等

        }

        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if(!character.IsOwner)
                return;
            
            if(character.isDead.Value)
                return;

            //TODO:当被打断时计算
            poiseIsBroken = true;


            //根据伤害来源的角度播放不同的受击动画
            //比如从前面攻击播放前受击动画，从后面攻击播放后受击动画等

            if(angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                //播放前受击动画
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if(angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                //播放前受击动画
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if(angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                //播放后受击动画
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            else if(angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                //播放左受击动画
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            else if(angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                //播放右受击动画
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }

            //如果角色姿态被打破了，播放受击动画
            if(poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation; //记录当前播放的受击动画的名称
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }
    
    }

}