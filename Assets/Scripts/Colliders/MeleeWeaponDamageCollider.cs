using UnityEngine;

namespace SG
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage;//当计算伤害时用来检查攻击来源的伤害修正

        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier;//轻攻击01的伤害修正
        public float heavy_Attack_01_Modifier;//重攻击01的伤害修正
        public float charge_Heavy_Attack_01_Modifier;//蓄力重攻击01的伤害修正
        protected override void Awake()
        {
            base.Awake();

            if(damageCollider == null)
            {
                damageCollider = GetComponent<Collider>(); //尝试在当前对象上获取Collider组件
            }
            damageCollider.enabled = false; //默认情况下禁用伤害碰撞器，只有在攻击动画的特定帧才启用它

        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();


            if (damageTarget == null)
            {
                damageTarget = other.GetComponent<CharacterManager>();
            }



            if (damageTarget != null)
            {
                if (damageTarget == characterCausingDamage)
                {
                    return; //如果碰撞到的角色是造成伤害的角色自己，就不造成伤害
                }

                contectPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position); //获取伤害接触点的位置

                //检查当目标位友军时是否可以伤害目标

                //检查目标是否被阻挡

                //检查目标是否处于无敌状态

                //应用伤害效果
                DamageTarget(damageTarget);
            }
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            //我们不想让目标在一次攻击中造成多次伤害，
            //所以添加一个List在造成伤害前检查目标是否已经在List里了，如果在List里了，就不再造成伤害了，避免重复伤害
            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget); //将目标添加到已经造成过伤害的角色列表中

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect); //创建一个伤害效果实例
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contectPoint; //将伤害接触点的位置传递给伤害效果
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up); //计算伤害来源的角度，并传递给伤害效果

            switch(characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.ChargeHeavyAttack01:
                    ApplyAttackModifiers(charge_Heavy_Attack_01_Modifier, damageEffect);
                    break;
                //如果有其他攻击类型，也在这里添加对应的伤害修正项

                default:
                    break;  
            }


             //如果敌人为AI，寻找下一个目标锁定

             //如果目标死亡了，处理目标死亡的逻辑，例如播放死亡动画、掉落物品等
            

            //damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect); //将伤害效果应用到目标角色身上

            //这里添加一个检查，只有造成伤害的角色的客户端才将伤害效果应用到目标角色身上，避免多个客户端同时处理伤害效果导致的冲突和不一致
            if(characterCausingDamage.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyServerCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.lightningDamage,
                    damageEffect.holyDamage,
                    damageEffect.poiseDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z
                ); //调用目标角色的网络管理器的服务器RPC函数，传入目标角色ID、造成伤害的角色ID、伤害数据、伤害来源角度和伤害接触点位置等信息，让服务器通知所有客户端处理伤害效果
                 //将伤害效果应用到目标角色身上
            }
        }

        private void ApplyAttackModifiers(float modifier, TakeDamageEffect damage)
        {
            damage.physicalDamage *= modifier;
            damage.magicDamage *= modifier;
            damage.fireDamage *= modifier;
            damage.lightningDamage *= modifier;
            damage.holyDamage *= modifier;
            damage.poiseDamage *= modifier;

            //如果有其他类型的伤害修正项，也在这里应用，例如重击伤害修正，敌人弱点伤害修正等
        }
    }
}
