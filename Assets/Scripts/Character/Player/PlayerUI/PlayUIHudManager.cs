using UnityEngine;

namespace SG
{
    public class PlayUIHudManager : MonoBehaviour
    {
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(float maxStamina)
        {
            staminaBar.SetMaxStat(Mathf.RoundToInt(maxStamina));
        }

        public void SetNewHealthValue(float oldValue, float newValue)
        {
            healthBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxHealthValue(float maxHealth)
        {
            healthBar.SetMaxStat(Mathf.RoundToInt(maxHealth));
        }

    }
}