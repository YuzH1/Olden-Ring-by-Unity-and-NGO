using UnityEngine;

namespace SG
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager characterManager;

        float Vertical;
        float Horizontal;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            // Implementation for updating animator parameters
            //方法1：直接设置Animator参数，这种方法简单直接，但可能会导致动画切换不够平滑，特别是在输入值变化较大时
            //0.1f是阻尼时间，Time.deltaTime确保每帧都平滑过渡
            characterManager.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            characterManager.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);

        }
    }
}