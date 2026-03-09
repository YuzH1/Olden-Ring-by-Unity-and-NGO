using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;
        
        [Header("Quick Slots")]
        [SerializeField] Image rightWeaponQuickSlotIcon;
        [SerializeField] Image leftWeaponQuickSlotIcon;
        

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false); //先将生命值数据条设置为不活跃，隐藏它
            healthBar.gameObject.SetActive(true); //再将生命值数据条设置为活跃，显示它，这样可以强制刷新数据条的显示，确保它们在UI中正确对齐
            staminaBar.gameObject.SetActive(false); //先将耐力值数据条设置为不活跃，隐藏它
            staminaBar.gameObject.SetActive(true); //再将耐力值数据条设置为活跃，显示它，这样可以强制刷新数据条的显示，确保它们在UI中正确对齐
        }

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            if(weapon == null)
            {
                Debug.LogWarning("在数据库中未找到 " + weaponID );
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }
            
            if(weapon.itemIcon == null)
            {
                Debug.LogWarning("此武器：" + weaponID + " 没有在数据库中设置图标");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;
            
        }

        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            if(weapon == null)
            {
                Debug.LogWarning("在数据库中未找到 " + weaponID );
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }
            
            if(weapon.itemIcon == null)
            {
                Debug.LogWarning("此武器：" + weaponID + " 没有在数据库中设置图标");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;
            
        }

    }
}