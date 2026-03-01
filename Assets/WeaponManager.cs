using UnityEngine;

namespace SG
{
    public class WeaponManager : MonoBehaviour
    {
        MeleeWeaponDamageCollider meleeDamageCollider;

        private void Awake()
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(WeaponItem weapon)
        {
            meleeDamageCollider.physicalDamage = weapon.physicalDamage;
            meleeDamageCollider.magicDamage = weapon.magicDamage;
            meleeDamageCollider.fireDamage = weapon.fireDamage;
            meleeDamageCollider.lightningDamage = weapon.lightningDamage;
            meleeDamageCollider.holyDamage = weapon.holyDamage;
        }
    }
    
}
