using System.Collections.Generic;
using SG;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider damageCollider; //伤害碰撞器组件

    [Header("Damage")]
    public float physicalDamage = 0;//未来会拓展为普通物理伤害、重击伤害、切割伤害、穿刺伤害等不同类型的物理伤害
    public float magicDamage = 0;//魔法伤害
    public float fireDamage = 0;//火焰伤害
    public float lightningDamage = 0;//闪电伤害
    public float holyDamage = 0;//神圣伤害
    [Header("Contect Point")]
    private Vector3 contectPoint; //伤害接触点的位置

    [Header("Character Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>(); //已经被这个伤害碰撞器造成过伤害的角色列表，用于避免重复伤害

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponent<CharacterManager>();

        if(damageTarget != null)
        {
            contectPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position); //获取伤害接触点的位置
        
            //检查当目标位友军时是否可以伤害目标

            //检查目标是否被阻挡

            //检查目标是否处于无敌状态

            //应用伤害效果
            DamageTarget(damageTarget); 
        }

    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        //我们不想让目标在一次攻击中造成多次伤害，
        //所以添加一个List在造成伤害前检查目标是否已经在List里了，如果在List里了，就不再造成伤害了，避免重复伤害
        if(charactersDamaged.Contains(damageTarget))
            return;
        
        charactersDamaged.Add(damageTarget); //将目标添加到已经造成过伤害的角色列表中

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect); //创建一个伤害效果实例
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contectPoint; //将伤害接触点的位置传递给伤害效果

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect); //将伤害效果应用到目标角色身上
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); //禁用伤害碰撞器时清空已经造成过伤害的角色列表，以便下一次启用时可以重新造成伤害
    }


}
