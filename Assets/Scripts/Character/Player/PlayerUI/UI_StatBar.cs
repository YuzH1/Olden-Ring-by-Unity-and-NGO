using UnityEngine;
using UnityEngine.UI;   

namespace SG
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;     
        private RectTransform rectTransform; //数据条的RectTransform组件，用于调整数据条的长度
        //改变数据条的数值决定于当前数值与最大数值的比例
        //次要数据条表示当前花费的数值
        
        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true; //是否根据当前数值与最大数值的比例来缩放数据条的长度
        [SerializeField] protected float widthScaleMultiplier = 1f; //数据条长度的缩放倍率，默认为1，即不缩放


        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if(scaleBarLengthWithStats)
            {
                //根据当前数值与最大数值的比例来缩放数据条的长度
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y); //调整数据条的长度，保持高度不变
                PlayerUIManager.Instance.playerUIHudManager.RefreshHUD(); //调整数据条的位置，确保它们在UI中正确对齐
            }
        }
        
    }

}
