using UnityEngine;
using UnityEngine.UI;   

namespace SG
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;     
        //改变数据条的数值决定于当前数值与最大数值的比例
        //次要数据条表示当前花费的数值

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;
        }
        
    }

}
