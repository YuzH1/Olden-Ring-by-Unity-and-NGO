using UnityEngine;

namespace SG
{
    public class UnDeadHandDamageCollider : DamageCollider
    {
       [SerializeField] AICharacterManager undeadCharacterCausingDamage;//当计算伤害时用来检查攻击来源的伤害修正

        protected override void Awake()
        {
            base.Awake();

            damageCollider = GetComponent<Collider>(); //获取伤害碰撞器组件
            undeadCharacterCausingDamage = GetComponentInParent<AICharacterManager>(); //在这个伤害碰撞器的父对象中找到一个带有AICharacterManager组件的对象，并将其赋值给undeadCharacterCausingDamage变量，这样伤害碰撞器就可以使用这个变量来获取攻击来源的角色信息和伤害修正等数据

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
            damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up); //计算伤害来源的角度，并传递给伤害效果

            

            if(damageTarget.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyServerCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    undeadCharacterCausingDamage.NetworkObjectId,
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
    }
}
