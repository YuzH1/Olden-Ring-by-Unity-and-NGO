using UnityEngine;

public class Enums : MonoBehaviour
{
    
}

public enum CharacterSlots
{
    characterSlot_01,
    characterSlot_02,
    characterSlot_03,
    characterSlot_04,
    characterSlot_05,
    characterSlot_06,
    characterSlot_07,
    characterSlot_08,
    characterSlot_09,
    characterSlot_10,
    No_Slot,
}

public enum CharacterGroup//角色阵营
{
    Team01,
    Team02,
    // Team03,
}

public enum WeaponModelSlot
{
    RightHand,
    LeftHand,
    // RightHips,
    // LeftHips,
    // Back,
}

//用来计算不同攻击类型的伤害倍率和体力消耗倍率
public enum AttackType
{
    LightAttack01,
    LightAttack02,
    HeavyAttack01,
    ChargeHeavyAttack01,
    HeavyAttack02,
    ChargeHeavyAttack02,
}










