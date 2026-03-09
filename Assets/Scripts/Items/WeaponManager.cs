using UnityEngine;

namespace SG
{
    public class WeaponManager : MonoBehaviour
    {
        public MeleeWeaponDamageCollider meleeDamageCollider;

        private void Awake()
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager characterWieldWeapon, WeaponItem weapon)
        {
            meleeDamageCollider.characterCausingDamage = characterWieldWeapon;
            meleeDamageCollider.physicalDamage = weapon.physicalDamage;
            meleeDamageCollider.magicDamage = weapon.magicDamage;
            meleeDamageCollider.fireDamage = weapon.fireDamage;
            meleeDamageCollider.lightningDamage = weapon.lightningDamage;
            meleeDamageCollider.holyDamage = weapon.holyDamage;

            meleeDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_Multiplier;
            meleeDamageCollider.light_Attack_02_Modifier = weapon.light_Attack_02_Multiplier;
            meleeDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Multiplier;
            meleeDamageCollider.heavy_Attack_02_Modifier = weapon.heavy_Attack_02_Multiplier;
            meleeDamageCollider.charge_Heavy_Attack_01_Modifier = weapon.charge_Heavy_Attack_01_Multiplier;
            meleeDamageCollider.charge_Heavy_Attack_02_Modifier = weapon.charge_Heavy_Attack_02_Multiplier;
        }
    }
    
}
