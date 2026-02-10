using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace SG
{
    public class UI_Match_Scroll_Wheel_To_Selected_Button : MonoBehaviour
    {
        [SerializeField] GameObject currentSelected;
        [SerializeField] GameObject previousSelected;
        [SerializeField] RectTransform currentSelectedRectTransform;
        [SerializeField] RectTransform contentPanel;
        [SerializeField] ScrollRect scrollRect;

        [Header("Scroll Settings")]
        [SerializeField] float scrollSpeed = 10f; //滚动速度，值越大滚动越快
        [SerializeField] float manualScrollThreshold = 0.1f; //手动滚动检测阈值

        private Vector2 targetPosition; //目标位置
        private Vector2 velocity = Vector2.zero; //用于SmoothDamp的速度变量
        private bool isManualScrolling = false; //是否正在手动滚动

        private void Update()
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;

            if(currentSelected != null)
            {
                if(currentSelected != previousSelected)
                {
                    previousSelected = currentSelected;
                    currentSelectedRectTransform = currentSelected.GetComponent<RectTransform>();
                    CalculateTargetPosition(currentSelectedRectTransform);
                }
            }

            // 检测是否正在手动拖动或惯性滚动
            isManualScrolling = scrollRect.velocity.magnitude > manualScrollThreshold;

            // 只有在非手动滚动时才执行自动平滑滚动
            if(!isManualScrolling)
            {
                contentPanel.anchoredPosition = Vector2.SmoothDamp(
                    contentPanel.anchoredPosition, 
                    targetPosition, 
                    ref velocity, 
                    1f / scrollSpeed
                );
            }
            else
            {
                // 手动滚动时，更新目标位置为当前位置，避免松手后跳回
                targetPosition = contentPanel.anchoredPosition;
                velocity = Vector2.zero;
            }
        }

        private void CalculateTargetPosition(RectTransform target)
        {
            Canvas.ForceUpdateCanvases(); //强制更新画布，确保布局信息是最新的

            //计算目标按钮相对于内容面板的位置
            Vector2 newPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) 
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

            newPosition.x = 0; //只在垂直方向滚动，保持水平位置不变

            targetPosition = newPosition; //设置目标位置，让SmoothDamp平滑过渡
        }

    }
}