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

        //武器战灰升级
        //锋利，厚重，轻便

        [Header("Stamina Costs")]
        public int baseStaminaCost = 20;
        //体力消耗修正
        //锋利，厚重，轻便


        //物品基础动作（轻攻击，重攻击，防御，战技）
        [Header("Actions")]
        public WeaponItemAction OH_RB_Action;//单手rb攻击动作

        //战灰

        //防御音效

       

    }

}
