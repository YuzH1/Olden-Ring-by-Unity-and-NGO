using System.Collections;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("YOU DIED Pop-Up")]
        [SerializeField] GameObject youDiedPopUpGameObject; //你死了弹窗的GameObject
        [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText; //你死了弹窗中的背景文本组件，用于显示提示信息的文字
        [SerializeField] TextMeshProUGUI youDiedPopUpTYext; //你死了弹窗中的文本组件，用于显示提示信息的文字
        [SerializeField] CanvasGroup youDiedPopUpCanvasGroup; //你死了弹窗的CanvasGroup组件，用于控制弹窗的显示和隐藏

        public void SendYouDiedPopUp()
        {
            //启用后期处理效果

            StopAllCoroutines();

            youDiedPopUpGameObject.SetActive(true); //显示你死了弹窗
            youDiedPopUpCanvasGroup.alpha = 0;
            youDiedPopUpBackgroundText.characterSpacing = 0; //重置背景文本的字符间距，确保它在弹窗显示时正确对齐
            //文本拉长，淡入
            StartCoroutine(StretchPopUpOverTime(youDiedPopUpBackgroundText, 12, 15)); //将背景文本的字符间距在1秒内从0平滑过渡到20，达到文本拉长的效果
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 3)); //将弹窗的alpha值在1秒内从0平滑过渡到1，达到淡入的效果

            //一段时间后淡出
            //将弹窗的alpha值在1秒内从1平滑过渡到0，达到淡出的效果，延迟3秒后开始淡出
            StartCoroutine(FadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5)); 
        }

        private IEnumerator StretchPopUpOverTime(TextMeshProUGUI text, float duration, float strechAmount)
        {
            if(duration > 0)
            {
                text.characterSpacing = 0; //重置文本的字符间距，确保它在弹窗显示时正确对齐
                float timer = 0;
                float startSpacing = text.characterSpacing;

                yield return null;

                while(timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    float t = Mathf.Clamp01(timer / duration);
                    //使用线性插值函数来平滑字符间距的变化，达到文本拉长的效果，duration越大，拉长的速度越慢
                    text.characterSpacing = Mathf.Lerp(startSpacing, strechAmount, t);
                    yield return null; //等待一帧，继续下一次循环
                }

                text.characterSpacing = strechAmount;
            }
        }

        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvasGroup, float duration)
        {
            if(duration > 0)
            {
                canvasGroup.alpha = 0; //重置CanvasGroup的alpha值，确保它在弹窗显示时正确对齐
                float timer = 0;
                float startAlpha = canvasGroup.alpha;

                yield return null;

                while(timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    float t = Mathf.Clamp01(timer / duration);
                    //使用线性插值函数来平滑alpha值的变化，达到淡入的效果，duration越大，淡入的速度越慢
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t);
                    yield return null; //等待一帧，继续下一次循环
                }
            }

            canvasGroup.alpha = 1; //确保alpha值最终为1，完全显示弹窗

            yield return null;//等待一帧，确保弹窗完全显示后再开始淡出
        }
    
        private IEnumerator FadeOutPopUpOverTime(CanvasGroup canvasGroup, float duration, float delay)
        {
            if(duration > 0)
            {
                while(delay > 0)
                {
                    delay = delay - Time.deltaTime;
                    yield return null; //等待一帧，直到延迟时间结束
                }

                float timer = 0;
                float startAlpha = canvasGroup.alpha;

                yield return null;

                while(timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    float t = Mathf.Clamp01(timer / duration);
                    //使用线性插值函数来平滑alpha值的变化，达到淡出的效果，duration越大，淡出的速度越慢
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);
                    yield return null; //等待一帧，继续下一次循环
                }

                youDiedPopUpGameObject.SetActive(false); //淡出完成后隐藏你死了弹窗
            }

            canvasGroup.alpha = 0; //确保alpha值最终为0，完全隐藏弹窗

            yield return null;//等待一帧，确保弹窗完全隐藏后再执行后续逻辑
            
        }
    
    }
}
