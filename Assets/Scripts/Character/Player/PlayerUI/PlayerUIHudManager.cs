using UnityEngine;

namespace SG
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;

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

    }
}