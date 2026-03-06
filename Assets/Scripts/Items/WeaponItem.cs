using SG;
using UnityEngine;

namespace SG
{
    public class WeaponItem : Item
    {
        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        public int strengthREQ = 0;//力量需求
        public int dexhREQ = 0;//敏捷需求
        public int intREQ = 0;//智力需求
        public int faithREQ = 0;//信仰需求

        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int holyDamage = 0;

        //武器防御能力

        [Header("Weapon Base Poise Damage")]
        public float poiseDamage = 10;
        //特殊武器攻击时增加自身韧性

        //武器伤害修正
        [Header("Attack Modifiers")]
        public float light_Attack_01_Multiplier = 1.1f;//轻攻击伤害倍率

        //锋利，厚重，轻便

        [Header("Stamina Cost Modifiers")]
        public int baseStaminaCost = 20;
        //体力消耗修正
        public float lightAttackStaminaCostMultiplier = 0.9f;//轻攻击体力消耗倍率
        //锋利，厚重，轻便


        //物品基础动作（轻攻击，重攻击，防御，战技）
        [Header("Actions")]
        public WeaponItemAction OH_RB_Action;//单手rb攻击动作

        //战灰

        //防御音效

       

    }

}
